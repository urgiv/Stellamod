﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Guuts
{
    internal class AdditorGuutsEnchantment : BaseEnchantment
    {

        public override void SetDefaults()
        {
            base.SetDefaults();

        }

        private bool Decresed = false;

        public override void AI()
        {
            base.AI();

            //Count up


            //If greater than time then start homing, we'll just swap the movement type of the projectile
            if (!Decresed)
            {
                foreach (var enchantment in MagicProj.Enchantments)
                {
                    //do a thing here

                    enchantment.time += 15;


                }
                Decresed = true;
            }



        }

        public override float GetStaffManaModifier()
        {
            return 1f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<GuutElement>();
        }


        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.GuutGray);
        }
    }
}
