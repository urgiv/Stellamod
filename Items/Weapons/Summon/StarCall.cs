﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.StringnNeedles.Alcadiz;
using Stellamod.Projectiles.Summons;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
	public class StarCall : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Starbomber");
			Tooltip.SetDefault("Your summons will explode into big stary bits!" +
				"\nTakes 2 slots to summon" +
				"\nThey will act as temporary summons for big damage");
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.rare = ItemRarityID.Green;
			Item.value = Terraria.Item.sellPrice(0, 5, 80, 0);
			Item.CloneDefaults(ItemID.Arkhalis);
			Item.damage = 90; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			Item.DamageType = DamageClass.Summon;
			Item.mana = 50;
			Item.useTime = 300; // The Item's use time in ticks (60 ticks == 1 second.)
			Item.useAnimation = 300; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true; //so the Item's animation doesn't do damage
			Item.knockBack = 12; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			Item.value = 10000; // how much the Item sells for (measured in copper)
			Item.UseSound = SoundID.DD2_PhantomPhoenixShot; // The sound that this Item plays when used.
			Item.autoReuse = true; // if you can hold click to automatically use it again
			Item.shoot = ModContent.ProjectileType<Starbomb>();
			Item.shootSpeed = 8f; // the speed of the projectile (measured in pixels per frame)
			Item.channel = true;
		}
		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.46f, .07f, .52f);
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);

			// Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			// Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
			return false;
		}
	}
}