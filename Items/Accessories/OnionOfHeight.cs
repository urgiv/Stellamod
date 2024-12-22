﻿using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class OnionOfHeight : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Book of Wooden Illusion");
            /* Tooltip.SetDefault("Increased Regeneration!" +
				"\n +3% damage" +
				"\n Increases crit strike change by 5% "); */

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(gold: 7);
            Item.rare = ItemRarityID.LightPurple;
            Item.accessory = true;


        }



        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            player.GetModPlayer<MyPlayer>().Onion1 = true;
            player.GetModPlayer<MyPlayer>().OnionDamage = 5;
        }




    }
}