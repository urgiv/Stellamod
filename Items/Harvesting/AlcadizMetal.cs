﻿using Stellamod.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
	internal class AlcadizMetal : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Alcadiz metal");
			/* Tooltip.SetDefault("Woa this is super hard and magical.." +
			"\nWhere does this ore come from?" +
			"\nBest use for metalic weapons and magical items!"); */
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 999;
			Item.value = Item.sellPrice(silver: 20);
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FrileOre>(), 1);
			recipe.AddIngredient(ItemID.DemoniteBar, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}


}
