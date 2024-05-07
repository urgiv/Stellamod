using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Mage
{
    public class TheShrieker : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("The Deafening"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 45;
			Item.DamageType = DamageClass.Magic;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 28;
			Item.useAnimation = 28;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;

			Item.autoReuse = true;
			Item.shoot = ProjectileType<ShriekerProg>();
			Item.shootSpeed = 6f;
			Item.mana = 34;
            Item.noMelee = true;

        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<TheDeafen>(), 1);
            recipe.AddIngredient(ModContent.ItemType<ConvulgingMater>(), 26);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}
	}
}