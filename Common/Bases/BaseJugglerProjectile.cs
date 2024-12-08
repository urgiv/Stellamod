﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    internal class JugglePlayer : ModPlayer
    {
        public float combo;
        public void ResetCombo()
        {
            combo = 0;
        }
    }

    internal abstract class BaseJugglerProjectile : ModProjectile
    {
        private enum AIState
        {
            Thrown,
            Catch
        }

        private bool _setInitialVelocity;
        private Vector2 InitialVelocity;
        private ref float Timer => ref Projectile.ai[0];
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        private Player Owner => Main.player[Projectile.owner];
        protected JugglePlayer Juggler => Owner.GetModPlayer<JugglePlayer>();
        protected float GlowProgress;
        protected float ClickDistance;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            GlowProgress = 1f;
            ClickDistance = 64;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(InitialVelocity);
            writer.Write(_setInitialVelocity);
            writer.Write(GlowProgress);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            InitialVelocity = reader.ReadVector2();
            _setInitialVelocity = reader.ReadBoolean();
            GlowProgress = reader.ReadSingle();
        }

        public override void AI()
        {
            base.AI();

            //Idk if I actually have to do it this way but ehhh not taking chances
            if (!_setInitialVelocity)
            {
                InitialVelocity = Projectile.velocity;
                _setInitialVelocity = true;
                Projectile.netUpdate = true;
            }

            switch (State)
            {
                case AIState.Thrown:
                    AI_Thrown();
                    break;
                case AIState.Catch:
                    AI_Catch();
                    break;
            }
        }

        private void SwitchState(AIState state)
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }

        private void AI_Thrown()
        {
            Timer++;
            if (Timer == 1)
            {

            }

            if (Timer % 16 == 0)
            {
                //A little bit of dust never hurt anyone
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin);
            }

            GlowProgress *= 0.98f;
            float maxDetectDistance = 1024;
            NPC nearest = ProjectileHelper.FindNearestEnemyThroughWalls(Projectile.position, maxDetectDistance);
            if (nearest != null)
            {
                //Slight homing
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, 1);
            }
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
        }

        private void AI_Catch()
        {
            Timer++;
            Projectile.velocity *= 0.94f;
            Projectile.velocity.Y += 0.2f;

            Vector2 mouseWorld = Main.MouseWorld;
            float distanceToMouse = Vector2.Distance(mouseWorld, Projectile.Center);
            bool successfulHit = distanceToMouse < ClickDistance;


            if (Main.myPlayer == Projectile.owner && Timer > 15)
            {

                if (Owner.controlUseItem)
                {
     
                    if (successfulHit)
                    {
                        //Some Effects IDK
                        float maxDetectDistance = 1024;
                        for (float i = 0; i < 4; i++)
                        {
                            float progress = i / 4f;
                            float rot = progress * MathHelper.ToRadians(360);
                            Vector2 offset = rot.ToRotationVector2() * 24;
                            var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                                innerColor: Color.White,
                                glowColor: Color.LightGray,
                                outerGlowColor: Color.Black);
                            particle.Rotation = rot + MathHelper.ToRadians(45);
                        }

                        for (float i = 0; i < 8; i++)
                        {
                            float progress = i / 4f;
                            float rot = progress * MathHelper.ToRadians(360);
                            Vector2 dustVelocity = rot.ToRotationVector2() * 6;
                            Dust.NewDustPerfect(Projectile.Center, DustID.SilverCoin, dustVelocity);
                        }

                        //Show Combo Count
                        int combatText = CombatText.NewText(Juggler.Player.getRect(), Color.White, $"x{Juggler.combo + 1}", true);
                        CombatText numText = Main.combatText[combatText];
                        numText.lifeTime = 60;

                        SoundStyle jugglerPong = SoundRegistry.JugglerPong;
                        jugglerPong.PitchVariance = 0.15f;
                        SoundEngine.PlaySound(jugglerPong, Projectile.position);
                        NPC nearest = ProjectileHelper.FindNearestEnemyThroughWalls(Projectile.position, maxDetectDistance);
                        if (nearest != null)
                        {
                            Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, InitialVelocity, 900);
                            SwitchState(AIState.Thrown);
                        }
                        else
                        {
                            SwitchState(AIState.Thrown);
                        }

                        GlowProgress = 1f;
                        Juggler.combo++;
                    }
                    else
                    {
                        //FAIL
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dirt"), Projectile.position);
                        Projectile.Kill();
                    }
                }
            }
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            float increasePerHit = 0.05f;

            //Increase damage with each hit in the combo
            modifiers.FinalDamage *= (1f + Juggler.combo * increasePerHit);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return State == AIState.Thrown;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.timeLeft = 180;
            Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, Owner.Center, InitialVelocity, 900);
            Projectile.velocity.Y -= 7;
            SwitchState(AIState.Catch);

            float catchCount = Juggler.combo;
            float pitch = MathHelper.Clamp(catchCount * 0.05f, 0f, 1f);
            SoundStyle jugglerHit = SoundRegistry.JugglerHit;
            jugglerHit.Pitch = pitch;
            jugglerHit.PitchVariance = 0.1f;
            jugglerHit.Volume = 0.5f;
            SoundEngine.PlaySound(jugglerHit, Projectile.position);

            if (Juggler.combo >= 5)
            {
                SoundStyle jugglerHitMax = SoundRegistry.JugglerHitMax;
                pitch = MathHelper.Clamp(catchCount * 0.02f, 0f, 1f);
                jugglerHitMax.Pitch = pitch;
                jugglerHitMax.PitchVariance = 0.1f;
                SoundEngine.PlaySound(jugglerHitMax, Projectile.position);
            }

            for (int i = 0; i < 4; i++)
            {
                //Get a random velocity
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);

                //Get a random
                float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                ParticleManager.NewParticle<StarParticle2>(target.Center, velocity, Color.DarkGoldenrod, randScale);
            }
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;

        public virtual float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, baseWidth, completionRatio);
        }

        public virtual Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.White;
            return Color.Lerp(startColor, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail(ref lightColor);
            DrawSprite(ref lightColor);
            return false;
        }

        private void DrawTrail(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.RestartDefaults();
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.Dashtrail);
            Vector2 trailOffset = -Main.screenPosition + Projectile.Size / 2;
            TrailDrawer.DrawPrims(Projectile.oldPos, trailOffset, 155);
        }

        private void DrawSprite(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = Projectile.rotation;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = 1f;

            if (State == AIState.Catch && Timer < 12)
            {
                drawColor = drawColor.MultiplyRGB(Color.DarkGray);
            }

            spriteBatch.Draw(texture, drawPos, null, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            spriteBatch.Restart(blendState: BlendState.Additive);

            if (Main.myPlayer == Projectile.owner)
            {
                float distanceToMouse = Vector2.Distance(Main.MouseWorld, Projectile.Center);
                bool successfulHit = distanceToMouse < ClickDistance;

                if (successfulHit && State == AIState.Catch)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        Color glowColor = Color.Lerp(Color.White, Color.Transparent, distanceToMouse / ClickDistance);
                        spriteBatch.Draw(texture, drawPos, null, glowColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                    }
                }
            }


            for (int i = 0; i < 3; i++)
            {
                Color glowColor = drawColor * GlowProgress;
                spriteBatch.Draw(texture, drawPos, null, glowColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
            spriteBatch.RestartDefaults();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dirt"), Projectile.position);
            Juggler.ResetCombo();
        }
    }
}
