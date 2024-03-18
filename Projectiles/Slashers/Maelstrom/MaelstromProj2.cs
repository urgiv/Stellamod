﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Dusts;
using Stellamod.Trails;
using Stellamod.Utilis;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Stellamod.Items.Accessories.Players;
using ParticleLibrary;
using Stellamod.Particles;

namespace Stellamod.Projectiles.Slashers.Maelstrom
{
    public class MaelstromProj2 : ModProjectile
    {
        public static bool swung = false;
        public int SwingTime = 240;
        public float holdOffset = 60f;
        public int combowombo;
        private bool _initialized;
        private int timer;
        private bool ParticleSpawned;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 4;
        }

        private Player Owner => Main.player[Projectile.owner];

        public float SwingDistance;
        public float Curvature;

        public ref float AiState => ref Projectile.ai[1];
        private Vector2 returnPosOffset; //The position of the projectile when it starts returning to the player from being hooked
        private Vector2 npcHookOffset = Vector2.Zero; //Used to determine the offset from the hooked npc's center
        private float npcHookRotation; //Stores the projectile's rotation when hitting an npc
        private NPC hookNPC; //The npc the projectile is hooked into

        public const float THROW_RANGE = 320; //Peak distance from player when thrown out, in pixels
        public const float HOOK_MAXRANGE = 800; //Maximum distance between owner and hooked enemies before it automatically rips out
        public const int HOOK_HITTIME = 1; //Time between damage ticks while hooked in
        public const int RETURN_TIME = 6; //Time it takes for the projectile to return to the owner after being ripped out

        public bool Flip = false;
        public bool Slam = false;
        public bool PreSlam = false;

        public Vector2 CurrentBase = Vector2.Zero;
        public override void SetDefaults()
        {
            Projectile.timeLeft = SwingTime;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 100;
            Projectile.width = 100;
            Projectile.friendly = true;
            Projectile.scale = 1f;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public virtual float Lerp(float val)
        {
            return val == 1f ? 1f : (val == 1f ? 1f : (float)Math.Pow(2, val * 10f - 10f) / 2f);
        }

        int Timeddeath = 0;
        public override void AI()
        {
            Timeddeath++;



            //   if (Timeddeath >= 45)
            // {
            //       Projectile.Kill();
            //  }

            Player player = Main.player[Projectile.owner];
            if (!_initialized && Main.myPlayer == Projectile.owner)
            {
                timer++;

                SwingTime = (int)(240 / player.GetAttackSpeed(DamageClass.Melee));
                Projectile.alpha = 255;
                Projectile.timeLeft = SwingTime;
                _initialized = true;
                Projectile.damage -= 9999;
                //Projectile.netUpdate = true;

            }
            else if (_initialized)
            {
                if (!player.active || player.dead || player.CCed || player.noItems)
                {
                    return;
                }
                Projectile.alpha = 0;
                if (timer == 1)
                {
                    player.GetModPlayer<MyPlayer>().SwordCombo = 0;
                    player.GetModPlayer<MyPlayer>().SwordComboR = 0;
                    Projectile.damage += 9999;
                    Projectile.damage *= 3;

                    timer++;
                }
                Vector3 RGB = new Vector3(1.28f, 0f, 1.28f);
                float multiplier = 0.2f;
                float max = 2.25f;
                float min = 1.0f;
                RGB *= multiplier;
                if (RGB.X > max)
                {
                    multiplier = 0.5f;
                }
                if (RGB.X < min)
                {
                    multiplier = 1.5f;
                }
                Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 30;

                int dir = (int)Projectile.ai[1];
                float swingProgress = Lerp(Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft, true));
                // the actual rotation it should have
                float defRot = Projectile.velocity.ToRotation();
                // starting rotation
                float endSet = ((MathHelper.Pi) * 2 / 0.2f);
                float start = defRot - endSet;

                // ending rotation
                float end = (defRot + endSet);
                // current rotation obv
                float rotation = dir == 1 ? start.AngleLerp(end, swingProgress) : start.AngleLerp(end, 1f - swingProgress);
                // offsetted cuz sword sprite
                Vector2 position = player.RotatedRelativePoint(player.MountedCenter);
                position += rotation.ToRotationVector2() * holdOffset;
                Projectile.Center = position;
                Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;

                player.heldProj = Projectile.whoAmI;
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.itemRotation = rotation * player.direction;
                player.itemTime = 2;
                player.itemAnimation = 2;
                //Projectile.netUpdate = true;

                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Projectile.oldPos[i] += player.velocity;
                }
                if (!ParticleSpawned)
                {


                    ParticleSpawned = true;
                }

            }
        }
        private Vector2 GetSwingPosition(float progress)
        {
            //Starts at owner center, goes to peak range, then returns to owner center
            float distance = MathHelper.Clamp(SwingDistance, THROW_RANGE * 0.1f, THROW_RANGE) * MathHelper.Lerp((float)Math.Sin(progress * MathHelper.Pi), 1, 0.04f);
            distance = Math.Max(distance, 100); //Dont be too close to player

            float angleMaxDeviation = MathHelper.Pi / 1.2f;
            float angleOffset = Owner.direction * (Flip ? -1 : 1) * MathHelper.Lerp(-angleMaxDeviation, angleMaxDeviation, progress); //Moves clockwise if player is facing right, counterclockwise if facing left
            return Projectile.velocity.RotatedBy(angleOffset) * distance;
        }


        public override bool ShouldUpdatePosition() => false;
        public bool bounced = false;


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Timer <= 120)
            {
                Player player = Main.player[Projectile.owner];

                Vector2 oldMouseWorld = Main.MouseWorld;
                if (!bounced)
                {
                    player.velocity = Projectile.DirectionTo(oldMouseWorld) * -10f;
                    bounced = true;
                }

                if (target.lifeMax <= 1000)
                {
                    if (target.life < target.lifeMax / 2)
                    {
                        target.SimpleStrikeNPC(9999, 1, crit: false, 1);
                    }
                }

                

                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 32f);
            }
           

        }


        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 1.5f, completionRatio);
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Turquoise, Color.Transparent, completionRatio) * 0.7f;
        }


        public TrailRenderer SwordSlash;
        public TrailRenderer SwordSlash2;
        public TrailRenderer SwordSlash3;
        public TrailRenderer SwordSlash4;
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();

            var TrailTex = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WhiteTrail").Value;
            var TrailTex2 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WhiteTrail").Value;
            var TrailTex3 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WaterTrail").Value;
            var TrailTex4 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WaterTrail").Value;
            Color color = Color.Multiply(new(1.50f, 1.75f, 3.5f, 0), 200);



            if (SwordSlash == null)
            {
                SwordSlash = new TrailRenderer(TrailTex, TrailRenderer.DefaultPass, (p) => new Vector2(50f), (p) => new Color(110, 255, 150, 90) * (1f - p));
                SwordSlash.drawOffset = Projectile.Size / 1.8f;
            }
            if (SwordSlash2 == null)
            {
                SwordSlash2 = new TrailRenderer(TrailTex2, TrailRenderer.DefaultPass, (p) => new Vector2(90f), (p) => new Color(250, 150, 200, 100) * (1f - p));
                SwordSlash2.drawOffset = Projectile.Size / 1.9f;

            }
            if (SwordSlash3 == null)
            {
                SwordSlash3 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass, (p) => new Vector2(100f), (p) => new Color(10, 250, 25, 90) * (1f - p));
                SwordSlash3.drawOffset = Projectile.Size / 2f;

            }

            if (SwordSlash4 == null)
            {
                SwordSlash4 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass, (p) => new Vector2(90f), (p) => new Color(255, 255, 255, 110) * (1f - p));
                SwordSlash4.drawOffset = Projectile.Size / 2.2f;

            }
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);


            float[] rotation = new float[Projectile.oldRot.Length];
            for (int i = 0; i < rotation.Length; i++)
            {
                rotation[i] = Projectile.oldRot[i] - MathHelper.ToRadians(45);
            }
            SwordSlash.Draw(Projectile.oldPos, rotation);
            SwordSlash2.Draw(Projectile.oldPos, rotation);
            SwordSlash3.Draw(Projectile.oldPos, rotation);
            SwordSlash4.Draw(Projectile.oldPos, rotation);



            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);


            Main.EntitySpriteDraw(texture,
               Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
               sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0); // drawing the sword itself
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture2 = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            Main.spriteBatch.End();

            Main.spriteBatch.Begin();


            return false;

        }

        public override void PostDraw(Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            float mult = Lerp(Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft));
            float alpha = (float)Math.Sin(mult * Math.PI);
            Vector2 pos = player.Center + Projectile.velocity * (mult);

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            float rotation = Projectile.rotation;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);


            // Redraw the projectile with the color not influenced by light
            Vector2 Dorigin = sourceRectangle.Size() / 2f;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + Dorigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(93, 203, 243), new Color(59, 72, 168), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k / 0.2f));
                Main.EntitySpriteDraw(texture, drawPos, null, color, rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(SwingTime);
            writer.Write(SwingDistance);
            writer.WriteVector2(returnPosOffset);
            writer.WriteVector2(npcHookOffset);
            writer.Write(npcHookRotation);
            writer.Write(Flip);
            writer.Write(Slam);
            writer.Write(Curvature);

            if (hookNPC == default(NPC)) //Write a -1 instead if the npc isnt set
                writer.Write(-1);
            else
                writer.Write(hookNPC.whoAmI);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SwingTime = reader.ReadInt32();
            SwingDistance = reader.ReadSingle();
            returnPosOffset = reader.ReadVector2();
            npcHookOffset = reader.ReadVector2();
            npcHookRotation = reader.ReadSingle();
            Flip = reader.ReadBoolean();
            Slam = reader.ReadBoolean();
            Curvature = reader.ReadSingle();

            int whoAmI = reader.ReadInt32(); //Read the whoami value sent
            if (whoAmI == -1) //If its a -1, sync that the npc hasn't been set yet
                hookNPC = default;
            else
                hookNPC = Main.npc[whoAmI];
        }
    }
}