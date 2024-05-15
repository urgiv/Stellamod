﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Stellamod.NPCs.Bosses.Niivi.Projectiles;
using Stellamod.NPCs.Town;
using Stellamod.Particles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi
{
    [AutoloadBossHead]
    internal partial class Niivi : ModNPC
    {
        public enum ActionState
        {
            Roaming,
            Obliterate,
            Sleeping,
            BossFight
        }

        public enum BossActionState
        {
            Idle,
            Frost_Breath,
            Laser_Blast,
            Star_Wrath,
            Charge,
            Thunderstorm,
            Baby_Dragons,
            Swoop_Out,
            PrepareAttack,
            Calm_Down,
            Frost_Breath_V2,
            Laser_Blast_V2,
            Star_Wrath_V2,
            Charge_V2,
            Thunderstorm_V2
        }

        public ActionState State
        {
            get
            {
                return (ActionState)NPC.ai[0];
            }
            set
            {
                NPC.ai[0] = (float)value;
            }
        }


        //Damage Values
        private int P1_LightningDamage => 240;
        private int P1_FrostBreathDamage => 120;
        private int P1_StarWrathDamage => 40;
        private int P1_LaserDamage => 500;


        //AI
        ref float Timer => ref NPC.ai[1];
        ref float AttackTimer => ref NPC.ai[2];
        public BossActionState BossState
        {
            get => (BossActionState)NPC.ai[3];
            set => NPC.ai[3] = (float)value;
        }

        int SleepingTimer;
        BossActionState NextAttack = BossActionState.Frost_Breath;
        int ScaleDamageCounter;
        int AggroDamageCounter;
        bool Spawned;
  
        private Player Target => Main.player[NPC.target];
        private IEntitySource EntitySource => NPC.GetSource_FromThis();
        private float DirectionToTarget
        {
            get
            {
                if (Target.position.X < NPC.position.X)
                    return -1;
                return 1;
            }
        }

        //Phase Switches
        private bool InPhase2 => NPC.life <= (NPC.lifeMax * 0.66f);
        private bool TriggeredPhase2;

        private bool InPhase3 => NPC.life <= (NPC.lifeMax * 0.22f);
        private bool TriggeredPhase3;

        private bool InPhase4 => NPC.life <= (NPC.lifeMax * 0.01f);
        private bool TriggeredPhase4;

        private int AttackCount;
        private int AttackSide;
        private bool DoAttack;
        private bool IsCharging;
        private Vector2 AttackPos;
        private Vector2 ChargeDirection;
        private Vector2 LaserAttackPos;

        public float RoamingDirection = -1f;
        public float RoamingSpeed = 2;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((float)BossState);
            writer.Write((float)NextAttack);
            writer.Write(ScaleDamageCounter);
            writer.Write(AggroDamageCounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BossState = (BossActionState)reader.ReadSingle();
            NextAttack = (BossActionState)reader.ReadSingle();
            ScaleDamageCounter = reader.ReadInt32();
            AggroDamageCounter = reader.ReadInt32();
        }

        public override void SetStaticDefaults()
        {
            //Don't want her to be hit by any debuffs
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.TrailCacheLength[Type] = Total_Segments;
            NPCID.Sets.TrailingMode[Type] = 2;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
            drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/Niivi/NiiviPreview";
            drawModifiers.PortraitScale = 0.8f; // Portrait refers to the full picture when clicking on the icon in the bestiary
            drawModifiers.PortraitPositionYOverride = 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Niivi, The First Dragon")
            });
        }

        public override void SetDefaults()
        {
            //Stats
            NPC.lifeMax = 232000;
            NPC.defense = 90;
            NPC.damage = 240;
            NPC.width = (int)NiiviHeadSize.X;
            NPC.height = (int)NiiviHeadSize.Y;
            //It won't be considered a boss or take up slots until the fight actually starts
            //So the values are like this for now
            NPC.boss = true;
            NPC.npcSlots = 0f;
            NPC.aiStyle = -1;

            //She'll tile collide and have gravity while on the ground, but not while airborne.
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0;

            NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
            TextureAssets.NpcHeadBoss[NPC.GetBossHeadTextureIndex()] = ModContent.Request<Texture2D>(TextureRegistry.EmptyTexture);
        }

        public override bool CheckActive()
        {
            //Return false to prevents despawning.
            //She'll have custom code when she's in her fight phase
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            //Return false for no Contact Damage
            if (BossState == BossActionState.Charge)
                return IsCharging;
            return false;
        }

        private void ResetState(ActionState actionState)
        {
            State = actionState;
            if(State == ActionState.BossFight)
            {
                TextureAssets.NpcHeadBoss[NPC.GetBossHeadTextureIndex()] = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Niivi/Niivi_Head_Boss");
                // Custom boss bar
                NPC.BossBar = ModContent.GetInstance<NiiviBossBar>();
                NPC.boss = true;
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Niivi");
            }
            else
            {
                TextureAssets.NpcHeadBoss[NPC.GetBossHeadTextureIndex()] = ModContent.Request<Texture2D>(TextureRegistry.EmptyTexture);
                Timer = 0;
                AttackTimer = 0;
                AttackCount = 0;
                NextAttack = BossActionState.Frost_Breath;
                Music = -1;

                NPC.boss = false;
                NPC.BossBar = null;
                NPC.netUpdate = true;
            }
        }

        private void ResetState(BossActionState bossActionState)
        {
            BossState = bossActionState;
            Timer = 0;
            AttackTimer = 0;
            AttackCount = 0;
            NPC.netUpdate = true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            int lifeToGiveIllurineScale = NPC.lifeMax / 300;
            int lifeToGiveIllurineScaleInBoss = NPC.lifeMax / 100;
            if (StellaMultiplayer.IsHost)
            {
                AggroDamageCounter += hit.Damage;
                ScaleDamageCounter += hit.Damage;

                int lifeToGive = State == ActionState.BossFight ? lifeToGiveIllurineScaleInBoss : lifeToGiveIllurineScale;
                if (ScaleDamageCounter >= lifeToGive)
                {
                    Vector2 velocity = -Vector2.UnitY;
                    velocity *= Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position, velocity,
                        ModContent.ProjectileType<NiiviScaleProj>(), 0, 1, Main.myPlayer);
                    ScaleDamageCounter = 0;
                }

                if (AggroDamageCounter >= lifeToGiveIllurineScale * 15 && State != ActionState.BossFight)
                {
                    ResetState(ActionState.BossFight);
                    ResetState(BossActionState.Idle);
                    AggroDamageCounter = 0;
                }
            }
        }

        public override void AI()
        {
            if (!Spawned)
            {
                NPC.boss = false;
                Spawned = true;
            }

            switch (State)
            {
                case ActionState.Roaming:
                    AIRoaming();
                    break;
                case ActionState.Sleeping:
                    AISleeping();
                    break;
                case ActionState.BossFight:
                    AIBossFight();
                    break; 
            }
        }

        private void AI_PhaseSwaps()
        {
            //Trigger Phase 2
            if (InPhase2 && !TriggeredPhase2)
            {
                AI_Phase2_Reset();
                TriggeredPhase2 = true;
                return;
            }

            if (InPhase3 && !TriggeredPhase3)
            {
                AI_Phase3_Reset();
                TriggeredPhase3 = true;
                return;
            }
        }

        private void AIRoaming()
        {
            if (!Main.dayTime)
            {
                AIRoaming_GoHome();
            }
            else
            {
                AIRoaming_FlyAroundTree();
            }
        }

        private void AIRoaming_FlyAroundTree()
        {
            float orbitDistance = 2000;
            Vector2 home = AlcadSpawnSystem.NiiviSpawnWorld + new Vector2(0, 1024);
            Vector2 direction = home.DirectionTo(NPC.Center);
            direction = direction.RotatedBy(MathHelper.TwoPi / 2000);
            Vector2 targetCenter = home + direction * orbitDistance;
            Vector2 directionToTargetCenter = NPC.Center.DirectionTo(targetCenter);
            AI_MoveToward(targetCenter, 2);
            OrientArching();
            if (directionToTargetCenter.X > 0)
            {
                FlightDirection = 1;
                LookDirection = 1;
                StartSegmentDirection = -Vector2.UnitX;

            }
            else
            {
                FlightDirection = -1;
                LookDirection = -1;
                StartSegmentDirection = Vector2.UnitX;
                TargetHeadRotation = -MathHelper.PiOver4 + MathHelper.Pi;
            }

            UpdateOrientation();
            LookAtTarget();
        }

        private void AIRoaming_GoHome()
        {
            Vector2 home = AlcadSpawnSystem.NiiviSpawnWorld;
            Vector2 directionToHome = NPC.Center.DirectionTo(home);
            float distanceToHome = Vector2.Distance(NPC.Center, home);

            //Set orientation
            if (directionToHome.X > 0)
            {
                FlightDirection = 1;
                LookDirection = 1;
                StartSegmentDirection = -Vector2.UnitX;
                OrientStraight();
                TargetHeadRotation = NPC.velocity.ToRotation();

            }
            else
            {
                FlightDirection = -1;
                LookDirection = -1;
                StartSegmentDirection = Vector2.UnitX;
                OrientStraight();
                TargetHeadRotation = NPC.velocity.ToRotation();
            }


            float speed = MathHelper.Min(RoamingSpeed, distanceToHome);
            Vector2 targetVelocity = directionToHome * speed;
            targetVelocity += new Vector2(0, VectorHelper.Osc(-1, 1));
            NPC.velocity = targetVelocity;

            if (distanceToHome <= 1)
            {
                ResetState(ActionState.Sleeping);
            }

            UpdateOrientation();
            LookAtTarget();
        }
 
        private void AISleeping()
        {
            if (Main.dayTime)
            {
                SleepingTimer = 0;
                ResetState(ActionState.Roaming);
            }
            else
            {
                FlightDirection = 1;
                LookDirection = 1;
                StartSegmentDirection = -Vector2.UnitX;

                //Go sleep
                Vector2 sleepPos = AlcadSpawnSystem.NiiviSpawnWorld + new Vector2(0, 164);
                NPC.Center = Vector2.Lerp(NPC.Center, sleepPos, 0.01f);
                NPC.velocity *= 0.9f;
                TargetSegmentRotation = -MathHelper.PiOver4 / 80;
                TargetHeadRotation = 0;
                SleepingTimer++;
                if (SleepingTimer > 60 && SleepingTimer % 60 == 0)
                {
                    ParticleManager.NewParticle<ZeeParticle>(NPC.Center + new Vector2(64, -32), -Vector2.UnitY, Color.White, 1f);
                }
            }
            UpdateOrientation();
        }


        private void AIBossFight()
        {
            AI_PhaseSwaps();
            switch (BossState)
            {
                case BossActionState.Idle:
                    AI_Idle();
                    break;
                  
                case BossActionState.Swoop_Out:
                    AI_SwoopOut();
                    break;
                case BossActionState.PrepareAttack:
                    AI_PrepareAttack();
                    break;
                case BossActionState.Frost_Breath:
                    AI_FrostBreath();
                    break;
                case BossActionState.Laser_Blast:
                    AI_LaserBlast();
                    break;
                case BossActionState.Star_Wrath:
                    AI_StarWrath();
                    break;
                case BossActionState.Charge:
                    AI_Charge();
                    break;
                case BossActionState.Thunderstorm:
                    AI_Thunderstorm();
                    break;
                case BossActionState.Baby_Dragons:
                    AI_BabyDragons();
                    break;
                case BossActionState.Calm_Down:
                    AI_CalmDown();
                    break;

                //Phase 2
                case BossActionState.Frost_Breath_V2:
                    AI_FrostBreath_V2();
                    break;
                case BossActionState.Thunderstorm_V2:
                    AI_Thunderstorm_V2();
                    break;
                case BossActionState.Laser_Blast_V2:
                    AI_LaserBlast_V2();
                    break;
                case BossActionState.Star_Wrath_V2:
                    AI_StarWrath_V2();
                    break;
                case BossActionState.Charge_V2:
                    AI_Charge_V2();
                    break;
            }

            UpdateOrientation();
        }

        private void ChargeVisuals<T>(float timer, float maxTimer) where T : Particle
        {
            float progress = timer / maxTimer;
            float minParticleSpawnSpeed = 8;
            float maxParticleSpawnSpeed = 2;
            int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
            if (timer % particleSpawnSpeed == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(168, 168);
                    Vector2 vel = (NPC.Center - pos).SafeNormalize(Vector2.Zero) * 4;
                    ParticleManager.NewParticle<T>(pos, vel, Color.White, 1f);
                }
            }
        }

        #region Phase 1
        private void AI_Idle()
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                //Despawn basically
                ResetState(BossActionState.Calm_Down);
            }
            Timer++;
            if (Timer >= 1)
            {
                ResetState(BossActionState.PrepareAttack);
            }

            UpdateOrientation();
            NPC.velocity *= 0.98f;
        }


        private void AI_CalmDown()
        {
            Timer++;
            if (Timer >= 60)
            {
                ResetState(ActionState.Roaming);
                ResetState(BossActionState.Idle);
            }

            UpdateOrientation();
            NPC.velocity *= 0.98f;
        }

        private void AI_FrostBreath()
        {

            /*
            //Ok so how is this attack going to work
            Step 1: Niivi takes aim at you for a few seconds, then rotates her head 135 degrees upward
            
            Step 2: Snowflake and snow particles circle around into her magic sigil thing
            
            Step 3: Then they form a frosty rune/sigil thingy
            
            Step 4: A second or two later, Niivi starts breathing the ice breath while slowly rotating her head
            
            Step 5: The attack goes 180 degrees, or a little more, so you'll need to move behind her
            
            Step 6: The breath spews out a windy looking projectile that quickly expands and dissipates
            
            Step 7: Stars and snowflake particles also come out
            
            Step 8: The screen is tinted blue/white during this attack (shader time!)
            
            Step 9: When the breath collides with tiles (including platforms if possible), 
                there is a chance for it to form large icicles, these are NPCs, you can break them
                they have a snowy aura

            Step 10: Niivi stops breathing once she reaches the edge of her range,
                she turns around towards you and fires three frost blasts while slowly flying away

            Step 11: The frost blasts travel a short distance, (slowing down over time)
                when they come to complete stop, they explode into icicles that are affected by gravity

            Step 12: Niivi flies away to decide a new attack
           
            In Phase 2:
                Niivi does frost balls before doing the breath, and rotates her head slightly faster
            */


            if (AttackTimer == 0)
            {
                //Taking aim
                Timer++;

                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
                if (Timer >= 60)
                {
                    NPC.velocity = -Vector2.UnitY;
                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 1)
            {
                //Rotate head 90-ish degrees upward
                Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);

                float targetRotation = MathHelper.PiOver2 * -AttackSide;
                Vector2 rotatedDirection = directionToTarget.RotatedBy(targetRotation);
                TargetHeadRotation = rotatedDirection.ToRotation();

                //Slowly accelerate up while charging
                NPC.velocity *= 1.002f;

                Timer++;
                if (Timer == 1)
                {
                    Vector2 velocity = Vector2.Zero;
                    int type = ModContent.ProjectileType<NiiviFrostTelegraphProj>();
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, NPC.Center, velocity, type,
                        0, 0, Main.myPlayer);
                    }
                }

                //Charge up
                ChargeVisuals<SnowFlakeParticle>(Timer, 60);
                if (Timer >= 60)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        int type = ModContent.ProjectileType<NiiviFrostCircleProj>();
                        int damage = 0;
                        int knockback = 0;
                        Projectile.NewProjectile(EntitySource, NPC.Center, Vector2.Zero,
                            type, damage, knockback, Main.myPlayer);
                    }

                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 2)
            {
                //Get the shader system!
                ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                shaderSystem.TintScreen(Color.Cyan, 0.1f);
                shaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.001f, 0.001f), blend: 0.025f);
                shaderSystem.VignetteScreen(-1f);

                //Slowdown over time
                NPC.velocity *= 0.99f;

                //Slowly rotate while shooting projectile
                float length = 720;
                float amountToRotateBy = 3 * MathHelper.TwoPi / length;
                amountToRotateBy = amountToRotateBy * AttackSide;
                TargetHeadRotation += amountToRotateBy;
                NPC.rotation = HeadRotation;

                Timer++;
                if (Timer % 4 == 0)
                {
                    float particleSpeed = 16;
                    Vector2 velocity = TargetHeadRotation.ToRotationVector2() * particleSpeed;
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 8);

                    Color[] colors = new Color[] { Color.Cyan, Color.LightCyan, Color.DarkCyan, Color.White };
                    Color color = colors[Main.rand.Next(0, colors.Length)];

                    //Spawn Star and Snowflake Particles
                    for(int i = 0; i < 4; i++)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            //Snowflake particle
                            ParticleManager.NewParticle<SnowFlakeParticle>(NPC.Center, velocity, color, 1f);
                        }
                        else
                        {
                            //Star particle
                            ParticleManager.NewParticle<StarParticle2>(NPC.Center, velocity, color, 1f);
                        }
                    }

                }

                if (Timer % 4 == 0 && StellaMultiplayer.IsHost)
                {
                    float speed = 16;
                    Vector2 velocity = TargetHeadRotation.ToRotationVector2() * speed;

                    //Add some random offset to the attack
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 8);

                    int type = ModContent.ProjectileType<NiiviFrostBreathProj>();
                    int damage = P1_FrostBreathDamage;
                    float knockback = 1;

                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, NPC.Center, velocity, type,
                        damage, knockback, Main.myPlayer);
                    }
                }

                if (Timer >= length)
                {

                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 3)
            {
                //Untint the screen
                ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                shaderSystem.UnTintScreen();
                shaderSystem.UnDistortScreen();
                shaderSystem.UnVignetteScreen();

                Timer++;
                if (Timer == 1)
                {
                    //Retarget, just incase target died ya know
                    NPC.TargetClosest();

                    //Re-orient incase target went behind
                    LookDirection = DirectionToTarget;
                    OrientArching();
                    FlipToDirection();

                    Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                    TargetHeadRotation = directionToTarget.ToRotation();
                }

                if (Timer >= 30)
                {
                    float speed = 24;
                    Vector2 velocity = TargetHeadRotation.ToRotationVector2() * speed;

                    //Add some random offset to the attack
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 8);

                    int type = ModContent.ProjectileType<NiiviFrostBombProj>();

                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(128, 128);
                    velocity *= Main.rand.NextFloat(0.5f, 1f);

                    int damage = P1_FrostBreathDamage / 2;
                    float knockback = 1;

                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, spawnPos, velocity, type,
                        damage, knockback, Main.myPlayer);
                    }

                    Timer = 0;
                    AttackCount++;
                }

                if (AttackCount >= 6)
                {
                    Timer = 0;
                    AttackCount = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 4)
            {
                NextAttack = BossActionState.Laser_Blast;
                ResetState(BossActionState.Swoop_Out);
            }
        }

        private void AI_SwoopOut()
        {
            Timer++;
            if (Timer == 1)
            {
                OrientationSpeed = 0.03f;
                NPC.velocity = -Vector2.UnitY * 0.02f;
                // TargetHeadRotation = NPC.velocity.ToRotation();
            }

            NPC.velocity *= 1.016f;
            NPC.velocity.Y -= 0.002f;
            if (Timer >= 30)
            {
                ResetState(BossActionState.Idle);
            }
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

        private void AI_PrepareAttack()
        {
            Timer++;
            if (Timer == 1)
            {
                DoAttack = false;

                //Initialize Attack
                NPC.TargetClosest();
                LookDirection = DirectionToTarget;
                OrientArching();
                FlipToDirection();

                if (NPC.position.X > Target.position.X)
                {
                    AttackSide = 1;
                }
                else
                {
                    AttackSide = -1;
                }

                //Values
                float offsetDistance = 384;
                float hoverDistance = 90;

                //Get the direction
                Vector2 targetCenter = Target.Center + (AttackSide * Vector2.UnitX * offsetDistance) + new Vector2(0, -hoverDistance);
                AttackPos = targetCenter;
            }

            //Rotate Head
            TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();

            if (AttackTimer == 0)
            {
                AI_MoveToward(AttackPos);
                if (Timer >= 360 || Vector2.Distance(NPC.Center, AttackPos) <= 8)
                {
                    DoAttack = true;
                }
            }

            if (DoAttack)
            {
                AttackTimer++;
                NPC.velocity *= 0.98f;
                if (AttackTimer >= 3)
                {
                    ResetState(NextAttack);
                }
            }
        }

        private void AI_LaserBlast()
        {
            /*
             * Step 1: Look at target for a bit
             * 
             * Step 2: Charge begin charging massive laser, white particles come in and slowly build up
             * 
             * Step 3: Fire the laser, twice?, Nah maybe three times
             */

            if (AttackTimer == 0)
            {
                Timer++;

                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
                if (Timer >= 60)
                {
                    NPC.velocity = -Vector2.UnitY;
                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 1)
            {
                Timer++;


                float progress = Timer / 60;
                progress = MathHelper.Clamp(progress, 0, 1);
                float sparkleSize = MathHelper.Lerp(0f, 4f, progress);
                if (Timer % 4 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Particle p = ParticleManager.NewParticle(NPC.Center, Vector2.Zero,
                            ParticleManager.NewInstance<GoldSparkleParticle>(), Color.White, sparkleSize);
                        p.timeLeft = 8;
                    }
                }

                //Rotate head 90-ish degrees upward
                if (Timer < 60)
                {
                    LaserAttackPos = Target.Center;
                    Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                    TargetHeadRotation = directionToTarget.ToRotation();

                    //Slowly accelerate up while charging
                    NPC.velocity *= 1.002f;

                    //Charge up
                    ChargeVisuals<StarParticle2>(Timer, 60);
                }
                else if (Timer == 61)
                {
                    Vector2 velocity = Vector2.Zero;
                    int type = ModContent.ProjectileType<NiiviFrostTelegraphProj>();
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, LaserAttackPos, velocity, type,
                            0, 0, Main.myPlayer);
                    }
                }
                else if (Timer >= 120)
                {
                    //SHOOT LOL
                    Vector2 fireDirection = TargetHeadRotation.ToRotationVector2();
                    float distance = Vector2.Distance(NPC.Center, LaserAttackPos);

                    int type = ModContent.ProjectileType<NiiviLaserBlastProj>();
                    int damage = P1_LaserDamage;
                    float knockback = 1;
                    if (StellaMultiplayer.IsHost)
                    {
                        float size = 5.5f;
                        float beamLength = distance;
                        Projectile.NewProjectile(EntitySource, NPC.Center, fireDirection, type,
                        damage, knockback, Main.myPlayer,
                            ai0: size,
                            ai1: beamLength);
                    }

                    for (int i = 0; i < 16; i++)
                    {
                        Vector2 particleVelocity = fireDirection * 16;
                        particleVelocity = particleVelocity.RotatedByRandom(MathHelper.PiOver4 / 3);
                        particleVelocity *= Main.rand.NextFloat(0.3f, 1f);
                        ParticleManager.NewParticle(NPC.Center, particleVelocity,
                            ParticleManager.NewInstance<StarParticle>(), Color.White, sparkleSize);
                    }

                    Timer = 0;
                    AttackCount++;
                }

                if (AttackCount >= 3)
                {
                    NextAttack = BossActionState.Star_Wrath;
                    ResetState(BossActionState.Swoop_Out);
                }
            }
        }

        private void AI_StarWrath()
        {
            if (AttackTimer == 0)
            {
                Timer++;

                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
                if (Timer >= 60)
                {
                    NPC.velocity = -Vector2.UnitY;
                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 1)
            {
                Timer++;
                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
                if (Timer % 8 == 0 && StellaMultiplayer.IsHost)
                {
                    int type = ModContent.ProjectileType<NiiviCometProj>();
                    int damage = P1_StarWrathDamage;
                    int knockback = 1;

                    float height = 768;
                    Vector2 targetCenter = Target.Center;
                    Vector2 cometOffset = -Vector2.UnitY * height + new Vector2(Main.rand.NextFloat(512, 1750), 0);
                    Vector2 cometPos = targetCenter + cometOffset;

                    float speed = 12;
                    Vector2 velocity = new Vector2(-1, 1) * speed;
                    Projectile.NewProjectile(EntitySource, cometPos, velocity,
                        type, damage, knockback, Main.myPlayer);
                }

                if (Timer >= 360)
                {
                    NextAttack = BossActionState.Charge;
                    ResetState(BossActionState.Swoop_Out);
                }
            }
        }

        private void AI_Charge()
        {
            NPC.rotation = 0;
            Timer++;
            if (Timer < 100)
            {
                OrientArching();
                if (Timer == 1)
                {
                    NPC.TargetClosest();
                }

                NPC.velocity *= 0.8f;
                Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                TargetHeadRotation = directionToTarget.ToRotation();

                LookDirection = DirectionToTarget;
                FlipToDirection();
            }
            else if (Timer < 150)
            {
                ChargeDirection = NPC.Center.DirectionTo(Target.Center);
                NPC.velocity *= 0.3f;
                TargetHeadRotation = MathHelper.Lerp(TargetHeadRotation, ChargeDirection.ToRotation(), 0.08f);
                StartSegmentDirection = Vector2.Lerp(StartSegmentDirection, HeadRotation.ToRotationVector2() * -LookDirection, 0.04f);
                for (int i = 0; i < NPC.oldPos.Length; i++)
                {
                    NPC.oldPos[i] = NPC.position;
                }

                LookDirection = DirectionToTarget;
                FlipToDirection();
            }
            else if (Timer < 180)
            {
                IsCharging = true;
                TargetSegmentRotation = 0;
                StartSegmentDirection = Vector2.Lerp(StartSegmentDirection, HeadRotation.ToRotationVector2() * -LookDirection, 0.04f);

                //DrawChargeTrail = true;
                if (Timer == 151)
                {
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/RekRoar");
                    SoundEngine.PlaySound(soundStyle, NPC.position);
                }
                NPC.velocity = ChargeDirection * 40;
            }
            else if (Timer < 240)
            {
                IsCharging = false;
                OrientArching();
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.Pi / 60);
                NPC.velocity *= 0.96f;

            }
            else
            {
                IsCharging = false;
                Timer = 0;
                AttackCount++;
                if (AttackCount >= 3)
                {
                    NextAttack = BossActionState.Thunderstorm;
                    ResetState(BossActionState.Swoop_Out);
                }
            }
        }

        private void AI_Thunderstorm()
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            //Aight, this shouldn't be too hard to do
            //She flies up and rains down lightning
            if (AttackTimer == 0)
            {
                Timer++;

                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
                if (Timer >= 60)
                {
                    NPC.velocity = -Vector2.UnitY;
                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 1)
            {
                Timer++;
                shaderSystem.TintScreen(Color.Black, 0.5f);
                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();

                NPC.velocity *= 1.05f;
                if (Timer >= 60)
                {
                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 2)
            {
                NPC.velocity *= 0.98f;
                Timer++;

                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();

                if (Timer == 1)
                {
                    LaserAttackPos = Target.Center;
                }

                if (Timer > 30 && Timer % 15 == 0 && StellaMultiplayer.IsHost)
                {
                    Vector2 pos = Target.Center + Target.velocity * 48;
                    pos += new Vector2(Main.rand.NextFloat(-64, 64), 0);
                    Projectile.NewProjectile(EntitySource, pos, Vector2.UnitY,
                        ModContent.ProjectileType<NiiviThundercloudProj>(), P1_LightningDamage, 2, Main.myPlayer);
                }

                if (Timer >= 90)
                {
                    AttackCount++;
                    Timer = 0;
                }
                if (AttackCount >= 4)
                {
                    AttackTimer++;
                    Timer = 0;
                }
            }
            else if (AttackTimer == 3)
            {
                NPC.velocity *= 0.98f;
                Timer++;
                if (Timer == 45 && StellaMultiplayer.IsHost)
                {
                    Vector2 pos = Target.Center + Target.velocity * 48;
                    Projectile.NewProjectile(EntitySource, pos, Vector2.UnitY,
                        ModContent.ProjectileType<NiiviLightningRayWarnProj>(), P1_LightningDamage, 2, Main.myPlayer);
                }

                if (Timer >= 90)
                {
                    shaderSystem.UnTintScreen();
                    NextAttack = BossActionState.Frost_Breath;
                    ResetState(BossActionState.Swoop_Out);
                }
            }
        }

        private void AI_BabyDragons()
        {

        }
        #endregion

        #region Phase 2
        private void AI_Phase2_Reset()
        {
            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            screenShaderSystem.FlashTintScreen(Color.White, 0.3f, 5);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.position);
            ResetShaders();
            ResetState(BossActionState.Swoop_Out);
            NextAttack = BossActionState.Laser_Blast_V2;
        }

        private void AI_FrostBreath_V2()
        {

        }

        private void AI_Charge_V2()
        {

        }

        private void AI_LaserBlast_V2()
        {
            /*
            * Step 1: Look at target for a bit
            * 
            * Step 2: Charge begin charging massive laser, white particles come in and slowly build up
            * 
            * Step 3: Fire the laser, twice?, Nah maybe three times
            */

            if (AttackTimer == 0)
            {
                Timer++;

                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
                if (Timer >= 60)
                {
                    NPC.velocity = -Vector2.UnitY;
                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 1)
            {
                Timer++;
                float progress = Timer / 60;
                progress = MathHelper.Clamp(progress, 0, 1);
                float sparkleSize = MathHelper.Lerp(0f, 4f, progress);
                if (Timer % 4 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Particle p = ParticleManager.NewParticle(NPC.Center, Vector2.Zero,
                            ParticleManager.NewInstance<GoldSparkleParticle>(), Color.White, sparkleSize);
                        p.timeLeft = 8;
                    }
                }

                //Rotate head 90-ish degrees upward
                if (Timer < 60)
                {
                    LaserAttackPos = Target.Center;
                    Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                    TargetHeadRotation = directionToTarget.ToRotation();

                    //Slowly accelerate up while charging
                    NPC.velocity *= 1.002f;

                    //Charge up
                    ChargeVisuals<StarParticle2>(Timer, 60);
                }
                else if (Timer == 61)
                {
                    Vector2 velocity = Vector2.Zero;
                    int type = ModContent.ProjectileType<NiiviFrostTelegraphProj>();
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, LaserAttackPos, velocity, type,
                            0, 0, Main.myPlayer);
                    }
                }
                else if (Timer == 120)
                {
                    //SHOOT LOL
                    Vector2 fireDirection = TargetHeadRotation.ToRotationVector2();
                    int type = ModContent.ProjectileType<NiiviLaserContinuousProj>();
                    int damage = P1_LaserDamage;
                    float knockback = 1;
                    NPC.rotation = HeadRotation;
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, NPC.Center, fireDirection, type,
                        damage, knockback, Main.myPlayer, ai1: NPC.whoAmI);
                    }


                    ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                    shaderSystem.TintScreen(Color.White, 0.3f);
                }
                else if (Timer >= 120)
                {
                    NPC.velocity *= 0.98f;
                    TargetHeadRotation += 0.02f;
                    NPC.rotation += 0.02f;
                }

                if (Timer >= 720)
                {
                    ResetShaders();
                    NextAttack = BossActionState.Laser_Blast_V2;
                    ResetState(BossActionState.Swoop_Out);
                }
            }
        }

        private void AI_StarWrath_V2()
        {

        }

        private void AI_Thunderstorm_V2()
        {

        }
        #endregion

        #region Phase 3
        private void AI_Phase3_Reset()
        {
            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            screenShaderSystem.FlashTintScreen(Color.White, 0.3f, 5);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.position);
            ResetShaders();
            ResetState(BossActionState.Swoop_Out);
            NextAttack = BossActionState.Laser_Blast_V2;
        }
        #endregion

        public override void OnKill()
        {
            ResetShaders();
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedNiiviBoss, -1);
        }

        private void ResetShaders()
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            shaderSystem.UnTintScreen();
            shaderSystem.UnDistortScreen();
            shaderSystem.UnVignetteScreen();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<PureHeart>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<NiiviBossRel>()));
        }
    }
}
