﻿using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.CommanderGintzia.Hands
{
    public class ComeHereBlast : ModProjectile
    {
        private Vector2[] _oldSwingPos;
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            _oldSwingPos = new Vector2[32];
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.height = 64;
            Projectile.width = 64;
            Projectile.hostile = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 120;
            Projectile.penetrate = -1;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center;

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 end = start + direction * Projectile.velocity;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }
        
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle windStorm = new SoundStyle("Stellamod/Assets/Sounds/WindStorm");
                SoundEngine.PlaySound(windStorm);
            }

            for (int i = 0; i < _oldSwingPos.Length; i++)
            {
                float progress = (float)i / (float)_oldSwingPos.Length;
                Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity, progress);
                _oldSwingPos[i] = pos;
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1f;
            return MathHelper.SmoothStep(baseWidth, baseWidth, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.White;

            if (Timer < 30)
            {
                startColor *= Timer / 30f;
            }

            if (Timer > 90)
            {
                float p = (Timer - 90) / 30f;
                p = 1f - p;
                startColor *= p;
            }

            return Color.Lerp(startColor, Color.Transparent, Easing.InCirc(completionRatio));
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public override bool PreDraw(ref Color lightColor)
        {
            //Draw Trail
            Main.spriteBatch.RestartDefaults();
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.Dashtrail);
            Vector2 trailOffset = -Main.screenPosition + Projectile.Size / 2;
            TrailDrawer.DrawPrims(_oldSwingPos, trailOffset, 155);
            return false;
        }
    }
}
