﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
	public class GovheilKaboom : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostShotIN");
			Main.projFrames[Projectile.type] = 16;
		}
		
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.width = 170;
			Projectile.height = 170;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 48;
			Projectile.scale = 2f;
			
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
        public override void AI()
        {
			
			Vector3 RGB = new(0.89f, 2.53f, 2.55f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

		}
		
		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 3)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 16)
				{
					Projectile.frame = 0;
				}
			}
			return true;

			
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(200, 200, 200, 0) * (1f - (float)Projectile.alpha / 50f);
		}

	
	}

}