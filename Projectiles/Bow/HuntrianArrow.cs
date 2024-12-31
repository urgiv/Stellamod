﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


namespace Stellamod.Projectiles.Bow
{
    internal class HuntrianArrow : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heat Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 9;
            Projectile.height = 17;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            AIType = ProjAIStyleID.Arrow;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.OnFire, 180);
        }
        
        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                for (int j = 0; j < 10; j++)
                {
                    Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
                    vector2 += -Utils.RotatedBy(Vector2.UnitY, (j * 3.141591734f / 6f), default(Vector2)) * new Vector2(8f, 16f);
                    vector2 = Utils.RotatedBy(vector2, (Projectile.rotation - 1.57079637f), default(Vector2));
                    int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GoldCoin, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                    Main.dust[num8].scale = 1.3f;
                    Main.dust[num8].noGravity = true;
                    Main.dust[num8].position = Projectile.Center + vector2;
                    Main.dust[num8].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num8].noLight = true;
                    Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
                }
            }

            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }

            Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 1.0f * Main.essScale);
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            for(int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * 1), (int)(45f * 1), (int)(15f * 1), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for(int i = 0; i < 8; i++)
            {
                Vector2 vel = -Projectile.velocity.RotatedByRandom(MathHelper.PiOver4);
                vel *= Main.rand.NextFloat(0.1f, 0.2f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, vel, Scale: 1f);
                d.noGravity = false;
            }

            for (int i = 0; i < 8; i++)
            {
                float progress = (float)i / 8f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 2;
                Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, vel, Scale: 1f);
            }
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.0f;
            return MathHelper.SmoothStep(baseWidth, 0.35f, completionRatio);
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightYellow, Color.Orange, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return false;
        }
    }
}
