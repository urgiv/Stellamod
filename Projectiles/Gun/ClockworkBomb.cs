﻿using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    internal class ClockworkBomb : ModProjectile
    {
        float Speeb;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 12;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
        }

        public Vector3 HuntrianColorXyz;
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawDimLight(Projectile, HuntrianColorXyz.X, HuntrianColorXyz.Y, HuntrianColorXyz.Z, new Color(83, 254, 164), lightColor, 2);
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(83, 254, 164), Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        public override void AI()
        {
            //Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1; 
            Projectile.ai[0]++;
            if(Projectile.ai[0] <= 2)
            {
                Speeb = Main.rand.NextFloat(0.92f, 0.96f);
            }
            Projectile.velocity *= Speeb;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if(Projectile.velocity.Length() <= 0.1f && Projectile.active)
            {
                Projectile.Kill();
            }


        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.ForestGreen, 1f).noGravity = true;
            }

            float damage = Projectile.damage;
            damage *= 0.5f;
            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<KaBoomMagic2>(), (int)damage, 3);
            p.friendly = true;
            p.usesLocalNPCImmunity = true;
            p.localNPCHitCooldown = -1;

            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ClockworkCity1"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ClockworkCity2"), Projectile.position);
            }

            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 2f);
        }
    }
}
