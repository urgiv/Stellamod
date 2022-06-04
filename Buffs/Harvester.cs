﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Stellamod.Buffs
{
	public class Harvester : ModBuff
	{
		public override void SetStaticDefaults()
		{



			DisplayName.SetDefault("HarvestIT");
			Description.SetDefault("A signifier for harvest npcs!");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;

		}
		public override void Update(Player player, ref int buffIndex)
		{
		
			Dust.NewDustPerfect(new Vector2(player.position.X + Main.rand.Next(player.width), player.position.Y + player.height - Main.rand.Next(7)), DustID.GoldCoin, Vector2.Zero);

		}


	}
}
