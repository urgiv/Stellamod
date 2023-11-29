﻿using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using Stellamod.Projectiles.Summons.VoidMonsters;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Summon
{
	public class XScissor : ModItem
    {
        public override void SetDefaults()
        {
			Item.damage = 100;
			Item.knockBack = 3f;
			Item.mana = 20;
			Item.width = 76;
			Item.height = 80;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.buyPrice(0, 30, 0, 0);
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph");

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;

			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ProjectileType<XScissorMinion>();
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			//Spawn at the mouse cursor position
			position = Main.MouseWorld;

			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			player.UpdateMaxTurrets();
			return false;
		}

        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ItemID.Excalibur, 1)
				.AddIngredient(ModContent.ItemType<MiracleThread>(), 12)
				.AddIngredient(ModContent.ItemType<WanderingFlame>(), 8)
				.AddIngredient(ModContent.ItemType<DarkEssence>(), 4)
				.AddIngredient(ModContent.ItemType<EldritchSoul>(), 4)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
    }

	public class XScissorMinion : ModProjectile
	{
		private int _counter;
		private Projectile _voidRiftProjectile1;
		private Projectile _voidRiftProjectile2;
		private const int Void_Eater_Big_Chance = 12;
		private const int Fire_Range = 768;
		private enum SummonState
        {
			X_Slash_Telegraph,
			X_Slash,
			Void_Rift
        }
		private SummonState _summonState = SummonState.X_Slash_Telegraph;
		//Lower = faster
		private const int Fire_Rate = 49;
		public override void SetStaticDefaults()
		{
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
			// These below are needed for a minion
			// Denotes that this projectile is a pet or minion
			Main.projPet[Projectile.type] = true;

			// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			// Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public sealed override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 28;
			// Makes the minion go through tiles freely
			Projectile.tileCollide = false;

			// These below are needed for a minion weapon
			// Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.friendly = true;

			// Only determines the damage type
			//Projectile.minion = true;
			Projectile.sentry = true;
			Projectile.timeLeft = Terraria.Projectile.SentryLifeTime;

			// Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.minionSlots = 0f;

			// Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.penetrate = -1;
		}

		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles()
		{
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage()
		{
			return false;
		}

		public override void AI()
		{
            switch (_summonState)
            {
				case SummonState.Void_Rift:
					_counter++;
					NPC npcToTarget = NPCHelper.FindClosestNPC(Projectile.position, Fire_Range);
					if (_counter >= Fire_Rate && npcToTarget != null)
					{
						Player owner = Main.player[Projectile.owner];
						int projToFire = ModContent.ProjectileType<VoidEaterMini>();
						if (Main.rand.NextBool(Void_Eater_Big_Chance))
						{
							projToFire = ModContent.ProjectileType<VoidEater>();
						}

						Vector2 velocityToTarget = VectorHelper.VelocityDirectTo(Projectile.position, npcToTarget.position, 5);
						var proj = Projectile.NewProjectileDirect(owner.GetSource_FromThis(), Projectile.position, velocityToTarget,
							projToFire, Projectile.damage, Projectile.knockBack, owner.whoAmI);
						proj.DamageType = DamageClass.Summon;

						//Cool little circle visual
						for (int i = 0; i < 16; i++)
						{
							Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
							Particle p = ParticleManager.NewParticle(Projectile.Center, speed, ParticleManager.NewInstance<VoidParticle>(),
								default(Color), 1 / 3f);
							p.layer = Particle.Layer.BeforeProjectiles;
						}

						//Firing Sound :P
						SoundEngine.PlaySound(SoundID.Item73);
						_counter = 0;
					}
					break;
            }

			Visuals();
		}

		private void SpawnVoidRiftProjectiles()
        {
			Player owner = Main.player[Projectile.owner];
			_voidRiftProjectile1 = Projectile.NewProjectileDirect(owner.GetSource_FromThis(), Projectile.position, Vector2.Zero,
				ModContent.ProjectileType<VoidRift>(), Projectile.damage*2, Projectile.knockBack, owner.whoAmI);
			_voidRiftProjectile1.rotation = MathHelper.ToRadians(-45);
			_voidRiftProjectile1.DamageType = DamageClass.Summon;

			_voidRiftProjectile2 = Projectile.NewProjectileDirect(owner.GetSource_FromThis(), Projectile.position, Vector2.Zero,
				ModContent.ProjectileType<VoidRift>(), Projectile.damage*2, Projectile.knockBack, owner.whoAmI);
			_voidRiftProjectile2.rotation = MathHelper.ToRadians(45);
			_voidRiftProjectile2.DamageType = DamageClass.Summon;
		}

		private void KillVoidRiftProjectiles()
        {
			_voidRiftProjectile1?.Kill();
			_voidRiftProjectile2?.Kill();
		}

		private void Visuals()
		{
            switch (_summonState)
            {
				case SummonState.X_Slash_Telegraph:
					Vector2 offset = new Vector2(-64, -32);
					Particle telegraphPart1 = ParticleManager.NewParticle(Projectile.Center + offset, Vector2.Zero, 
						ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), default(Color), 1f);
					Particle telegraphPart2 = ParticleManager.NewParticle(Projectile.Center + offset, Vector2.Zero, 
						ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), default(Color), 1f);
					telegraphPart1.rotation = MathHelper.ToRadians(-45);
					telegraphPart2.rotation = MathHelper.ToRadians(45);
					_summonState = SummonState.X_Slash;
					break;
				case SummonState.X_Slash:
					_counter++;
					if(_counter > RipperSlashTelegraphParticle.Animation_Length)
					{
						var xSlashPart1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Zero,
							ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, Projectile.owner, 0f, 0f);
						var xSlashPart2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Zero,
							ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, Projectile.owner, 0f, 0f);
						(xSlashPart1.ModProjectile as RipperSlashProjBig).randomRotation = false;
						(xSlashPart2.ModProjectile as RipperSlashProjBig).randomRotation = false;
						xSlashPart2.rotation = MathHelper.ToRadians(90);
						_counter = 0;
						_summonState = SummonState.Void_Rift;
						SpawnVoidRiftProjectiles();
					}
                    break;
                case SummonState.Void_Rift:
					_voidRiftProjectile1.timeLeft = 2;
					_voidRiftProjectile1.Center = Projectile.Center;

					_voidRiftProjectile2.timeLeft = 2;
					_voidRiftProjectile2.Center = Projectile.Center;
					break;
            }

            //It needs to make two of those particles
            //Then have a delay before actually enabling the AI and void rift particle
			Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
		}

        public override void OnKill(int timeLeft)
        {
			KillVoidRiftProjectiles();
		}
    }
}
