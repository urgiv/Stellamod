﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
    //Wall Version
    public class ExampleDecorativeWallItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<ExampleDecorativeWall>();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Tiles/ExampleDecorativeWallItem").Value;
            Vector2 drawOrigin = Vector2.Zero;
            Vector2 drawPosition = position + drawOrigin;
            spriteBatch.Draw(iconTexture, drawPosition, null, drawColor, 0f, drawOrigin, 0.5f, SpriteEffects.None, 0);
        }

    }

    internal class ExampleDecorativeWall : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;
            base.SetStaticDefaults();
            //If you need other static defaults it go here
        }
    }

}
