﻿using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.WorldG;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    internal class STARBLOCKER : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 62;
            Item.consumable = false;
            Item.rare = ItemRarityID.LightPurple;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(gold: 10);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 10;
            Item.useTime = 10;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }


        public override bool? UseItem(Player player)/* tModPorter Suggestion: Return null instead of false */
        {
            if (EventWorld.Aurorean)
            {
                EventWorld.Aurorean = false;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetworkText auroeanStarfallEnded = NetworkText.FromLiteral("The Aurorean Starfall has been blocked! :(");
                    ChatHelper.BroadcastChatMessage(auroeanStarfallEnded, new Color(234, 96, 114));
                }
                else
                {
                    Main.NewText("The Aurorean Starfall has been blocked! :(", 234, 96, 114);
                }
                NetMessage.SendData(MessageID.WorldData);
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<STARCORE>(), 3)
                .AddIngredient(ModContent.ItemType<AuroreanStarI>(), 600)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
