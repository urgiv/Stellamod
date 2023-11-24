﻿
using Stellamod.Tiles;
using Stellamod.Tiles.Abyss;
using Stellamod.Tiles.Abyss.Aurelus;
using Stellamod.Tiles.Acid;
using Stellamod.Tiles.Naxtrin;
using Stellamod.Tiles.RoyalCapital;
using System;
using Terraria.ModLoader;

namespace Stellamod
{
    public class BiomeTileCounts : ModSystem
    {
        public int AbyssCount;
        public static bool InAbyss => ModContent.GetInstance<BiomeTileCounts>().AbyssCount > 80;


        public int AcidCount;
        public static bool InAcid => ModContent.GetInstance<BiomeTileCounts>().AcidCount > 80;

        public int AurelusCount;
        public static bool InAurelus => ModContent.GetInstance<BiomeTileCounts>().AurelusCount > 70;

        public int GovheilCount;
        public static bool InGovheil => ModContent.GetInstance<BiomeTileCounts>().GovheilCount > 30;

        public int StarbloomCount;
        public static bool InStarbloom => ModContent.GetInstance<BiomeTileCounts>().StarbloomCount > 20;
      
        public int NaxtrinCount;
        public static bool InNaxtrin => ModContent.GetInstance<BiomeTileCounts>().NaxtrinCount > 10;

        public int RoyalCapitalCount;
        public static bool InRoyalCapital => ModContent.GetInstance<BiomeTileCounts>().RoyalCapitalCount > 10;

        public int VeriCount;
        public static bool InVeri => ModContent.GetInstance<BiomeTileCounts>().VeriCount > 20;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            AcidCount = tileCounts[ModContent.TileType<AcidialDirt>()];
            AbyssCount = tileCounts[ModContent.TileType<AbyssalDirt>()];
            AurelusCount = tileCounts[ModContent.TileType<AurelusTempleBlock>()];
            GovheilCount = tileCounts[ModContent.TileType<GovheilCastleTile>()];
            StarbloomCount = tileCounts[ModContent.TileType<StarbloomTempleBlock>()];
            NaxtrinCount = tileCounts[ModContent.TileType<NoxianBlock>()];
            RoyalCapitalCount = tileCounts[ModContent.TileType<AlcazBlock>()];
            VeriCount = tileCounts[ModContent.TileType<VeriplantDirt>()];
        }
    }
}