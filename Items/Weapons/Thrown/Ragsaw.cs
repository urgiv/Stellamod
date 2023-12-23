﻿
using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Items.Weapons.Thrown
{
    public class Ragsaw : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("GreyBricks"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Item.damage = 51;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 40;
            Item.noUseGraphic = true;
            Item.height = 40;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.consumable = false;
            Item.crit = 10;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RagsawP>();
            Item.shootSpeed = 15f;
            Item.rare = ItemRarityID.Blue;
        }


    }
}