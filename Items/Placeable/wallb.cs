﻿using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable
{
    public class wallb : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("wall2");
            // Tooltip.SetDefault("Thingb");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Structures.wall2>());
            Item.value = 150;
            Item.maxStack = 20;
            Item.width = 38;
            Item.height = 24;
        }
    }
}