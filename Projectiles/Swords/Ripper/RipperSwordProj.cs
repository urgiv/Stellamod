﻿using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords.Ripper
{
    internal class RipperSwordProj : ModProjectile
    {
        private Vector2 _targetCenter;
        private Vector2 _velocity;
        private const int Freeze = 45;
        private const int Fire = 80; 
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 112;
        }

        private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
        {
            //This code should give quite interesting movement

            //Accelerate to being on top of the player
            float distX = targetCenter.X - Projectile.Center.X;
            if (Projectile.Center.X < targetCenter.X && Projectile.velocity.X < moveSpeed)
            {
                Projectile.velocity.X += accel;
                if (Projectile.velocity.X > distX)
                    Projectile.velocity.X = distX;

            }
            else if (Projectile.Center.X > targetCenter.X && Projectile.velocity.X > -moveSpeed)
            {
                Projectile.velocity.X -= accel;
                if (Projectile.velocity.X < distX)
                    Projectile.velocity.X = distX;
            }

            //Accelerate to being above the player.
            float distY = targetCenter.Y - Projectile.Center.Y;
            if (Projectile.Center.Y < targetCenter.Y && Projectile.velocity.Y < moveSpeed)
            {
                Projectile.velocity.Y += accel;
                if (Projectile.velocity.Y > distY)
                    Projectile.velocity.Y = distY;
            }
            else if (Projectile.Center.Y > targetCenter.Y && Projectile.velocity.Y > -moveSpeed)
            {
                Projectile.velocity.Y -= accel;
                if (Projectile.velocity.Y < distY)
                    Projectile.velocity.Y = distY;
            }
        }

        public override void AI()
        {
            ref float ai_Counter = ref Projectile.ai[0];
            if(ai_Counter == 0)
            {
                float radius = 384;
                _targetCenter = Projectile.Center + new Vector2(
                    Main.rand.NextFloat(-radius, radius),
                    Main.rand.NextFloat(-radius, radius));
            }

            ai_Counter++;
            if(ai_Counter == Fire)
            {
                Projectile.velocity = _velocity;
            } 
            else if (ai_Counter > Freeze)
            {
                float targetRotation = _velocity.ToRotation() + MathHelper.ToRadians(45);
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, targetRotation, 0.4f);
            }
            else if (ai_Counter == Freeze)
            {
                //I made the projectile just move super slow when it spawned, so gotta do this to return to normal speed.
                Projectile.velocity = Vector2.Zero;
                _velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * 45;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit"), Projectile.position);
            }
            else if (ai_Counter < Freeze)
            {
                AI_Movement(_targetCenter, 25, 5);
                Projectile.rotation += ai_Counter * 0.01f;
            }

            Visuals();
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(new Color(60, 0, 118, 175), Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.VortexTrail);
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(60, 0, 118), Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        private void Visuals()
        {
            if (Main.rand.NextBool(20))
            {
                Dust.NewDust(Projectile.Center, 2, 2, DustID.GemAmethyst);
            }

            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }

        public override void OnKill(int timeLeft)
        {
            var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
              ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, Projectile.owner);
            RipperSlashProjBig ripperSlash = proj.ModProjectile as RipperSlashProjBig;
            ripperSlash.randomRotation = false;
            proj.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
            for (int i = 0; i < 16; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, speed, Scale: 3f);
                d.noGravity = true;
            }
        }
    }
}
