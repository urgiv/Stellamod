﻿using Microsoft.Xna.Framework;
using Stellamod.Assets.Biomes;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Grave
{
    public class GraveSeeker : ModNPC
	{
		private float Spawner;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Swampster");
			Main.npcFrameCount[NPC.type] = 6;
		}

		public enum ActionState
		{

			Speed,
			Wait
		}
		// Current state
		public int frameTick;
		// Current state's timer
		public float timer;

		// AI counter
		public int counter;

		public ActionState State = ActionState.Wait;
		public override void SetDefaults()
		{
			NPC.width = 30;
			NPC.height = 20;
			NPC.damage = 30;
			NPC.defense = 8;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.lifeMax = 185;
			NPC.HitSound = SoundID.NPCHit32;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 563f;
			NPC.knockBackResist = .45f;
			NPC.aiStyle = NPCAIStyleID.HoveringFighter;
			AIType = NPCID.Ghost;
		}

		
		int invisibilityTimer;
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 11; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Plantera_Green, 1, -1f, 1, default, .61f);
			}
			

		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.2f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override void AI()
		{
			Spawner++;
			if(Spawner == 1)
			{
                for (int k = 0; k < 32; k++)
                {
					Vector2 velocity = Main.rand.NextVector2CircularEdge(16, 16);
					Dust.NewDustPerfect(NPC.Center, DustID.Plantera_Green, velocity); // 1, -1f, 1, default, .61f);
                }
            }

            timer++;
			NPC.spriteDirection = NPC.direction;
			
			invisibilityTimer++;
			if (invisibilityTimer >= 100)
			{
				Speed();

				for (int k = 0; k < 11; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenMoss, NPC.direction, -1f, 1, default, .61f);
				
				
				invisibilityTimer = 0;
			}

			switch (State)
			{

				case ActionState.Wait:
					counter++;
					Wait();
					break;

				case ActionState.Speed:
					counter++;
					Speed();
					NPC.velocity *= 0.98f;
					break;

				
				default:
					counter++;
					break;
			}
		}


		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Gravestone, 3, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ItemID.QuadBarrelShotgun, 30, 1, 1));
        //   npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Morrowshroom>(), 2, 1, 3));
         //   npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MorrowChestKey>(), 4, 1, 1));
		//	npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OvermorrowWood>(), 1, 1, 5));
		}


		public void Wait()
		{
			timer++;

			if (timer > 50)
			{

				NPC.oldVelocity *= 0.99f;
				
				

			}
			else if (timer == 60)
            {
				State = ActionState.Speed;
				timer = 0;
			}
		}

		public void Speed()
		{
			timer++;

			
			if (timer > 50)
			{
				
				//NPC.velocity.X *= 5f;
				NPC.velocity.Y *= 0.5f;
				for (int k = 0; k < 5; k++)
                {
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenMoss, NPC.direction, -1f, 1, default, .61f);
				}
					




			}

			 if (timer == 100)
            {
				State = ActionState.Wait;
				timer = 0;
			}

		}
	}
}