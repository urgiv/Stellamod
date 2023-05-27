﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using static Terraria.ModLoader.ModContent;


namespace Stellamod.Particles
{
	public class GoldRingParticle : Particle
	{
		public override void SetDefaults()
		{
			width = 34;
			height = 34;
			Scale = 40f;
			timeLeft = 400;
			oldPos = new Vector2[10];
			oldRot = new float[1];
			SpawnAction = Spawn;
		}
		public override void AI()
		{
			

			Scale *= 0.99f;

			velocity *= 0.98f;

			rotation += 0.1f;
			if (Scale <= 0f)
				active = false;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
		{
			Texture2D tex = Request<Texture2D>("Stellamod/Particles/ScorchingParticle").Value;
			float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;

			if (alpha < 0f)
				alpha = 0f;

			Color color = Color.Multiply(new(0.5f, 0.5f, 0.5f, 0), alpha);
			spriteBatch.Draw(tex, position - Main.screenPosition, new Rectangle(0, 0, tex.Width, tex.Height), color, MathHelper.ToRadians(ai[0]).AngleLerp(MathHelper.ToRadians((ai[0] * 180f)), (120f - timeLeft) / 120f), new Vector2(tex.Width / 2f, tex.Height / 2f), Scale, SpriteEffects.None, 0f);
			return false;
		}
		public void Spawn()
		{
			ai[1] = Main.rand.NextFloat(4f, 10f) / 1f;
			ai[2] = Main.rand.Next(0, 4);
			ai[3] = Main.rand.NextFloat(0f, 5f);
			timeLeft = (int)ai[4] > 0 ? (int)ai[4] : timeLeft;
		}
	}
}