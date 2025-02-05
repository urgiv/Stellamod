﻿using Microsoft.Xna.Framework;

using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Items.Quest.Merena;
using Stellamod.Items.Weapons.Melee;
using Stellamod.NPCs.Town;
using Stellamod.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class CelestiaWeddingPlayer : ModPlayer
    {
        public bool hasCelestia;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasCelestia = false;
        }
    }

    public class CelestiasWeddingRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Advanced Brooch Knapsack");
            /* Tooltip.SetDefault("Increased Regeneration!" +
				"\n +10% damage" +
				"\n Allows you to equip advanced brooches! (Very useful :P)" +
				"\n Allows the effects of the Hiker's Backpack! "); */

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "ADBPa", "She'll just love you forever I guess.")
            {
                OverrideColor = new Color(220, 87, 24)

            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 0, 90);
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), material: ModContent.ItemType<GrailBar>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CelestiaWeddingPlayer>().hasCelestia = true;
            player.GetModPlayer<MyPlayer>().HikersBSpawn = true;
            player.noKnockback = true;
            player.lavaImmune = true;
            player.GetDamage(DamageClass.Generic) *= 1.12f; // Increase ALL player damage by 100%
            player.GetArmorPenetration(DamageClass.Generic) *= 1.12f; // Increase ALL player damage by 100%

            if (player.ownedProjectileCounts[ModContent.ProjectileType<Celestia>()] == 0)
            {

                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, player.velocity * -1f,
                    ModContent.ProjectileType<Celestia>(), 0, 1f, player.whoAmI);
            }
        }
    }
}