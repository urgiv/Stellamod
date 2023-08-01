﻿
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;

namespace Stellamod.Items.Materials
{

    public class FloralFungus : ModItem
    {

        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("This is a modded Item."); // The (English) text shown below your item's name
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
        }

        public override void SetDefaults()
        {
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height

            Item.maxStack = 999; // The item's max stack value
            Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Ivythorn>(), 2);
            recipe.AddIngredient(ItemID.GlowingMushroom, 1);
            recipe.AddIngredient(ItemID.Mushroom, 1);
            recipe.AddTile(TileID.Bottles);
            recipe.Register();
        }
    }
}
