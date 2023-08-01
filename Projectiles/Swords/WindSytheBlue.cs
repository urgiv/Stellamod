
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Weapons.Swords
{
    public class WindSytheBlue : ModProjectile
    {
        public bool OptionallySomeCondition { get; private set; }
		public bool AddedVel;
		public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Electric Wind Sythe");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{

			Projectile.penetrate--;
			if (Projectile.penetrate <= 0)
				Projectile.Kill();
			else
			{

				if (Projectile.velocity.X != oldVelocity.X)
					Projectile.velocity.X = -oldVelocity.X;

				if (Projectile.velocity.Y != oldVelocity.Y)
					Projectile.velocity.Y = -oldVelocity.Y;

			}
			for (int i = 0; i < 15; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226);
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226);
			}
			return false;
		}
		public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] <= 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Sounds/Custom/Item/SkyrageShasher"), Projectile.position);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Sounds/Custom/Item/SkyrageShasherEle"), Projectile.position);
            }
            Projectile.rotation += 0.1f;
			if(Projectile.timeLeft >= 250 && Projectile.timeLeft <= 310)
            {
				Projectile.alpha -= 10;
			}
		}
		public override bool PreDraw(ref Color lightColor)
		{
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(Color.Lerp(new Color(35, 215, 234), new Color(130, 111, 255), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}
		public override void SetDefaults()
		{
			Projectile.scale = 0.7f;
			Projectile.rotation += 0.1f;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 310;
			Projectile.tileCollide = true;
            Projectile.penetrate = 12;
			Projectile.alpha = 130;
			Projectile.width = 45;
			Projectile.height = 45;
			AIType = ProjectileID.WoodenBoomerang;
        }
    }
}


