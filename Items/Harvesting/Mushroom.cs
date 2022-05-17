﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
    internal class Mushroom : ModItem
    {


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mushroom");
            Tooltip.SetDefault("Ew a mushroom" +
            "\nBest use for planting");





        }

        public override void SetDefaults()
        {

            Item.width = 20;
            Item.height = 20;

            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 1);
        }
    }


}
