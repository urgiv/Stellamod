﻿using Microsoft.Xna.Framework;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    public abstract class BaseSafunaiItem : ClassSwapItem
    {
        public int combo;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            combo++;
            if (combo == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Safunais"), position);

            }
            if (combo == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Safunais2"), position);

            }
            if (combo == 3)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Safunais"), position);

            }
            if (combo == 4)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Safunais2"), position);

            }
            if (combo == 5)
            {
                combo = 0;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Safunais3"), position);
            }

            float distanceMult = Main.rand.NextFloat(0.8f, 1.2f);
            float curvatureMult = 0.7f;
            bool slam = combo % 5 == 4;

            Vector2 direction = velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));
            Projectile proj = Projectile.NewProjectileDirect(source, position, direction, type, damage, knockback, player.whoAmI);

            if (proj.ModProjectile is BaseSafunaiProjectile modProj)
            {
                modProj.SwingTime = (int)(Item.useTime * UseTimeMultiplier(player) * (slam ? 1.75f : 1)) * 16;
                modProj.SwingDistance = player.Distance(Main.MouseWorld) * distanceMult;
                modProj.Curvature = 0.33f * curvatureMult;
                modProj.Flip = combo % 2 == 1;
                modProj.Slam = slam;
                modProj.PreSlam = combo % 5 == 3;
                modProj.Projectile.netUpdate = true;
            }

            return false;
        }

        public override float UseTimeMultiplier(Player player) => player.GetAttackSpeed(DamageClass.Melee); //Scale with melee speed buffs, like whips
    }
}
