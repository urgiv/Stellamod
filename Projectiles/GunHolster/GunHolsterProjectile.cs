﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal abstract class GunHolsterProjectile : ModProjectile
    {
        private enum ActionState
        {
            Holster,
            Prepare,
            Fire,
            Hide
        }

        protected float AttackSpeed = 10;
        protected Vector2 HolsterOffset;
        protected bool IsRightHand;

        protected float RecoilRotation = MathHelper.PiOver4;
        protected float RecoilDistance = 5;


        private float PrepTime => 4 / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float ExecTime => AttackSpeed / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float HideTime => AttackSpeed / Owner.GetTotalAttackSpeed(Projectile.DamageType) / 2;
        private Player Owner => Main.player[Projectile.owner];

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }


        private ActionState State
        {
            get => (ActionState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        private float IdleRotation
        {
            get
            {
                Vector2 direction = Owner.Center.DirectionTo(Main.MouseWorld);
                float rotation = direction.ToRotation();
                return rotation;
            }
        }

        private float StartRotation;
        private float FireStartRotation;
        private float HideStartRotation;
        private float Recoil;
 
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }


        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            int type = ModContent.ItemType<Items.Weapons.Ranged.GunSwapping.GunHolster>();
            if (Owner.HeldItem.type != type
                && Main.mouseItem.type != type)
                Projectile.Kill();

            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            switch (State)
            {
                case ActionState.Holster:
                    AI_Holster();
                    break;
                case ActionState.Prepare:
                    AI_Prepare();
                    break;
                case ActionState.Fire:
                    AI_Fire();
                    break;
                case ActionState.Hide:
                    AI_Hide();
                    break;
            }
        }

        protected abstract void Shoot(Vector2 position, Vector2 direction);

        private void AI_Holster()
        {
            Vector2 direction = Owner.Center.DirectionTo(Main.MouseWorld);
            Projectile.rotation = direction.ToRotation();

            bool mouseInput = IsRightHand ? Main.mouseRight : Main.mouseLeft;
            if (Projectile.owner == Main.myPlayer 
                && mouseInput 
                && Owner.PickAmmo(Owner.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int useAmmoItemId))
            {
                if(Projectile.spriteDirection == -1)
                {
                    StartRotation = Projectile.rotation - MathHelper.ToRadians(15);
                    FireStartRotation = StartRotation + MathHelper.ToRadians(15);
                    HideStartRotation = StartRotation + RecoilRotation;
                }
                else
                {
                    StartRotation = Projectile.rotation + MathHelper.ToRadians(15);
                    FireStartRotation = StartRotation - MathHelper.ToRadians(15);
                    HideStartRotation = StartRotation - RecoilRotation;
                }


                State = ActionState.Prepare;
                Timer = 0;
            }

            SetGunPosition();    
        }

        private void AI_Prepare()
        {
            Timer++;
            float endRotation = FireStartRotation;
            float progress = Timer / PrepTime;
            float easedProgress = Easing.OutCubic(progress);
            Projectile.rotation = MathHelper.Lerp(StartRotation, endRotation, easedProgress);

            SetGunPosition();
            if (Timer >= PrepTime)
            {
                State = ActionState.Fire;
                Timer = 0;
            }
        }


        private void AI_Fire()
        {
            Timer++;
            float endRotation = HideStartRotation;
            float progress = Timer / ExecTime;
            float easedProgress = Easing.OutExpo(progress);

            Projectile.rotation = MathHelper.Lerp(StartRotation, endRotation, easedProgress);
            SetGunPosition();

            if (Timer == 1)
            {
                Vector2 direction = Owner.Center.DirectionTo(Main.MouseWorld);
                Vector2 position = Projectile.Center + direction * Projectile.width / 2;
                Shoot(position, direction);
            }

            Recoil = MathHelper.Lerp(0, RecoilDistance, Easing.SpikeOrb(progress));
            if (Timer >= ExecTime)
            {
                State = ActionState.Hide;
                Timer = 0;
            }
        }

        private void AI_Hide()
        {
            Timer++;
            float progress = Timer / HideTime;
            float easedProgress = Easing.OutCubic(progress);
            Projectile.rotation = MathHelper.Lerp(FireStartRotation, IdleRotation, easedProgress);
            SetGunPosition();

            if (Timer >= HideTime)
            {
                State = ActionState.Holster;
                Timer = 0;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public void SetGunPosition()
        {
            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            if (IsRightHand)
            {
                Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
                Vector2 armPosition = Owner.GetBackHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

                armPosition.Y += Owner.gfxOffY;
                Projectile.Center = armPosition; // Set projectile to arm position
                Projectile.Center += HolsterOffset.RotatedBy(Projectile.rotation);
                Projectile.position -= new Vector2(0, 4);
            }
            else
            {
                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
                Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

                armPosition.Y += Owner.gfxOffY;
                Projectile.Center = armPosition; // Set projectile to arm position
                Projectile.Center += HolsterOffset.RotatedBy(Projectile.rotation);
            }

            Projectile.position += new Vector2(-Recoil, 0).RotatedBy(Projectile.rotation);
            if (Projectile.spriteDirection == -1)
            {
                Projectile.position += new Vector2(0, 12).RotatedBy(Projectile.rotation);
                Projectile.rotation -= MathHelper.ToRadians(180);
 
            }
        

            Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            if (!IsRightHand)
            {
                Owner.heldProj = Projectile.whoAmI;
            }
        }
    }
}
