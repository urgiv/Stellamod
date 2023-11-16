﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Armors.Vanity.Gia;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Items.Placeable;
using Stellamod.Items.Quest.Merena;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Melee.Safunais;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.Items.Weapons.Whips;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.NPCs.Town
{
	// [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
	[AutoloadHead]
	public class Merena : ModNPC
	{
		public int NumberOfTimesTalkedTo = 0;
		public const string ShopName = "Shop";

		public override void SetStaticDefaults()
		{
			// DisplayName automatically assigned from localization files, but the commented line below is the normal approach.
			// DisplayName.SetDefault("Example Person");
			Main.npcFrameCount[Type] = 8; // The amount of frames the NPC has

			


			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
				Direction = 1 // -1 is left and 1 is right. NPCs are drawn facing the left by default but ExamplePerson will be drawn facing the right
							  // Rotation = MathHelper.ToRadians(180) // You can also change the rotation of an NPC. Rotation is measured in radians
							  // If you want to see an example of manually modifying these when the NPC is drawn, see PreDraw
			};


			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			// Set Example Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an example in ExampleMod/Localization/en-US.lang).
	




			; // < Mind the semicolon!
		}

		// Current state


		// Current frame
		public int frameCounter;
		// Current frame's progress
		public int frameTick;
		// Current state's timer
		public float timer;

		// AI counter
		public int counter;
		public override void SetDefaults()
		{
			NPC.townNPC = false; // Sets NPC to be a Town NPC
			NPC.friendly = true; // NPC Will not attack player
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = -1;
			NPC.damage = 90;
			NPC.defense = 42;
			NPC.lifeMax = 200;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;
			NPC.dontTakeDamageFromHostiles = true;


		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.VortexPillar,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement("Magic Magic MAGIC"),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Merena the bewitched sorcerer")
			});
		}

		// The PreDraw hook is useful for drawing things before our sprite is drawn or running code before the sprite is drawn
		// Returning false will allow you to manually draw your NPC
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			// This code slowly rotates the NPC in the bestiary
			// (simply checking NPC.IsABestiaryIconDummy and incrementing NPC.Rotation won't work here as it gets overridden by drawModifiers.Rotation each tick)
			if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
			{
				drawModifiers.Rotation += 0.001f;

				// Replace the existing NPCBestiaryDrawModifiers with our new one with an adjusted rotation
				NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
				NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
			}

			return true;

		}
		public override string GetChat()
		{
			WeightedRandom<string> chat = new WeightedRandom<string>();

			int partyGirl = NPC.FindFirstNPC(NPCID.Clothier);
			if (partyGirl >= 0 && Main.rand.NextBool(4))
			{
				chat.Add(Language.GetTextValue("Funny enough the clothier used to come through here all the time for some of our amazing fabrics", Main.npc[partyGirl].GivenName));
			}
			// These are things that the NPC has a chance of telling you when you talk to it.
			chat.Add(Language.GetTextValue("A long time ago, I used to experiment with Biotech. It went wrong, and teleported pieces of itself all over the world. You might have found some by now."));
			chat.Add(Language.GetTextValue("Maybe if you find some of my lost tech, you could craft my original biotech and retry my experiments!"));
			chat.Add(Language.GetTextValue("The guide seems to know too much for a human being. I feel like he has something else connected to him…"));
			chat.Add(Language.GetTextValue("The merchant keeps telling me about this dangerous eye thing. I could just zap it with my Bio laser."), 5.0);
			chat.Add(Language.GetTextValue("Aimacra seems pretty neat, too bad she's taken"), 0.4);
			chat.Add(Language.GetTextValue("I wouldn't mind hooking up with the Steampunker :)"), 0.1);
			chat.Add(Language.GetTextValue("So many spare parts and materials, bring them all to me!"), 0.1);
			chat.Add(Language.GetTextValue("I can't believe I let it get loose, I've complicated too many things."), 0.1);

			NumberOfTimesTalkedTo++;
			if (NumberOfTimesTalkedTo >= 10)
			{
				//This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
				chat.Add(Language.GetTextValue("Can you go collect some runes for me? I'd love for you to get working."));
			}

			return chat; // chat is implicitly cast to a string.
		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			int num = NPC.life > 0 ? 1 : 5;

			for (int k = 0; k < num; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood);
			}
		}





	
		public override List<string> SetNPCNameList()
		{
			return new List<string>() {
				"Merena the Sorcerer",
				"Merena the Sorcerer"
				
			};
		}




		public override void SetChatButtons(ref string button, ref string button2)
		{ // What the chat buttons are when you open up the chat UI
			button2 = Language.GetTextValue("LegacyInterface.28");
			button = "Merenas Quest";

		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop)
		{
			if (!firstButton)
			{
				// We want 3 different functionalities for chat buttons, so we use HasItem to change button 1 between a shop and upgrade action.

				//if (Main.LocalPlayer.HasItem(ItemID.HiveBackpack))
				//{
				//	SoundEngine.PlaySound(SoundID.Item37); // Reforge/Anvil sound

				//	Main.npcChatText = $"I upgraded your {Lang.GetItemNameValue(ItemID.HiveBackpack)} to a {Lang.GetItemNameValue(ModContent.ItemType<WaspNest>())}";

				//	int hiveBackpackItemIndex = Main.LocalPlayer.FindItem(ItemID.HiveBackpack);
				//	var entitySource = NPC.GetSource_GiftOrReward();

				//	Main.LocalPlayer.inventory[hiveBackpackItemIndex].TurnToAir();
				//	Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<WaspNest>());

				//	return;
				//}

				shop = ShopName;
			}

			if (firstButton)
			{

				Player player = Main.LocalPlayer;
				WeightedRandom<string> chat = new WeightedRandom<string>();



				//-----------------------------------------------------------------------------------------------	

				if (Main.LocalPlayer.HasItem(ModContent.ItemType<KillVerliaC>()))
				{
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound

					Main.npcChatText = $"Oh damn thanks! Next on the list I need you to steal an orb from a village in an underground morrowed village, the orb contains a magic unlike any other. I have no idea how it was manifested but its needed for this tome.";

					int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<KillVerliaC>());
					var entitySource = NPC.GetSource_GiftOrReward();

					Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
					Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<ExploreMorrowedVillage>(), 1);


				}


				if (Main.LocalPlayer.HasItem(ModContent.ItemType<ExploreMorrowedVillageC>()))
				{
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound

					Main.npcChatText = $"Woa, the energy is pouring out of this one with seamless orange stripes! How did you even get your hands on this?? Either way thanks, now I just need 100 dust bags, it helps with the brewery.";

					int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<ExploreMorrowedVillageC>());
					var entitySource = NPC.GetSource_GiftOrReward();

					Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
					Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Give100DustBags>(), 1);


				}

				if (Main.LocalPlayer.HasItem(ModContent.ItemType<Give100DustBagsC>()))
				{
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound

					Main.npcChatText = $"Neat neat, that shouldn't have been too bad for you I think. Next I need some magical paper, there are magical creatures all over the world of hardmode who drop these, most of them being rare and unique creatures, go get em'!";

					int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<Give100DustBagsC>());
					var entitySource = NPC.GetSource_GiftOrReward();

					Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
					Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<MakeMagicPaper>(), 1);


				}

				if (Main.LocalPlayer.HasItem(ModContent.ItemType<MakeMagicPaperC>()))
				{
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound

					Main.npcChatText = $"OHH Great lmao. Ok we have one more thing we need to do. Legend has it an old thief of this Royal Capital stole an extremely special Carian tome, they stay deep underground hidden far away. Even if the rumors arent true I'd love for you to find this scroll, it may take years...";

					int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<MakeMagicPaperC>());
					var entitySource = NPC.GetSource_GiftOrReward();

					Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
					Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<MakeUltimateScroll>(), 1);


				}

				if (Main.LocalPlayer.HasItem(ModContent.ItemType<TomeOfInfiniteSorcery>()))
				{
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound

					Main.npcChatText = $"THANK YOU THANK YOU THANK YOU, omg this is the best day of my life! I never knew this actually existed! Were the rumors true??! dsfjhnbhfribdhs- Nevermind who cares anymore, we can both be the best mages ever! I open my shop to you and here, a token of my graditude. ";

					int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<TomeOfInfiniteSorcery>());
					var entitySource = NPC.GetSource_GiftOrReward();
	
					
					
					
					
					
					
					// Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<MakeUltimateScroll>(), 1);


				}

				if (!Main.LocalPlayer.HasItem(ModContent.ItemType<TomeOfInfiniteSorcery>()) || !Main.LocalPlayer.HasItem(ModContent.ItemType<MakeMagicPaperC>()) || !Main.LocalPlayer.HasItem(ModContent.ItemType<Give100DustBagsC>()) || !Main.LocalPlayer.HasItem(ModContent.ItemType<KillVerliaC>()) || !Main.LocalPlayer.HasItem(ModContent.ItemType<ExploreMorrowedVillageC>()))
				{

					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
					Main.npcChatText = $"Hey you wanna do a quest? I'll open up my expansive magic shop for you if you do.. I have quite some great goodies in store but I want to become the best witch in all of the Lunar Veil, and I want you to help me make a tome, but first I need you to kill Verlia first for me.";

					var entitySource = NPC.GetSource_GiftOrReward();
					Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<KillVerlia>(), 1);





					// Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<MakeUltimateScroll>(), 1);


				}

			}


		}


		public void ResetTimers()
		{
			timer = 0;
			frameCounter = 0;
			frameTick = 0;
		}











		public override void ModifyActiveShop(string shopName, Item[] items)
		{
			foreach (Item item in items)
			{
				// Skip 'air' items and null items.
				if (item == null || item.type == ItemID.None)
				{
					continue;
				}

				// If NPC is shimmered then reduce all prices by 50%.
				if (NPC.IsShimmerVariant)
				{
					int value = item.shopCustomPrice ?? item.value;
					item.shopCustomPrice = value / 2;
				}
			}
		}




		public override void AddShops()
		{


		

				if (Main.LocalPlayer.HasItem(ModContent.ItemType<TomeOfInfiniteSorcery>()))
				{
				var npcShop = new NPCShop(Type, ShopName)

				.Add(new Item(ModContent.ItemType<WickofSorcery>()) { shopCustomPrice = Item.buyPrice(platinum: 1) })
				.Add(new Item(ModContent.ItemType<PearlescentScrap>()) { shopCustomPrice = Item.buyPrice(silver: 50) })
				.Add(new Item(ModContent.ItemType<Hyua>()) { shopCustomPrice = Item.buyPrice(silver: 10) })
				.Add(new Item(ModContent.ItemType<AlcadThrowingCards>()) { shopCustomPrice = Item.buyPrice(silver: 10) })
				.Add(new Item(ModContent.ItemType<AlcaricMush>()) { shopCustomPrice = Item.buyPrice(gold: 2) })
				.Add(new Item(ItemID.Book) { shopCustomPrice = Item.buyPrice(copper: 7) })
				.Add(new Item(ItemID.AbigailsFlower) { shopCustomPrice = Item.buyPrice(gold: 1) });
				npcShop.Register(); // Name of this shop tab

				}

            else
            {
				var npcShop = new NPCShop(Type, ShopName)

				.Add(new Item(ItemID.Book) { shopCustomPrice = Item.buyPrice(copper: 7) });
		
				npcShop.Register(); // Name of this shop tab
			}



		
		}













		//	else if (Main.moonPhase < 4) {
		// shop.item[nextSlot++].SetDefaults(ItemType<ExampleGun>());
		//		shop.item[nextSlot].SetDefaults(ItemType<ExampleBullet>());
		//	}
		//	else if (Main.moonPhase < 6) {
		// shop.item[nextSlot++].SetDefaults(ItemType<ExampleStaff>());
		// 	}
		//
		// 	// todo: Here is an example of how your npc can sell items from other mods.
		// 	// var modSummonersAssociation = ModLoader.TryGetMod("SummonersAssociation");
		// 	// if (ModLoader.TryGetMod("SummonersAssociation", out Mod modSummonersAssociation)) {
		// 	// 	shop.item[nextSlot].SetDefaults(modSummonersAssociation.ItemType("BloodTalisman"));
		// 	// 	nextSlot++;
		// 	// }
		//
		// 	// if (!Main.LocalPlayer.GetModPlayer<ExamplePlayer>().examplePersonGiftReceived && GetInstance<ExampleConfigServer>().ExamplePersonFreeGiftList != null) {
		// 	// 	foreach (var item in GetInstance<ExampleConfigServer>().ExamplePersonFreeGiftList) {
		// 	// 		if (Item.IsUnloaded) continue;
		// 	// 		shop.item[nextSlot].SetDefaults(Item.Type);
		// 	// 		shop.item[nextSlot].shopCustomPrice = 0;
		// 	// 		shop.item[nextSlot].GetGlobalItem<ExampleInstancedGlobalItem>().examplePersonFreeGift = true;
		// 	// 		nextSlot++;
		// 	// 		//TODO: Have tModLoader handle index issues.
		// 	// 	}
		// 	// }
		// }







	}


	
}