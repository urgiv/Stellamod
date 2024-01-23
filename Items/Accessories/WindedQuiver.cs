﻿using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Arrows;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    internal class WindedQuiverPlayer : ModPlayer
    {
        public bool hasQuiver;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasQuiver = false;
        }

        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly && hasQuiver)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordThrow"));
                type = ModContent.ProjectileType<ShinobiKnife>();
                damage += 2;
                velocity *= 2f;
            }
        }
    }

    internal class WindedQuiver : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 46;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<WindedQuiverPlayer>().hasQuiver = true;
        }
    }
}
