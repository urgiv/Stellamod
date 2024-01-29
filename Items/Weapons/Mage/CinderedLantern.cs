﻿using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class CinderedLantern : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fungal Flace");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Green;
            Item.shootSpeed = 15;
            Item.autoReuse = true;
     
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<LampShot>();
            Item.shootSpeed = 10f;
            Item.mana = 8;
            Item.useAnimation = 7;
            Item.useTime = 7;
            Item.consumeAmmoOnLastShotOnly = true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4f, 2f);
        }



        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.Anvils);
            recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<SingulariumBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Cinderscrap>(), 100);
            recipe.AddRecipeGroup(nameof(ItemID.GoldBar), 10);
            recipe.AddRecipeGroup(nameof(ItemID.ShadowScale), 10);
            recipe.Register();
        }
    }
}
