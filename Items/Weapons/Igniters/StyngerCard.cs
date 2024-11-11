﻿using Terraria;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class StyngerCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 8;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 19;
        }
    }
}