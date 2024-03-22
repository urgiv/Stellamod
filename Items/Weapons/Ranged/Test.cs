﻿using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.NPCs.Catacombs.Trap.Sparn;
using Stellamod.NPCs.Catacombs.Water.WaterCogwork;
using Stellamod.Projectiles.Chains;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class Test : ModItem
	{
		public override void SetStaticDefaults()
		{
			/* Tooltip.SetDefault("Meatballs" +
				"\nDo not be worried, this mushes reality into bit bits and then shoots it!" +
				"\nYou can never miss :P"); */
			// DisplayName.SetDefault("Teraciz");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 62;
			Item.height = 32;
			Item.scale = 0.9f;
			Item.rare = ItemRarityID.Green;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = true;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Balls");

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 19;
			Item.knockBack = 0;
			Item.noMelee = true;

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<SupernovaChainFront>();
			Item.shootSpeed = 0.01f;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		// This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(2f, -2f);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			for(int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if(Vector2.Distance(player.Center, npc.Center) <= 128)
				{
                    npc.AddBuff(ModContent.BuffType<SupernovaChained>(), 300);
                }
		
			}
			return false;
        }
    }
}