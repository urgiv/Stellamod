﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Placeable;
using Stellamod.NPCs.Bosses.StarrVeriplant.Projectiles;
using Stellamod.NPCs.Catacombs;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.StarrVeriplant
{
    [AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head ic
    public class StarrVeriplant : ModNPC
    {
        private enum ActionState
		{
			Idle,
			Stomp,
			Stomp_Multi
		}

		private ActionState State
		{
			get => (ActionState)NPC.ai[0];
			set => NPC.ai[0] = (float)value;
		}

		private ref float Timer => ref NPC.ai[1];
		private ref float AttackCounter => ref NPC.ai[2];
		private ref float StompCounter => ref NPC.ai[3];

		private Player Target => Main.player[NPC.target];
		private bool InPhase2 => NPC.life < NPC.lifeMax / 2;
        private bool HasDoneStomp;
		private bool CanMultiStomp;
        private float StompSpeed = 1;
		private Vector2 StompPos;
	
        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 64;
			NPCID.Sets.TrailCacheLength[Type] = 10;
			NPCID.Sets.TrailingMode[Type] = 1;
			NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCID.Sets.MPAllowedEnemies[NPC.type] = true;

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				CustomTexturePath = "Stellamod/NPCs/Bosses/StarrVeriplant/StarrPreview",
				PortraitScale = 0.8f, // Portrait refers to the full picture when clicking on the icon in the bestiary
				PortraitPositionYOverride = 0f,
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 80;
			NPC.height = 44;
			NPC.damage = 25;
			NPC.defense = 5;
			NPC.lifeMax = 600;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.value = Item.buyPrice(copper: 40);
			NPC.SpawnWithHigherTime(30);
			NPC.boss = true;
			NPC.npcSlots = 10f;

			// Take up open spawn slots, preventing random NPCs from spawning during the fight
			// Don't set immunities like this as of 1.4:
			// NPC.buffImmune[BuffID.Confused] = true;
			// immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

			// Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
			NPC.aiStyle = -1;

            // Custom boss bar
            NPC.BossBar = ModContent.GetInstance<MiniBossBar>();

            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Veriplant");
			}
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
	//		DrawHelper.DrawAdditiveAfterImage(NPC, Color.White, Color.Transparent);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
			writer.WriteVector2(StompPos);
			writer.Write(StompSpeed);
			writer.Write(HasDoneStomp);
			writer.Write(CanMultiStomp);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
			StompPos = reader.ReadVector2();
			StompSpeed = reader.ReadSingle();
			HasDoneStomp = reader.ReadBoolean();
			CanMultiStomp = reader.ReadBoolean();
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// Sets the description of this NPC that is listed in the bestiary
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A beloved magical stone guardian, protected the natural life and would petrify anyone who disturbs it."))
			});
		}

        public override bool? CanFallThroughPlatforms()
        {
			bool isStompState = State == ActionState.Stomp || State == ActionState.Stomp_Multi;
            if (isStompState && NPC.HasValidTarget && Main.player[NPC.target].Top.Y > NPC.Bottom.Y)
            {
                // If Flutter Slime is currently falling, we want it to keep falling through platforms as long as it's above the player
                return true;
            }

            return false;
            // You could also return null here to apply vanilla behavior (which is the same as false for custom AI)
        }

        public override void AI()
        {
            base.AI();
			if (InPhase2)
			{
				float finalStompSpeed = 0.5f;
				//Gradually faster over time, but we limit it so it doesn't get out of hand
				StompSpeed -= 0.001f;
				StompSpeed = MathF.Max(StompSpeed, finalStompSpeed);
			}

			//AI States
			switch (State)
			{
				case ActionState.Idle:
					AI_Idle();
                    break;
				case ActionState.Stomp:
					AI_Stomp();
					break;
				case ActionState.Stomp_Multi:
					AI_MultiStomp();
					break;
			}
		}

		private void AI_Idle()
		{
			//Despawn code
			if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
				if (!NPC.HasValidTarget)
				{
                    NPC.EncourageDespawn(60);
                } 
			}

			//Idle state and then choose attack
			Timer++;
			if(Timer >= 60)
			{
				if (StellaMultiplayer.IsHost)
				{
					//We want him to always use multi stomp the moment he goes into phase 2
					//So we have a bool for that
					bool doStomp = (!HasDoneStomp || (Main.rand.NextBool(3) && CanMultiStomp));
                    if (InPhase2 && doStomp)
                    {
						CanMultiStomp = false;
                        HasDoneStomp = true;
                        SwitchState(ActionState.Stomp_Multi);
                    }
                    else
                    {
                        SwitchState(ActionState.Stomp);
                    }
                }
			}
		}

		private void AI_Stomp()
		{
			switch (AttackCounter)
			{
				case 0:
                    //Target a player
                    Timer++;
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                    }

					//Hover up
					Vector2 targetPos = Target.Center + new Vector2(0, -232);
					Vector2 targetVelocity = (targetPos - NPC.Center).SafeNormalize(Vector2.Zero);
					float distanceToTarget = Vector2.Distance(NPC.Center, targetPos);
					float moveSpeed = MathF.Min(distanceToTarget, 12);
					targetVelocity *= moveSpeed;
					NPC.velocity = targetVelocity;
					if(distanceToTarget <= 2 || Timer > 60)
					{
						Timer = 0;
						AttackCounter++;
                        NPC.velocity.X = 0;
                        NPC.velocity.Y = 0;
                    }
					break;

				case 1:
					//Hover Left/Right
					Timer++;
					Vector2  horizontalVelocity = (Target.Center - NPC.Center);
					float xMove = horizontalVelocity.X;
		
					if(Timer > 60 * StompSpeed)
					{
						NPC.velocity.X *= 0.95f;
					}
					else
					{
                        NPC.velocity.X = xMove;
                    }

					if(Timer >= 100 * StompSpeed)
					{
						NPC.velocity.X = 0;
						Timer = 0;
						AttackCounter++;
					}
                    break;

				case 2:			
					Timer++;
					//Give some initial velocity
					if(Timer == 1)
					{
						NPC.velocity.Y = 1;
					}

					//Calculate Stomp Velocity
					if(Timer < 8)
					{
                        NPC.velocity.Y *= 1.5f;
                    }
		
					if(NPC.collideY)
					{
						MyPlayer myPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
						myPlayer.ShakeAtPosition(NPC.Center, 1024f, 30f);

						for(int i = 0; i < 16; i++)
						{
							float radius = 150;
							Vector2 offset = Vector2.UnitX * Main.rand.Next(-1, 1);
							offset *= Main.rand.NextFloat(1f, radius);
							offset += new Vector2(radius / 2, 0);

                            Vector2 velocity = Vector2.UnitX * Main.rand.Next(-1, 1);
							velocity *= Main.rand.NextFloat(1f, 2f);
                            Dust.NewDustPerfect(NPC.Bottom + offset, ModContent.DustType<Dusts.TSmokeDust>(), velocity, 0, Color.Black * 0.5f,
								Main.rand.NextFloat(0.3f, 0.7f));
                        }
                        //Stomp happens, so the code would be here
                        NPC.velocity.Y = 0;
						Timer = 0;
						AttackCounter++;
					}
					break;
				case 3:
					NPC.velocity.X = 0;
					NPC.velocity.Y = 0;
					CanMultiStomp = true;
					Timer++;
					if(Timer >= 24 * StompSpeed)
					{
                        //Stomp
                        SwitchState(ActionState.Idle);
                    }
					break;
			}
		}

		private void AI_MultiStomp()
		{
			switch (AttackCounter)
			{
				case 0:
                    //Target a player
                    Timer++;
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
						StompPos = NPC.Center + new Vector2(0, -164 - StompCounter * 16);
                    }

                    //Hover up
                    Vector2 targetVelocity = (StompPos - NPC.Center).SafeNormalize(Vector2.Zero);
                    float distanceToTarget = Vector2.Distance(NPC.Center, StompPos);
                    float moveSpeed = MathF.Min(distanceToTarget, 12);
                    targetVelocity *= moveSpeed;
                    NPC.velocity = targetVelocity;
                    if (distanceToTarget <= 2 || Timer > 60)
                    {
                        Timer = 0;
                        AttackCounter++;
                        NPC.velocity.X = 0;
                        NPC.velocity.Y = 0;
                    }
                    break;
				case 1:
                    Timer++;
                    //Give some initial velocity
                    if (Timer == 1)
                    {
                        NPC.velocity.Y = 1;
                    }

                    //Calculate Stomp Velocity
                    if (Timer < 8)
                    {
                        NPC.velocity.Y *= 1.5f;
                    }

                    if (NPC.collideY)
                    {
                        MyPlayer myPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
                        myPlayer.ShakeAtPosition(NPC.Center, 1024f, 30f);

                        for (int i = 0; i < 16; i++)
                        {
                            float radius = 150;
                            Vector2 offset = Vector2.UnitX * Main.rand.Next(-1, 1);
                            offset *= Main.rand.NextFloat(1f, radius);
                            offset += new Vector2(radius / 2, 0);

                            Vector2 velocity = Vector2.UnitX * Main.rand.Next(-1, 1);
                            velocity *= Main.rand.NextFloat(1f, 2f);
                            Dust.NewDustPerfect(NPC.Bottom + offset, ModContent.DustType<Dusts.TSmokeDust>(), velocity, 0, Color.Black * 0.5f,
                                Main.rand.NextFloat(0.3f, 0.7f));
                        }

						if (StellaMultiplayer.IsHost)
						{
                            Vector2 projVelocity = Vector2.UnitX * 10;
							Vector2 spawnOffset = -Vector2.UnitY * 32;
                            int projType = ModContent.ProjectileType<StompShockwave>();
							int projDamage = 10;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom + spawnOffset, projVelocity, projType, projDamage, KnockBack: 1, Main.myPlayer);

							projVelocity = -projVelocity;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom + spawnOffset, projVelocity, projType, projDamage, KnockBack: 1, Main.myPlayer);
                        }

                        //Stomp happens, so the code would be here
                        NPC.velocity.Y = 0;
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;
				case 2:
					StompCounter++;
					if(StompCounter >= 3)
					{
						SwitchState(ActionState.Idle);
					}
					else
					{
						Timer = 0;
						AttackCounter = 0;
					}
					break;
			}
		}

		private void SwitchState(ActionState state)
		{
			State = state;
			Timer = 0;
			AttackCounter = 0;
			StompCounter = 0;
			NPC.netUpdate = true;
		}

        public override void OnKill()
        {
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedStoneGolemBoss, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 10, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StoneKey>(), 1, 1, 1));
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<VeriBossRel>()));
		}
    }
}
