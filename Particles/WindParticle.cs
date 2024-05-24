﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Particles
{
    internal class WindParticle : Particle
    {
        //Lifetime of the particle
        public int Min_Lifetime => 30;
        public int Max_Lifetime => 120;

        //How quickly it rotates
        public float Rotation_Speed => 0.03f;

        //How quickly the velocity changes
        public float Velocity_Multiplier => 0.98f;

        //Variables
        public float RotationDirection;
        public float StartScale;
        public int LifeTime;

        public override void SetDefaults()
        {
            width = 358;
            height = 358;
            Scale = 1f;
            timeLeft = 120;
            oldPos = new Vector2[4];
            oldRot = new float[4];
            SpawnAction = Spawn;
        }

        public void Spawn()
        {
            ai[1] = Main.rand.NextFloat(2f, 8f) / 1f;
            ai[2] = Main.rand.Next(0, 2);
            ai[3] = Main.rand.NextFloat(0f, 20f);
            //Randomize the rotation direction
            RotationDirection = Main.rand.NextFloatDirection();

            //Randomize the lifetime for added variety
            LifeTime = Main.rand.Next(Min_Lifetime, Max_Lifetime);
            timeLeft = LifeTime;

            StartScale = Scale;
        }

        public override void AI()
        {
            //Slowly decrease its size, using easing
            float timeLeft = base.timeLeft;
            float lifeTime = LifeTime;
            float progress = timeLeft / lifeTime;
            float size = MathHelper.Lerp(StartScale, 0f, Easing.InQuint(1 - progress));
            Vector2 easedScale = new Vector2(size, size);
            scale = easedScale;

            /*
            //Rotate it
            rotation += RotationDirection * Rotation_Speed;
            */
            //Change velocity over time
            velocity *= Velocity_Multiplier;

            if (Scale <= 0f)
                active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Vector2 origin = this.OriginCenter();
            spriteBatch.Draw(texture, screenPos,
                null, Color.White,
                velocity.ToRotation(),
                origin,
                1.35f * scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
