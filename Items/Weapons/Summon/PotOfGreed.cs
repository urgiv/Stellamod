﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Summon
{
    public class PotOfGreed : ModItem
    {
		private int _attackStyle;
        public override void SetDefaults()
        {
			Item.damage = 61;
			Item.knockBack = 3f;
			Item.mana = 40;
			Item.width = 54;
			Item.height = 34;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.value = Item.sellPrice(0, 5, 0, 0);
			Item.rare = ItemRarityID.LightPurple;
			
			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;

			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ModContent.ProjectileType<PotOfGreedMinion>();
		}

        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ItemID.CookingPot, 1)
				.AddIngredient(ModContent.ItemType<MiracleThread>(), 20)
				.AddIngredient(ModContent.ItemType<WanderingFlame>(), 8)
				.AddIngredient(ModContent.ItemType<DarkEssence>(), 4)
				.AddIngredient(ModContent.ItemType<EldritchSoul>(), 4)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}

        private void ChangeForm(int newForm)
        {
            _attackStyle = newForm;
            if (_attackStyle == 1)
            {
				Item.UseSound = SoundID.Item3;
				Item.DamageType = DamageClass.Default;
				Item.knockBack = 0;
                Item.mana = 200;
                Item.useStyle = ItemUseStyleID.DrinkLiquid;
				Item.shoot = 0; 
            }
            else if (_attackStyle == 0)
            {
                Item.damage = 100;
                Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph");
                Item.DamageType = DamageClass.Summon;
				Item.knockBack = 3;
                Item.useTime = 36;
                Item.useAnimation = 36;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.noUseGraphic = false;
                Item.mana = 20;
                Item.shoot = ModContent.ProjectileType<PotOfGreedMinion>();
                Item.shootSpeed = 0;
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");

			if(_attackStyle == 0)
			{
                line = new TooltipLine(Mod, "Brooch of the TaGo", "Right click to change form, requires a Sewing Kit")
                {
                    OverrideColor = Color.Magenta
                };
                tooltips.Add(line);
			}
			else
			{
                line = new TooltipLine(Mod, "Brooch of the TaGo", "Changed by Sewing Kit, effects may be incorrect...")
                {
                    OverrideColor = Color.Magenta
                };
                tooltips.Add(line);
            }

        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2 && player.GetModPlayer<SewingKitPlayer>().hasSewingKit)
            {
                if (_attackStyle == 0)
                {
                    ChangeForm(1);
                }
                else
                {
                    ChangeForm(0);
                }

                int sound = Main.rand.Next(0, 3);
                switch (sound)
                {
                    case 0:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia1"), player.position);
                        break;
                    case 1:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia2"), player.position);
                        break;
                    case 2:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia3"), player.position);
                        break;
                }

				return false;
            }

            return base.CanUseItem(player);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Purple);
            if (_attackStyle == 1)
            {
                Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Summon/PotOfGreedMiracle").Value;
                Vector2 size = new Vector2(28, 42);
                Vector2 drawOrigin = size / 2;
                spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, drawOrigin, scale, SpriteEffects.None, 0);
                return false;
            }

            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (!player.GetModPlayer<SewingKitPlayer>().hasSewingKit && _attackStyle == 1)
            {
                ChangeForm(0);
				return true;
            }

            if (_attackStyle == 1 && player.altFunctionUse == 0)
			{
                player.AddBuff(ModContent.BuffType<MiracleLiquid>(), 300);
                return true;
            }

			return base.UseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
   
            //Spawn at the mouse cursor position
            if (player.ownedProjectileCounts[ProjectileType<PotOfGreedMinion>()] > 0)
            {
				//Desummon it
				for(int p = 0; p< Main.maxProjectiles; p++)
                {
					Projectile projectile = Main.projectile[p];
					if (projectile.owner != player.whoAmI)
						continue;

					if(projectile.type == ModContent.ProjectileType<PotOfGreedMinion>() && projectile.active)
                    {
						projectile.Kill();
                    }
                }
            }
			

			position = Main.MouseWorld;
			SoundEngine.PlaySound(SoundID.Item82, player.position);
			Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
			player.UpdateMaxTurrets();
			return false;
		}
    }

	public class PotOfGreedMinion : ModProjectile
	{
		private int _particleCounter;
		private int[] _shadowMinionLifeTime;
		private Projectile[] _shadowMinions;
		private int _newShadowMinionSpawnTimer;
		private float _rotation;

		private const int Max_Shadow_Minion_Count = 7;
		private const int Shadow_Minion_Lifetime = 120;
		private const int Time_Between_Shadow_Minions = 10;
		private const float Shadow_Minion_Summon_Radius = 196;

		//Visuals 
		private const float Body_Radius = 64;
		private const int Body_Particle_Count = 12;

		//Lower number = faster
		private const int Body_Particle_Rate = 2;
		public override void SetStaticDefaults()
		{
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

			// These below are needed for a minion
			// Denotes that this projectile is a pet or minion
			Main.projPet[Projectile.type] = true;

			// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
		}

		public sealed override void SetDefaults()
		{
			Projectile.width = 84;
			Projectile.height = 80;

			// Makes the minion go through tiles freely
			Projectile.tileCollide = false;

			// These below are needed for a minion weapon
			// Only controls if it deals damage to enemies on contact (more on that later)
			//Projectile.friendly = true;

			// Only determines the damage type

			//I DON'T KNOW IF I NEED TO SET minion to true for sentries, I'm not going to
		//	Projectile.minion = true;
			Projectile.sentry = true;
			Projectile.timeLeft = Terraria.Projectile.SentryLifeTime;

			// Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.minionSlots = 0f;

			// Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.penetrate = -1;

			_shadowMinions = new Projectile[Max_Shadow_Minion_Count];
			_shadowMinionLifeTime = new int[Max_Shadow_Minion_Count];
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

		public override bool PreDraw(ref Color lightColor)
		{
			//Void Pre-Draw Effects
			Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
				new Vector3(60, 0, 118),
				new Vector3(117, 1, 187),
				new Vector3(3, 3, 3), 0);

			DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, ColorFunctions.MiracleVoid, lightColor, 1);
			DrawHelper.DrawAdditiveAfterImage(Projectile, ColorFunctions.MiracleVoid, Color.Black, ref lightColor);
			return true;
		}

		private bool CanSpawnShadowMinion()
		{
			Player player = Main.player[Main.myPlayer];
			if (player.Distance(Projectile.position) > Shadow_Minion_Summon_Radius)
				return false;
			for (int i =0;i < _shadowMinions.Length; i++)
            {
				Projectile shadowMinion = _shadowMinions[i];
				if (shadowMinion == null)
					return true;
						
            }
			return false;
        }


		private int GetNextShadowMinionIndex()
        {
			for (int s = 0; s < _shadowMinions.Length; s++)
			{
				if (_shadowMinions[s] == null)
				{
					return s;
				}
			}
			return -1;
		}

		private bool IsShadowMinion(Projectile minion)
        {
			if (minion == Projectile)
				return true;

			for (int s = 0; s < _shadowMinions.Length; s++)
			{
				if (_shadowMinions[s] == minion)
				{
					return true;
				}
			}
			return false;
		}

		private void KillShadowMinions()
        {
			for (int i = 0; i < _shadowMinionLifeTime.Length; i++)
			{
				_shadowMinionLifeTime[i]--;
				Projectile shadowMinion = _shadowMinions[i];
				if (shadowMinion == null)
					continue;

				if (_shadowMinionLifeTime[i] <= 0 && shadowMinion.active)
				{
					int dustCircleCount = 16;
					float degreesPer = 360 / (float)dustCircleCount;
					for (int k = 0; k < dustCircleCount; k++)
					{
						float degrees = k * degreesPer;
						Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
						Vector2 vel = direction * 8;
						Dust.NewDust(shadowMinion.Center, 0, 0, DustID.GemAmethyst, vel.X * 0.5f, vel.Y * 0.5f);
					}

					SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.position);
					shadowMinion.Kill();
					_shadowMinions[i] = null;
				}
			}
		}

		private void ForceKillShadowMinions()
        {
			for (int i = 0; i < _shadowMinionLifeTime.Length; i++)
			{
				_shadowMinionLifeTime[i]--;
				Projectile shadowMinion = _shadowMinions[i];
				if (shadowMinion == null)
					continue;

				if (shadowMinion.active)
				{
					int dustCircleCount = 16;
					float degreesPer = 360 / (float)dustCircleCount;
					for (int k = 0; k < dustCircleCount; k++)
					{
						float degrees = k * degreesPer;
						Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
						Vector2 vel = direction * 8;
						Dust.NewDust(shadowMinion.Center, 0, 0, DustID.GemAmethyst, vel.X * 0.5f, vel.Y * 0.5f);
					}
					SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.position);

					shadowMinion.Kill();
					_shadowMinions[i] = null;
				}
			}
		}

		private void UpdateShadowMinions()
        {
			_newShadowMinionSpawnTimer--;
			if (_newShadowMinionSpawnTimer <= 0 && CanSpawnShadowMinion())
			{
				//Spawn a new shadow minion;
				//Get the type of a minion that this player owns, if none this won't do anything
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile projectileToClone = Main.projectile[i];

					//Only chec stuff that this guy owns
					if (projectileToClone.owner != Projectile.owner)
						continue;
					
					//Only check minions\\\\
					if (!projectileToClone.minion || projectileToClone.sentry)
						continue;

					if (IsShadowMinion(projectileToClone))
						continue;

					//If we're here, then we have the thing that we want	
					Vector2 shadowMinionSpawnPosition = Projectile.Center + new Vector2(0, -32);
					Projectile shadowMinion = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), shadowMinionSpawnPosition, Vector2.Zero,
						projectileToClone.type,	Projectile.damage, projectileToClone.knockBack, Projectile.owner);

					shadowMinion.minionSlots = 0;

					int shadowMinionIndex = GetNextShadowMinionIndex();
					_shadowMinions[shadowMinionIndex] = shadowMinion;
					_shadowMinionLifeTime[shadowMinionIndex] = Shadow_Minion_Lifetime;
					_newShadowMinionSpawnTimer = Time_Between_Shadow_Minions;
					SoundEngine.PlaySound(SoundID.Item117, Projectile.position);

					int dustCircleCount = 48;
					float degreesPer = 360 / (float)dustCircleCount;
					for (int k = 0; k < dustCircleCount; k++)
					{
						float degrees = k * degreesPer;
						Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
						Vector2 vel = direction * 8;
						Dust.NewDust(shadowMinionSpawnPosition, 0, 0, DustID.GemAmethyst, vel.X * 0.5f, vel.Y * 0.5f);
					}

					//Break out of this loop so we don't spawn more on accident
					break;
				}
            }

            Player player = Main.player[Main.myPlayer];
			bool isWithin = player.Distance(Projectile.position) < Shadow_Minion_Summon_Radius;
			if (isWithin)
			{
                for (int i = 0; i < _shadowMinionLifeTime.Length; i++)
                {
                    _shadowMinionLifeTime[i] = Shadow_Minion_Lifetime;
                }
            }
		}


		private void ShadowMinionVisuals()
        {
			for(int i = 0; i < _shadowMinions.Length; i++)
            {
				Projectile shadowMinion = _shadowMinions[i];
				if (shadowMinion == null)
					continue;

				if (Main.rand.NextBool(2))
				{
					int dust = Dust.NewDust(shadowMinion.position, shadowMinion.width, shadowMinion.height, DustID.GemAmethyst);
					Main.dust[dust].scale = 1.5f;
					Main.dust[dust].noGravity = true;
				}
			}
		}

		public override void AI()
		{
			//Actual Effect
			//Is create shadow clones of your minion
			UpdateShadowMinions();
			KillShadowMinions();
			ShadowMinionVisuals();

			//Visuals
			_rotation += 0.05f;
			int count = 4;

			//This is the flame coming out of the pot
			_particleCounter++;
			if (_particleCounter > Body_Particle_Rate)
			{
				for (int i = 0; i < Body_Particle_Count; i++)
				{
					Vector2 position = Projectile.position + Main.rand.NextVector2Circular(Body_Radius / 2, Body_Radius / 2);
					position += new Vector2(Projectile.width / 2, -24);
					Particle p = ParticleManager.NewParticle(position, new Vector2(0, -2f), ParticleManager.NewInstance<VoidParticle>(),
						default(Color), Main.rand.NextFloat(0.5f, 1f));
					p.layer = Particle.Layer.BeforeProjectiles;
				}
				_particleCounter = 0;
			}


			//This is the ring that shows where the shadow minions spawn
			for (int i = 0; i < count; i++)
			{
				Vector2 position = Projectile.Center + new Vector2(Shadow_Minion_Summon_Radius, 0).RotatedBy(((i * MathHelper.PiOver2 / count) + _rotation) * 4);
				ParticleManager.NewParticle(position, new Vector2(0, -0.25f), ParticleManager.NewInstance<VoidParticle>(), default(Color), 1/3f);
			}

			float hoverSpeed = 5;
			float rotationSpeed = 2.5f;
			float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
			float rotation = VectorHelper.Osc(MathHelper.ToRadians(-5), MathHelper.ToRadians(5), rotationSpeed);
			Projectile.velocity = new Vector2(0, yVelocity);
			Projectile.rotation = rotation;
			DrawHelper.AnimateTopToBottom(Projectile, 5);
			Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
		}

        public override void OnKill(int timeLeft)
        {
			ForceKillShadowMinions();
		}
    }
}
