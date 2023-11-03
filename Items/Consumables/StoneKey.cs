﻿

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Stellamod.Items.Materials;
using Stellamod.NPCs.Bosses.Jack;
using Stellamod.NPCs.Bosses.INest;
using Stellamod.Utilis;

namespace Stellamod.Items.Consumables
{
    public class StoneKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Electronic Death Remote (EDR)");
            // Tooltip.SetDefault("'that big red button probably will do something you’ll regret... \n Your conscience advises you to press it and see what happens!'");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 28;
            Item.rare = 2;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = 4;
            Item.rare = ItemRarityID.Orange;
        }

      /*  public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<VirulentPlating>(), 30);
            recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 20);
            recipe.AddIngredient(ModContent.ItemType<DreadFoil>(), 15);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }

*/
        

    }
}