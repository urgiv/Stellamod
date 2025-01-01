﻿using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    internal class Scatterbombs : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Assassin's Knife");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 0, 1, 00);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Throwing;
            Item.shoot = ModContent.ProjectileType<ScatterbombP>();
            Item.shootSpeed = 14f;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankOrb>(), material: ModContent.ItemType<Cinderscrap>());
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
    }
}
