﻿using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.IgniterEx;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Brooches;
using Stellamod.Helpers;
using Stellamod.Items.Materials.Molds;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class FrostCard : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Generic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 1;
            Item.mana = 0;
        }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostCard Igniter");
			/* Tooltip.SetDefault("Use with a combination of dusts to make spells :)" +
				"\n Use a powder or dust and then use this type of weapon!"); */
		}
		public override void SetDefaults()
		{
			Item.damage = 2;
			Item.mana = 3;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 80;
			Item.useAnimation = 80;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/clickk");
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<IgniterStart>();
			Item.autoReuse = true;
			Item.crit = 50;
			Item.shootSpeed = 20;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

			for (int i = 0; i < Main.npc.Length; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.active && npc.HasBuff<Dusted>())
				{
					Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, velocity, type, damage, knockback, player.whoAmI);
					
				}
				
				
			}
			return base.Shoot(player, source, position, velocity, type, damage, knockback);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FrileOre>(), 21);
			recipe.AddIngredient(ItemID.FallenStar, 3);
			recipe.AddIngredient(ModContent.ItemType<CondensedDirt>(), 5);
			recipe.AddIngredient(ModContent.ItemType<BlankCard>(), 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Player player = Main.player[Main.myPlayer];
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();

			//Check that this item is equipped

			//Check that you have advanced brooches since these don't work without
			if (broochPlayer.hasIgniteron)
			{
				//Give backglow to show that the effect is active
				DrawHelper.DrawAdvancedBroochGlow(Item, spriteBatch, position, new Color(198, 124, 225));
			}
			else
			{
				float sizeLimit = 28;
				//Draw the item icon but gray and transparent to show that the effect is not active
				Main.DrawItemIcon(spriteBatch, Item, position, Color.Gray * 0.8f, sizeLimit);
				return false;
			}


			return true;
		}
	}
}