﻿using Microsoft.Xna.Framework;
using SpiritMod.Tiles;
using Stellamod.Gores;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Tiles.Ishtar
{
    [TileTag(TileTags.VineSway)]
    public class IshtarVines : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoFail[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            //RegisterItemDrop(ModContent.ItemType<Items.Harvesting.MorrowVine>());

            TileID.Sets.VineThreads[Type] = true;
            TileID.Sets.IsVine[Type] = true;

            HitSound = SoundID.Grass;
            DustType = DustID.WhiteTorch;

            AddMapEntry(new Color(2, 0, 0));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = 2;

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Framing.GetTileSafely(i, j + 1);
            if (tile.HasTile && tile.TileType == Type)
            {
                WorldGen.KillTile(i, j + 1);


                int gore = GoreHelper.TypeFallingIllurianVine;
                var source = new Terraria.DataStructures.EntitySource_TileUpdate(i, j);
                for (int x = 0; x < 2; x++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Vector2 pos = new Vector2(i * 16, j * 16);
                    Gore.NewGore(source, pos, velocity, gore);
                }
            }
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = .030f * 1;
            g = .037f * 1;
            b = .032f * 1;
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            int type = -1;
            if (tileAbove.HasTile && !tileAbove.BottomSlope)
            {
                type = tileAbove.TileType;
            }

            if (type == ModContent.TileType<IshtarMoss>() || type == Type)
            {
                return true;
            }

            WorldGen.KillTile(i, j);
            return true;
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            if (WorldGen.genRand.NextBool(2) && !tileBelow.HasTile)
            {
                bool placeVines = false;
                int yTests = j;
                while (yTests > j - 10)
                {
                    Tile testTile = Framing.GetTileSafely(i, yTests);
                    if (testTile.BottomSlope)
                    {
                        break;
                    }
                    else if (!testTile.HasTile || testTile.TileType != ModContent.TileType<IshtarMoss>())
                    {
                        yTests--;
                        continue;
                    }
                    placeVines = true;
                    break;
                }
                if (placeVines)
                {
                    tileBelow.TileType = Type;
                    tileBelow.HasTile = true;
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                    }
                }
            }
        }



        public float GetOffset(int i, int j, int frameX, float sOffset = 0f)
        {
            float sin = (float)Math.Sin((Main.time + (i * 24) + (j * 19)) * (0.04f * (!Lighting.NotRetro ? 0f : 1)) + sOffset) * 1.4f;
            if (Framing.GetTileSafely(i, j - 1).TileType != Type) //Adjusts the sine wave offset to make it look nicer when closer to ground
                sin *= 0.25f;
            else if (Framing.GetTileSafely(i, j - 2).TileType != Type)
                sin *= 0.5f;
            else if (Framing.GetTileSafely(i, j - 3).TileType != Type)
                sin *= 0.75f;

            return sin;
        }
    }
}