using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Ranged
{
    public class BubbleBlasher : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Western Pistol"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
        public override void SetDefaults()
		{
			Item.damage = 45;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 56;
			Item.height = 56;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 100000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item66;
			Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BBBubble>();
            Item.shopCustomPrice = 23;
			Item.shootSpeed = 5;
			Item.useAmmo = AmmoID.Bullet;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<BBBubble>(), damage, Item.knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

    }
}