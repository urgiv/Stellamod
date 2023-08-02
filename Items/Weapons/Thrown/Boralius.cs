
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Stellamod.Projectiles.Thrown;
namespace Stellamod.Items.Weapons.Thrown
{
	public class Boralius : ModItem
	{
        private Vector2 newVect;

        public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Boralius"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.

		}

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 40;
            Item.noUseGraphic = true;
            Item.height = 40;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = 1;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = 2;
            Item.crit = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Boralius2>();
            Item.shootSpeed = 15f;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BorealWood, 10);
            recipe.AddIngredient(ModContent.ItemType<Plantius>(), 1);
            recipe.AddIngredient(ModContent.ItemType<WinterbornShard>(), 5);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}