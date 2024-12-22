﻿using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Projectiles.Safunai.Alcarish;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Safunais
{
    public class Alcarish : BaseSafunaiItem
    {

        public override DamageClass AlternateClass => DamageClass.Generic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 5;
            Item.mana = 0;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "Alcarish", Helpers.LangText.Common("Safunai"))
            {
                OverrideColor = new Color(308, 71, 99)

            };
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Alcarish", "(C) Medium Damage Scaling wind shots On Hit!")
            {
                OverrideColor = new Color(220, 87, 24)

            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 18;
            Item.shootSpeed = 1f;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item116;
            Item.shoot = ModContent.ProjectileType<AlcarishProj>();

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 10;
            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }
    }
}
