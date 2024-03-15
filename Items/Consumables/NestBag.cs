﻿
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Melee.Spears;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    internal class NestBag : ModItem
    {

        public override void SetStaticDefaults()
        {           
            //Research Counts
            Item.ResearchUnlockCount = 3;

            //Behave like a boss bag, this will make it also show up on the minimap
            ItemID.Sets.BossBag[Type] = true;

            // DisplayName.SetDefault("Treasure Bag");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
        }

        public override void SetDefaults()
        {
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height
            Item.rare = ItemRarityID.Expert;
            Item.maxStack = Item.CommonMaxStack; // The item's max stack value
            Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
            Item.consumable = true;
            Item.expert = true;
        }

        public override bool CanRightClick() //this make so you can right click this item
        {
            return true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<IrradiatedGreatBlade>(), chanceDenominator: 2));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<IrradieagleWrath>(), chanceDenominator: 2));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheIrradiaspear>(), chanceDenominator: 2));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<StaffoftheIrradiaflare>(), chanceDenominator: 2));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<IrradiatedCreeperStaff>(), minimumDropped: 3, maximumDropped: 25));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BrokenTech>(), minimumDropped: 10, maximumDropped: 10));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<UnknownCircuitry>(), minimumDropped: 10, maximumDropped: 10));
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

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(53, 107, 112, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(152, 208, 113, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
