﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Animations;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.REK
{
    internal class RekSegment
    {
        public string TexturePath;
        public Texture2D Texture => ModContent.Request<Texture2D>(TexturePath).Value;
        public Texture2D GlowTexture => ModContent.Request<Texture2D>(TexturePath + "_Glow").Value;
        public Color GlowWhiteColor;
        public bool GlowWhite;
        public float GlowTimer;
        public Vector2 Size => Texture.Size();
        public Vector2 Position;
        public Vector2 Center => Position + Size / 2;
        public Vector2 Velocity;
        public float Rotation;
        public float Scale = 1f;

        public RekSegment(NPC npc)
        {
            Position = npc.position;
            Rotation = 0;
            Velocity = Vector2.Zero;
        }
    }

    internal class RekSnake : ModNPC
    {
        //Draw Code
        private string BaseTexturePath => "Stellamod/NPCs/Bosses/GothiviaTheSun/REK/";
        public PrimDrawer TrailDrawer { get; private set; } = null;
        private float SegmentStretch = 0.66f;
        private float ChargeTrailOpacity;
        private bool DrawChargeTrail;

        //Segments
        private RekSegment Head => Segments[0];
        private RekSegment[] Segments;
        private Vector2 HitboxFixer => new Vector2(NPC.width, NPC.height) / 2;

        //AI
        private bool _resetTimers;
        private enum ActionState
        {
            Idle,
            Dash,
            Clappback,
            EyePopout,
            Beamer,
            Ouroboros,
            Crystal
        }

   
        private ActionState State
        {
            get =>      (ActionState) NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        private ref float Timer => ref NPC.ai[1];
        private ref float AttackTimer => ref NPC.ai[2];
        private Player Target => Main.player[NPC.target];
        private IEntitySource EntitySource => NPC.GetSource_FromThis();
        private Vector2 GothiviaPosition;
        private float GothiviaOrbitRadius;


        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[Type] = 32;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        }
        
        public override void SetDefaults()
        {
            NPC.width = 90;
            NPC.height = 90;
            NPC.lifeMax = 126000;
           
            NPC.damage = 300;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.boss = true;

            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Rek");
            }

            //Initialize Segments
            int bodySegments = 4;
            int bodFrontExtraSegments = 7;
            int bodyExtraSegments = 6;
            int tailSegments = 8;
            List<RekSegment> segments = new List<RekSegment>();
            //Set the textures
            
            //Head
            RekSegment segment = new RekSegment(NPC);
            segment.TexturePath = Texture;
            segments.Add(segment);

            //Neck
            segment = new RekSegment(NPC);
            segment.TexturePath = $"{BaseTexturePath}RekNeck";
            segments.Add(segment);

            for(int i = 0; i < bodySegments; i++)
            {
                segment = new RekSegment(NPC);
                segment.TexturePath = $"{BaseTexturePath}RekBody{i + 1}";
                segments.Add(segment);
            }

            for(int i = 0; i < bodFrontExtraSegments; i++)
            {
                segment = new RekSegment(NPC);
                segment.TexturePath = $"{BaseTexturePath}RekBody{Main.rand.Next(2, 4)}";
                segments.Add(segment);
            }

            //Front Tail Segments
            for(int i = 0; i < bodyExtraSegments; i++)
            {
                segment = new RekSegment(NPC);
                string texturePath = $"{BaseTexturePath}RekBody4";
                if (i > bodyExtraSegments / 2)
                {
                    texturePath = $"{BaseTexturePath}RekBody5";
                }

                segment.TexturePath = texturePath;
                segments.Add(segment);
            }

            //Tail Segments
            for(int i = 0; i < tailSegments; i++)
            {
     
                float p = i;
                float progress = p / (float)tailSegments;
                progress = 1 - progress;
                string texturePath = $"{BaseTexturePath}RekBody6";
                if(i > 2)
                {
                    texturePath =  $"{BaseTexturePath}RekBody7";
                }
                segment = new RekSegment(NPC);
                segment.TexturePath = texturePath;
                segment.Scale = Math.Max(0.5f, progress);
                segments.Add(segment);
            }

            segment = new RekSegment(NPC);
            segment.TexturePath = $"{BaseTexturePath}RekTail";
            segments.Add(segment);
            Segments = segments.ToArray();
        }

        private void FinishResetTimers()
        {
            if (_resetTimers)
            {
                Timer = 0;
                AttackTimer = 0;
                _resetTimers = false;
            }
        }

        private void ResetTimers()
        {
            if (StellaMultiplayer.IsHost)
            {
                _resetTimers = true;
                NPC.netUpdate = true;
            }
        }

        private void ResetState(ActionState state)
        {
            State = state;
            ResetTimers();
            if (StellaMultiplayer.IsHost)
            {
                NPC.netUpdate = true;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            GothiviaPosition = NPC.position;
        }

        public override void AI()
        {
            FinishResetTimers();
            switch (State)
            {
                case ActionState.Idle:
                    AI_Idle();
                    break;
                case ActionState.Dash:
                    AI_Dash();
                    break;
                case ActionState.Clappback:
                    AI_Clappback();
                    break;
                case ActionState.EyePopout:
                    AI_EyePopout();
                    break;
                case ActionState.Beamer:
                    AI_Beamer();
                    break;
                case ActionState.Ouroboros:
                    AI_Ouroboros();
                    break;
                case ActionState.Crystal:
                    AI_Crystal();
                    break;
            }
        }

        private void AI_MoveInOrbit()
        {
            //Orbit Around
            Vector2 direction = GothiviaPosition.DirectionTo(NPC.Center);
            direction = direction.RotatedBy(MathHelper.TwoPi / 720);
            Vector2 targetCenter = GothiviaPosition + direction * GothiviaOrbitRadius;
            AI_MoveToward(targetCenter, 4);
        }

        private void AI_MoveToward(Vector2 targetCenter, float speed = 8)
        {
            //chase target
            Vector2 directionToTarget = NPC.Center.DirectionTo(targetCenter);
            float distanceToTarget = Vector2.Distance(NPC.Center, targetCenter);
            if (distanceToTarget < speed)
            {
                speed = distanceToTarget;
            }

            Vector2 targetVelocity = directionToTarget * speed;

            if (NPC.velocity.X < targetVelocity.X)
            {
                NPC.velocity.X++;
                if (NPC.velocity.X >= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }
            else if (NPC.velocity.X > targetVelocity.X)
            {
                NPC.velocity.X--;
                if (NPC.velocity.X <= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }

            if (NPC.velocity.Y < targetVelocity.Y)
            {
                NPC.velocity.Y++;
                if (NPC.velocity.Y >= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
            else if (NPC.velocity.Y > targetVelocity.Y)
            {
                NPC.velocity.Y--;
                if (NPC.velocity.Y <= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
        }

        private void AI_Idle()
        {
            Timer++;
            AI_MoveInOrbit();
            MakeLikeWorm();
            if (Timer >= 60)
            {
                ResetState(ActionState.Dash);
            }
        }

        private void AI_Dash()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/HavocCharge"), NPC.position);

                Vector2 directionToPlayer = NPC.Center.DirectionTo(Target.Center);
                Vector2 targetVelocity = directionToPlayer * 1;
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);

                //Let's start glowing
                Color color;
                switch (AttackTimer)
                {
                    default:
                        color = Color.White;
                        break;
                    case 1:
                        color = Color.Orange;
                        break;
                    case 2:
                        color = Color.Green;
                        break;
                }

                StartSegmentGlow(color);
            }

            //Glowing white
            if(Timer < 60)
            {
                SegmentStretch -= 0.01f;
                NPC.velocity *= 0.98f;
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi/60);
                //Some smoky dusts while charging up
                for (int i = 0; i < Segments.Length; i++)
                {
                    RekSegment segment = Segments[i];
                    if (segment.GlowWhite)
                    {
                        if (Main.rand.NextBool(8))
                        {
                            Vector2 randOffset = Main.rand.NextVector2Circular(32, 32);
                            Vector2 velocity = -(Vector2.UnitY * 5).RotatedByRandom(MathHelper.PiOver4 / 2);
                            Dust.NewDustPerfect(segment.Center + randOffset, ModContent.DustType<TSmokeDust>(), velocity, 0, Color.DarkGray, 0.5f).noGravity = true;
                        }

                        if (Main.rand.NextBool(32))
                        {
                            Dust.NewDustPerfect(segment.Center, ModContent.DustType<GlowDust>(),
                            (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
                        }
                    }
                }

                float glowSpeed = 1 / 60f;
                GlowWhite(glowSpeed);
            }

            if (Timer == 60)
            {
                SegmentStretch = 0.66f;
                float attackProgress = (AttackTimer) / 3;
                float speed = 64 + attackProgress * 48;

                Vector2 directionToPlayer = NPC.Center.DirectionTo(Target.Center);
                Vector2 targetVelocity = directionToPlayer * speed;
                NPC.velocity = targetVelocity;
          


                //Turn on the trail and roar!!!
                DrawChargeTrail = true;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1024f, 64f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekRoar"), NPC.position);
                StopSegmentGlow();
            }

            if(Timer > 60)
            {
                float glowSpeed = 1 / 40f;
                GlowWhite(glowSpeed);
                NPC.velocity *= 0.98f;
       
            }

            if(Timer > 80)
            {
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.PiOver4 / 40f);
            }
            if(Timer >= 100)
            {
                DrawChargeTrail = false;
                Timer = 0;
                AttackTimer++;
            }

            NPC.rotation = NPC.velocity.ToRotation();
            MakeLikeWorm();
            if (AttackTimer >= 3)
            {
                ResetSegmentGlow();
                ResetState(ActionState.Idle);
            }
        }

        private void AI_Clappback()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                NPC.velocity *= 0.98f;

                SegmentStretch = 0.66f;
                StartSegmentGlow(Color.Orange);
            }

            if(Timer < 90)
            {
                AI_MoveToward(Target.Center);
                SegmentStretch = MathHelper.Lerp(SegmentStretch, 3f, 0.1f);
                if (Main.rand.NextBool(2))
                {
                    StartSegmentGlow(Color.Orange);
                }
                else
                {
                    StartSegmentGlow(Color.White);
                }
                GlowWhite(1 / 90f);
            }

            if(Timer > 90)
            {
                SegmentStretch = MathHelper.Lerp(SegmentStretch, 0.66f, 0.1f);
            }

            NPC.rotation = NPC.velocity.ToRotation();
            MakeLikeWorm();
            if (Timer >= 120)
            {
                Timer = 0;
                ResetSegmentGlow();
                ResetState(ActionState.Idle);
            }
        }

        private void AI_EyePopout()
        {

        }

        private void AI_Beamer()
        {

        }

        private void AI_Ouroboros()
        {

        }

        private void AI_Crystal()
        {

        }

        #region Draw Code

        private void GlowWhite(float speed)
        {
            for(int i = 0; i < Segments.Length; i++)
            {
                RekSegment segment = Segments[i];
                if (segment.GlowWhite)
                {
                    segment.GlowTimer += speed;
                    if (segment.GlowTimer >= 1f)
                        segment.GlowTimer = 1f;
                }
                else
                {
                    segment.GlowTimer -= speed;
                    if (segment.GlowTimer <= 0)
                        segment.GlowTimer = 0;
                }
            }
        }

        private void MakeLikeWorm()
        {
            //Segments
            Head.Position = NPC.position;
            Head.Rotation = NPC.rotation;
            MoveSegmentsLikeWorm();
        }

        private void StartSegmentGlow( Color color)
        {
            for(int i = 0; i < Segments.Length; i++)
            {
                StartSegmentGlow(i, color);
            }
        }

        private void StartSegmentGlow(int index, Color color)
        {
            RekSegment segment = Segments[index];
            segment.GlowWhiteColor = color;
            segment.GlowWhite = true;
        }

        private void StopSegmentGlow()
        {
            for(int i = 0; i < Segments.Length; i++)
            {
                StopSegmentGlow(i);
            }
        }

        private void StopSegmentGlow(int index)
        {
            RekSegment segment = Segments[index];
            segment.GlowWhite = false;
        }

        private void ResetSegmentGlow()
        {
            for(int i = 0; i < Segments.Length; i++)
            {
                ResetSegmentGlow(i);
            }
        }

        private void ResetSegmentGlow(int index)
        {
            RekSegment segment = Segments[index];
            segment.GlowWhite = false;
            segment.GlowTimer = 0;
        }

        private void MoveSegmentsLikeWorm()
        {
            for (int i = 1; i < Segments.Length; i++)
            {
                MoveSegmentLikeWorm(i);
            }
        }

        private void MoveSegmentLikeWorm(int index)
        {
            int inFrontIndex = index - 1;
            if (inFrontIndex < 0)
                return;

            ref var segment = ref Segments[index];
            ref var frontSegment = ref Segments[index - 1];

            // Follow behind the segment "in front" of this NPC
            // Use the current NPC.Center to calculate the direction towards the "parent NPC" of this NPC.
            float dirX = frontSegment.Position.X - segment.Position.X;
            float dirY = frontSegment.Position.Y - segment.Position.Y;

            // We then use Atan2 to get a correct rotation towards that parent NPC.
            // Assumes the sprite for the NPC points upward.  You might have to modify this line to properly account for your NPC's orientation
            segment.Rotation = (float)Math.Atan2(dirY, dirX);
            // We also get the length of the direction vector.
            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
            // We calculate a new, correct distance.

            float fixer = 1;
            if (index == Segments.Length - 1)
            {
                fixer /= 1.75f;
            }

            float dist = (length - segment.Size.X * SegmentStretch * fixer) / length;

            float posX = dirX * dist;
            float posY = dirY * dist;

            //reset the velocity
            segment.Velocity = Vector2.Zero;


            // And set this NPCs position accordingly to that of this NPCs parent NPC.
            segment.Position.X += posX;
            segment.Position.Y += posY;
        }


        public float WidthFunctionCharge(float completionRatio)
        {
            return NPC.width * NPC.scale / 0.75f * (1f - completionRatio);
        }

        public Color ColorFunctionCharge(float completionRatio)
        {
            if (!DrawChargeTrail)
            {
                ChargeTrailOpacity -= 0.05f;
                if (ChargeTrailOpacity <= 0)
                    ChargeTrailOpacity = 0;
            }
            else
            {
                ChargeTrailOpacity += 0.05f;
                if (ChargeTrailOpacity >= 1)
                    ChargeTrailOpacity = 1;
            }

            Color color = Color.Lerp(Color.Orange, Color.Red, 0.2f);
            return color * NPC.Opacity * MathF.Pow(Utils.GetLerpValue(0f, 0.1f, completionRatio, true), 3f) * ChargeTrailOpacity * (1f - completionRatio);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunctionCharge, ColorFunctionCharge, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.FadedStreak);
            Vector2 size = new Vector2(90, 90);
            TrailDrawer.DrawPrims(NPC.oldPos, size * 0.5f - screenPos, 155);

            //Draw all the segments
            for (int i = Segments.Length - 1; i > -1; i--)
            {
                RekSegment segment = Segments[i];
                Vector2 drawPosition = segment.Position - screenPos + HitboxFixer;
                float drawRotation = segment.Rotation;
                Vector2 drawOrigin = segment.Size / 2;
                float drawScale = NPC.scale * segment.Scale;
                spriteBatch.Draw(segment.Texture, drawPosition, null, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            for (int i = Segments.Length - 1; i > -1; i--)
            {
                RekSegment segment = Segments[i];
                if (!ModContent.RequestIfExists<Texture2D>(segment.TexturePath + "_Glow", out var asset))
                    continue;

                Vector2 drawPosition = segment.Position - screenPos + HitboxFixer;
                float drawRotation = segment.Rotation;
                Vector2 drawOrigin = segment.Size / 2;
                float drawScale = NPC.scale;

                float osc = VectorHelper.Osc(0, 1);
   
                spriteBatch.Draw(asset.Value, drawPosition, null, drawColor * osc, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                if (segment.GlowTimer > 0 && ModContent.RequestIfExists<Texture2D>(segment.TexturePath + "_White", out var whiteAsset))
                {
                    spriteBatch.Draw(whiteAsset.Value, drawPosition, null, segment.GlowWhiteColor * segment.GlowTimer, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                }
                
                for (float j = 0f; j < 1f; j += 0.25f)
                {
                    float radians = (j + osc) * MathHelper.TwoPi;
                    spriteBatch.Draw(segment.GlowTexture, drawPosition + new Vector2(0f, 8f).RotatedBy(radians) * osc,
                        null, Color.White * osc * 0.3f, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                }
            }
        }
        #endregion
    }
}
