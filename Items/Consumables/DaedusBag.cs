﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.DropRules;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Armors.Daeden;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.NPCs.Bosses.DaedusRework;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    public class DaedusBag : ModItem
	{

		public override void SetStaticDefaults()
		{
			//Research Counts
			Item.ResearchUnlockCount = 3;

			//Behave like a boss bag, this will make it also show up on the minimap
			ItemID.Sets.BossBag[Type] = true;

			// ..But this set ensures that dev armor will only be dropped on special world seeds, since that's the behavior of pre-hardmode boss bags.
			ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
		}

		public override void SetDefaults()
		{
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.width = 42;
			Item.height = 32;
			Item.rare = ItemRarityID.Expert;
			Item.expert = true;
		}

		public override bool CanRightClick()
		{
			return true;
		}

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
			int numResults = 5;

			var armorRule = ItemDropRule.Common(ModContent.ItemType<DaedenLegs>(), chanceDenominator: numResults);
			armorRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DaedenChestplate>()));
			armorRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DaedenMask>()));

			itemLoot.Add(ItemDropRule.AlwaysAtleastOneSuccess(
				ItemDropRule.Common(ModContent.ItemType<BearBroochA>(), chanceDenominator: numResults),
				ItemDropRule.Common(ModContent.ItemType<VixedBroochA>(), chanceDenominator: numResults),
				ItemDropRule.Common(ModContent.ItemType<HeatGlider>(), chanceDenominator: numResults),
				ItemDropRule.Common(ModContent.ItemType<DaedussSunSheid>(), chanceDenominator: numResults),
				armorRule
				));


			itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<DaedusR>()));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Plate>(), minimumDropped: 200, maximumDropped: 1300));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcadizScrap>(), minimumDropped: 4, maximumDropped: 55));
		}

		// Below is code for the visuals

		public override Color? GetAlpha(Color lightColor)
		{
			// Makes sure the dropped bag is always visible
			return Color.Lerp(lightColor, Color.White, 0.4f);
		}

		public override void PostUpdate()
		{
			// Spawn some light and dust when dropped in the world
			Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.4f);

			if (Item.timeSinceItemSpawned % 12 == 0)
			{
				Vector2 center = Item.Center + new Vector2(0f, Item.height * -0.1f);

				// This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
				Vector2 direction = Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f);
				float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
				Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);

				Dust dust = Dust.NewDustPerfect(center + direction * distance, DustID.SilverFlame, velocity);
				dust.scale = 0.5f;
				dust.fadeIn = 1.1f;
				dust.noGravity = true;
				dust.noLight = true;
				dust.alpha = 0;
			}
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			// Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
			Texture2D texture = TextureAssets.Item[Item.type].Value;

			Rectangle frame;

			if (Main.itemAnimations[Item.type] != null)
			{
				// In case this item is animated, this picks the correct frame
				frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
			}
			else
			{
				frame = texture.Frame();
			}

			Vector2 frameOrigin = frame.Size() / 2f;
			Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
			Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

			float time = Main.GlobalTimeWrappedHourly;
			float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

			time %= 4f;
			time /= 2f;

			if (time >= 1f)
			{
				time = 2f - time;
			}

			time = time * 0.5f + 0.5f;

			for (float i = 0f; i < 1f; i += 0.25f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			for (float i = 0f; i < 1f; i += 0.34f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			return true;
		}
	}
}