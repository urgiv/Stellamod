﻿
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Whips;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Whips
{
    public class MorrowWhipI : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Melee;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 4;
            Item.mana = 0;
        }
        public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			// DisplayName.SetDefault("Alcadiz Whip");
			/* Tooltip.SetDefault("Your summons will target focused enemies" +
				"\nSummons will act on a target and do tremendous more damage" +
				"\nBut when you mark an enemy, you lower your defense :(" +
				"\nDon't dieee!"); */
		}

		public override void SetDefaults()
		{
			// Call this method to quickly set some of the properties below.
			//Item.DefaultToWhip(ModContent.ProjectileType<ExampleWhipProjectileAdvanced>(), 20, 2, 4);

			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.damage = 8;
			Item.knockBack = 5;
			Item.rare = ItemRarityID.Blue;
			
			Item.shoot = ModContent.ProjectileType<MorrowWhipProj>();
			Item.shootSpeed = 4;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.UseSound = SoundID.Item152;
			Item.channel = true;

			// This is used for the charging functionality. Remove it if your whip shouldn't be chargeable.
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.value = 1200;
		}
	}
}