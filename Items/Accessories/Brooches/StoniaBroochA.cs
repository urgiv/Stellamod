﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class StoniaBroochA : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Brooch of The Huntria Morrow");
			/* Tooltip.SetDefault("Simple Brooch!" +
				"\nHeavy increased damage to your arrows" +
				"\n +2 Defense and increased ranged damage" +
				"\n Use the power of the deep dark morrow.."); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "", "");

			line = new TooltipLine(Mod, "Brooch of stio", "Simple Brooch!")
			{
				OverrideColor = new Color(198, 124, 225)

			};
			tooltips.Add(line);



		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 0, 90);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;


		}
		

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<MyPlayer>().BroochStone = true;
			player.GetModPlayer<MyPlayer>().StoneBCooldown--;

		}




	}
}