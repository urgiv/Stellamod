﻿
using Stellamod.Tiles.Ishtar;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
	public class IshtarBlock : ModItem
	{
		public override void SetStaticDefaults()
		{
			// Tooltip.SetDefault("Super silk!");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

		}

		public override void SetDefaults()
		{
			Item.width = 12;
			Item.height = 12;
			Item.maxStack = Item.CommonMaxStack;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 5;
			Item.useTime = 5;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<IshtarMoss>();
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		
	}
}