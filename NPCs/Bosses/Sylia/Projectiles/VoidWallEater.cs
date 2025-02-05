﻿using Microsoft.Xna.Framework;

using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.NPCs.Bosses.Sylia.Projectiles
{
    public class VoidWallEater : ModProjectile
    {
        private int _particleCounter;
        private int _dustCounter;
        private float _projSpeed = 3;
        //AI Values
        private const float Max_Proj_Speed = 5;

        //Visuals
        private const float Body_Radius = 48;
        private const int Body_Particle_Count = 4;
        private const int Kill_Particle_Count = 16;
        private const int Explosion_Particle_Count = 8;

        //Lower number = faster
        private const int Body_Particle_Rate = 2;
        private const int Body_Dust_Rate = 3;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 72;
            Projectile.height = 52;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 450;
            Projectile.penetrate = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.005f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        //Trails
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width*1.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(new Color(60, 0, 118, 125), Color.Transparent, completionRatio) * 0.5f;
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            //Draw The Body
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(60, 0, 118),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.VortexTrail);
            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, ColorFunctions.MiracleVoid, lightColor, 1);
            DrawHelper.DrawAdditiveAfterImage(Projectile, ColorFunctions.MiracleVoid, Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        private void Visuals()
        {
            _particleCounter++;
            if (_particleCounter > Body_Particle_Rate)
            {
                _particleCounter = 0;
            }

            _dustCounter++;
            if(_dustCounter > Body_Dust_Rate)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(Body_Radius / 2, Body_Radius / 2);
                Dust dust = Dust.NewDustPerfect(position, DustID.GemAmethyst, Scale: Main.rand.NextFloat(0.5f, 3f));
                dust.noGravity = true;
                _dustCounter = 0;
            }

            Projectile.scale = VectorHelper.Osc(0.9f, 1f, 5f);
            DrawHelper.AnimateTopToBottom(Projectile, 4);
            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }

        public override void OnKill(int timeLeft)
        {
            //REPLACE SOUND AT SOME POINT
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
        }
    }
}
