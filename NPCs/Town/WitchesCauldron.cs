﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.CauldronSystem;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.NPCs.Town
{
    internal class WitchesCauldron : ModNPC
    {
        private ref float Timer => ref NPC.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            Main.npcFrameCount[Type] = 30;
        }

        public override void SetDefaults()
        {
            NPC.width = 96;
            NPC.height = 128;
            NPC.aiStyle = -1;
            NPC.damage = 90;
            NPC.defense = 42;
            NPC.lifeMax = 9000;
            NPC.knockBackResist = 0.5f;
            NPC.npcSlots = 0;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.dontTakeDamage = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.noGravity = true;
            NPC.friendly = true; // NPC Will not attack player
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.5f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Witch's Cauldron"
            };
        }

        public override bool CanChat()
        {
            return true;
        }
        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            // These are things that the NPC has a chance of telling you when you talk to it.
            chat.Add(LangText.Chat(this, "Basic1"));
            return chat; // chat is implicitly cast to a string.
        }
        public override bool CheckActive()
        {
            //Don't despawn
            return false;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        { // What the chat buttons are when you open up the chat UI

            button = LangText.Chat(this, "Button");
            CauldronUISystem cauldronUISystem = ModContent.GetInstance<CauldronUISystem>();
            cauldronUISystem.CloseUI();
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            CauldronUISystem cauldronUISystem = ModContent.GetInstance<CauldronUISystem>();
            if (firstButton)
            {
                cauldronUISystem.OpenUI();
                cauldronUISystem.CauldronPos = NPC.Center;
                Main.CloseNPCChatOrSign();
                Main.playerInventory = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            float drawRotation = NPC.rotation;

            SpriteEffects effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            //Ok so we need some glowing huhh
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            Vector2 s = NPC.Size / 2;
            //Trail Code
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Color startColor = new Color(255, 255, 113);
                startColor *= 0.5f;
                Color endColor = new Color(232, 111, 24);
                endColor *= 0.5f;
                Vector2 trailDrawPos = NPC.oldPos[k] - Main.screenPosition + s + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(startColor, endColor, 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(texture, trailDrawPos, NPC.frame, color, NPC.oldRot[k], NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            float yOffset = MathF.Sin(Timer * 0.2f);
            NPC.position += new Vector2(0, yOffset);
            Lighting.AddLight(NPC.position, 1, 1, 1);

            if(Timer % 32 == 0)
            {
                Dust.NewDustPerfect(NPC.Center + new Vector2(Main.rand.NextFloat(0, 16), Main.rand.NextFloat(-32, -16)),
                    ModContent.DustType<Sparkle>(), new Vector2(Main.rand.NextFloat(-0.02f, 0.4f), -Main.rand.NextFloat(0.1f, 2f)), 0, new Color(0.05f, 0.08f, 0.2f, 0f), Main.rand.NextFloat(0.25f, 2f));
            }
        }
    }
}
