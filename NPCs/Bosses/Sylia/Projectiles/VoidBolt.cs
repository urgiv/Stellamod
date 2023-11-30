﻿using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia.Projectiles
{
    public class VoidBolt : ModProjectile
    {
        private int _particleCounter;
        private int _dustCounter;
        private float _projSpeed = 0;
        private float _maxProjSpeed;

        //AI Values
        //Visuals
        private const float Max_Proj_Speed_Min = 5.1f;
        private const float Max_Proj_Speed_Max = 6.1f;
        private const int Body_Radius = 4;
        private const int Body_Particle_Count = 1;
        private const int Kill_Particle_Count = 16;
        private const int Explosion_Particle_Count = 8;

        //Lower number = faster
        private const int Body_Particle_Rate = 1;
        private const int Body_Dust_Rate = 9;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 52;
            Projectile.height = 38;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (_maxProjSpeed == 0)
            {
                _maxProjSpeed = Main.rand.NextFloat(Max_Proj_Speed_Min, Max_Proj_Speed_Max);
            }

            //Movement Code
            //Wanna just home into enemies and then explode or something
            //On second thought, maybe have ai similar to charging type minions like optic staff
            //hmmm
            Player playerToHomeTo = Main.player[Main.myPlayer];
            float closestDistance = Vector2.Distance(Projectile.position, playerToHomeTo.position);
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                float distanceToPlayer = Vector2.Distance(Projectile.position, player.position);
                if(distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    playerToHomeTo = player;
                }
            }

            _projSpeed += 0.25f;
            if (_projSpeed > _maxProjSpeed)
            {
                _projSpeed = _maxProjSpeed;
            }

            Projectile.velocity = (playerToHomeTo.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * _projSpeed;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }


        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            //Draw The Body
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(60, 0, 118),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, new Color(60, 0, 118), lightColor, 1);
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(60, 0, 118), Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        private void Visuals()
        {
            _particleCounter++;
            if (_particleCounter > Body_Particle_Rate)
            {
                //Main Body
                for (int i = 0; i < Body_Particle_Count; i++)
                {
                    Vector2 position = Projectile.Center + 
                        new Vector2(Main.rand.Next(0, Body_Radius), Main.rand.Next(0, Body_Radius));
                    position -= new Vector2(16, 28);
                    float size = Main.rand.NextFloat(0.75f, 1f);
                    Particle p = ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<VoidParticle>(),
                        default(Color), size);

                    p.layer = Particle.Layer.BeforeProjectiles;
                    Particle tearParticle = ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<VoidTearParticle>(),
                        default(Color), size + 0.025f);

                    tearParticle.layer = Particle.Layer.BeforePlayersBehindNPCs;
                }

                _particleCounter = 0;
            }

            _dustCounter++;
            if(_dustCounter > Body_Dust_Rate)
            {
                Vector2 position = Projectile.position + Main.rand.NextVector2Circular(Body_Radius / 2, Body_Radius / 2);
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
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact);
            for (int i = 0; i < Explosion_Particle_Count; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1.5f, 1.5f);
                Particle p = ParticleManager.NewParticle(Projectile.Center, speed, ParticleManager.NewInstance<VoidParticle>(),
                    default(Color), Main.rand.NextFloat(0.9f, 1.33f));
            }

            //Just some dusts so it looks nicer when it dies
            for (int i = 0; i < Kill_Particle_Count; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, speed, Scale: 3f);
                d.noGravity = true;
            }
        }
    }
}
