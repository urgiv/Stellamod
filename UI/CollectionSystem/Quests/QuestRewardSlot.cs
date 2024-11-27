﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;

namespace Stellamod.UI.CollectionSystem.Quests
{
    internal class QuestRewardSlot : UIElement
    {
        internal Item Item;
        private readonly int _context;
        private readonly float _scale;
        internal QuestRewardSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;
            Item = new Item();
            Item.SetDefaults(0);

            var value = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}CollectionTabSlot",
                ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(value.Width() * scale, 0f);
            Height.Set(value.Height() * scale, 0f);
            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
        }

        public float Glow { get; set; }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {

        }

        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();

            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            //Draw Backing
            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();
            Texture2D value = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}CollectionTabSlot").Value;
            Vector2 centerPos = pos + rectangle.Size() / 2f;

            spriteBatch.Draw(value, rectangle.TopLeft(), null, color2, 0f, default(Vector2), _scale, SpriteEffects.None, 0f);
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, _scale, 32, Color.White);


            if (contains)
            {
                Main.hoverItemName = Item.Name;
                Main.HoverItem = Item;
            }


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, default, default, default, default, Main.UIScaleMatrix);

            for (int i = 0; i < 8f; i++)
            {
                Color glowColor = Color.White * Glow;
                float progress = (float)i / 8f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 offset = rot.ToRotationVector2() * 8 * Glow;
                ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos + offset, _scale, 32, glowColor);
            }

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
            Main.inventoryScale = oldScale;
        }
    }
}
