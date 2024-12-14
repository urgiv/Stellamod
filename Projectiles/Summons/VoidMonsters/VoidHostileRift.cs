﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Stellamod.Helpers;
using Stellamod.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.VoidMonsters
{
    public class VoidHostileRift : ModProjectile
    {
        private bool _sync;
        private int _particleCounter;
        private const int Body_Particle_Count = 4;

        //Lower number = faster
        private const int Body_Particle_Rate = 2;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 30;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 150;
            Projectile.height = 40;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;

            //Local NPC hit time so it doesn't interfere with other piercing weps
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Draw The Body
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(60, 0, 118),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, ColorFunctions.MiracleVoid, lightColor, 1);
            DrawHelper.DrawAdditiveAfterImage(Projectile, ColorFunctions.MiracleVoid, Color.Black, ref lightColor);
            // Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int projFrames = Main.projFrames[Projectile.type];
            int frameHeight = texture.Height / projFrames;
            int startY = frameHeight * Projectile.frame;

            Rectangle frame = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 drawOrigin = frame.Size() / 2f;
            float offsetX = 20f;
            drawOrigin.X = Projectile.spriteDirection == 1 ? frame.Width - offsetX : offsetX;

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new Vector2(Projectile.width / 2 - frameOrigin.X, Projectile.height - frame.Height);
            Vector2 drawPos = Projectile.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                Main.EntitySpriteDraw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), Projectile.rotation, frameOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                Main.EntitySpriteDraw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), Projectile.rotation, frameOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return base.PreDraw(ref lightColor);
        }

        public override void AI()
        {
            if (!_sync && Main.myPlayer == Projectile.owner)
            {
                Projectile.netUpdate = true;
                _sync = true;
            }

            float slashRotation = Projectile.ai[0];
            Projectile.rotation = slashRotation;
            Visuals();
        }

        private void Visuals()
        {
            _particleCounter++;
            if (_particleCounter > Body_Particle_Rate)
            {

                _particleCounter = 0;
            }

            float scaleOut = 20;
            if(Projectile.timeLeft < scaleOut && Projectile.DamageType != DamageClass.Summon)
            {
                Projectile.scale = MathHelper.Lerp(0f, 1f, Projectile.timeLeft / scaleOut);
            }

            DrawHelper.AnimateTopToBottom(Projectile, 3);
            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }

        public override void OnKill(int timeLeft)
        {

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SyliaRiftClose"));
        }
    }
}
