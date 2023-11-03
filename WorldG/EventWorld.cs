﻿
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using System;
using Terraria.IO;
using Stellamod.Tiles;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;
using Stellamod.Helpers;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Ores;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Placeable;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Whips;
using Stellamod.Items.Materials;
using Stellamod.Tiles.Abyss;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Buffs;

namespace Stellamod.WorldG
{


    public class EventWorld : ModSystem
    {
        //Gintzing--------------------------
        public static bool Gintzing;
        public static bool GintzingText;
        public static bool TryForGintze;
        public static bool GintzeDayReset;
        public static int GintzeKills;

        public static bool GintzingBoss;
        //SoulStorm--------------------------
        public static bool SoulStorm;

        //AuroreanStars--------------------------
        public static bool AuroreanSpawn;
        public static bool Aurorean;
        public static bool AuroreanText;



        public static void GintzeWin()
        {
            if (GintzingBoss)
            {
                GintzeDayReset = true;
                Main.NewText("The Gintze army has been defeated!", 34, 121, 100);
                GintzeKills = 0;
                GintzingBoss = false;
            }
        }
        public override void PostUpdateWorld()
        {
 
            Player player = Main.LocalPlayer;
            if (!player.active)
                return;
            MyPlayer CVA = player.GetModPlayer<MyPlayer>();

            //AuroreanStars--------------------------

            if (!Main.dayTime && !Aurorean && !AuroreanSpawn)
            {
                AuroreanSpawn = true;
                if (Main.rand.NextBool(6))
                {
                    Aurorean = true;
                    if (!AuroreanText)
                    {
                        Main.NewText("Aurorean Stars are falling!", 234, 96, 114);
                        AuroreanText = true;
                    }
                }
            }
            if (Main.dayTime && Aurorean)
            {
                
                Aurorean = false;
                if (AuroreanText)
                {
                    Main.NewText("The Aurorean starfall has ended", 234, 96, 114);
                    AuroreanText = false;
                }
            }
            if (Main.dayTime)
            {
                AuroreanSpawn = false;

            }

            if (AuroreanSpawn)
            {

            }
            //------------------------------------------------------------------------------
            if (Gintzing)
            {
       
                if (Main.expertMode)
                {
                    if (GintzeKills >= 80)
                    {
                        GintzingBoss = true;
                        GintzingText = false;
                        Gintzing = false;
                        GintzeKills = 0;
                    }

                }
                else if(Main.masterMode)
                {
                    if (GintzeKills >= 100)
                    {
                        GintzingBoss = true;      
                        GintzingText = false;
                        Gintzing = false;
                        GintzeKills = 0;
                    }
                }
                else
                {
                    if (GintzeKills >= 65)
                    {
                        GintzingBoss = true;
                        GintzingText = false;
                        Gintzing = false;
                        GintzeKills = 0;
                    }
                }


               

                player.AddBuff(ModContent.BuffType<GintzeSeen>(), 100);
            }
            if (!Main.dayTime)
            {
                TryForGintze = false;
                GintzeDayReset = false;
            }
            if (!TryForGintze && Main.dayTime && player.townNPCs >= 2 && !Main.hardMode && !GintzeDayReset && !GintzingBoss && !DownedBossSystem.downedGintzlBoss)
            {
                if (Main.rand.NextBool(1))
                {
                    Gintzing = true;
                    if (!GintzingText)
                    {
                        Main.NewText("The Gintze army is approaching...", 34, 121, 100);
                        GintzingText = true;
                    }
                }
                TryForGintze = true;
            }


            if (!TryForGintze && Main.dayTime && player.townNPCs >= 2 && player.ZoneOverworldHeight && player.ZoneForest && !Main.hardMode && !GintzeDayReset && !GintzingBoss && DownedBossSystem.downedGintzlBoss)
            {
                if (Main.rand.NextBool(40))
                {
                    Gintzing = true;
                    if (!GintzingText)
                    {
                        Main.NewText("The Gintze army is returning for another round...", 34, 121, 100);
                        GintzingText = true;
                    }
                }
                TryForGintze = true;
            }

        }
    }
}



















