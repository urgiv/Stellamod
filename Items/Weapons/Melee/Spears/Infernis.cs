﻿using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Spears;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Spears
{
    internal class Infernis : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Infernis");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 41;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Orange;
            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ModContent.ProjectileType<InfernisProj>();
            Item.shootSpeed = 20f;
            Item.mana = 5;
            Item.useAnimation = 20;
            Item.useTime = 45;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
    }
}
