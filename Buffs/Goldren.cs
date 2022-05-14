﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Stellamod.Buffs
{
	public class Goldren : ModBuff
	{
		public override void SetStaticDefaults()
		{



			DisplayName.SetDefault("Charm Buff!");
			Description.SetDefault("10+ Defense and Golden trail oooo :0");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}



		public override void Update(Player player, ref int buffIndex)
		{
			player.statDefense += 10;
			Dust.NewDustPerfect(new Vector2(player.position.X + Main.rand.Next(player.width), player.position.Y + player.height - Main.rand.Next(7)), DustID.GoldCoin, Vector2.Zero);

		}
	}
}