﻿using Stellamod.Tiles.Structures.Huntria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable.Huntria
{
	public class CurtainRightI : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("CurtainRight");
			Tooltip.SetDefault("Curtain");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<CurtainRight>());
			Item.value = 150;
			Item.maxStack = 20;
			Item.width = 38;
			Item.height = 24;
		}
	}
}