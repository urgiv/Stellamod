﻿using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Buffs.Dusteffects;
using Stellamod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Powders
{
	public class DirtPowder : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Powdered Sepsis");
			
		}
		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 45;
			Projectile.ignoreWater = true;
		}
        public override void AI()
        {

			Projectile.velocity *= 0.96f;

        }
        public override bool PreAI()
		{
			Projectile.tileCollide = false;
			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Dirt, 0f, 0f);
			Main.dust[dust].scale = 1.4f;


			return true;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{

			target.AddBuff(ModContent.BuffType<Dusted>(), 720);
			target.AddBuff(ModContent.BuffType<GrassBuff>(), 720);
			base.OnHitNPC(target, damage, knockback, crit);
		}
	}
}