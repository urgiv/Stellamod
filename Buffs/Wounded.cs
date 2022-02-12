﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Stellamod.Buffs
{
	public class Wounded : ModBuff
	{
		public override void SetStaticDefaults()
		{



			DisplayName.SetDefault("Wounded");
			Description.SetDefault("'A cut that saps life'");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.lifeRegen -= 6;

			if (Main.rand.NextBool(2))
			{
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Plantera_Green);
				Main.dust[dust].scale = .85f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = true;
			}
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.lifeRegen -= 6;

			if (Main.rand.NextBool(4))
			{
				int dust = Dust.NewDust(player.position, player.width, player.height, DustID.Plantera_Green);
				Main.dust[dust].scale = 1.25f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].noLight = true;
				int dust2 = Dust.NewDust(player.position, player.width, player.height, DustID.CrystalPulse);
				Main.dust[dust].scale = .95f;
				Main.dust[dust2].noGravity = true;
				Main.dust[dust2].noLight = true;
			}
		}
	}
}