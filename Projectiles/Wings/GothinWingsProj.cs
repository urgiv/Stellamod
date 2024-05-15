﻿using Microsoft.Xna.Framework;
using Stellamod.Items.Accessories.Wings;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Wings
{
    internal class GothinWingsProj : WingDefaultProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 60;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 166;
            Projectile.height = 96;


            FrameSpeed = 10;
            AlwaysAnimate = false;
            WingOffset = new Vector2(40, 0);
            AccessoryItemType = ModContent.ItemType<GothinWings>();
        }
    }
}
