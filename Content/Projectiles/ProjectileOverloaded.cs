using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;

namespace GenshinMod.Content.Projectiles
{
    public class ProjectileOverloaded : GenshinProjectile
	{
		public Texture2D TextureProjectile;
		float alpha = 1f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Overload Blast");
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 20;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			Main.projFrames[Projectile.type] = 9;
		}

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2f);
			TextureProjectile ??= GetTexture();
		}

        public override void SafeAI()
		{
			Projectile.friendly = FirstFrame;
			Lighting.AddLight(Projectile.Center, 0.6f, 0.3f, 0.4f);
			if (timeSpent % 2 == 0)
			{
				Projectile.frame++;
				if (Projectile.frame > 9)
					Projectile.Kill();
			}

			if (timeSpent > 6) alpha -= (1 / 20f) * 2;
		}

		public override void PostDraw(Color lightColor)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
			Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
			float rotation = Projectile.rotation + (OwnerGenshinPlayer.LastUseDirection == 1 ? 0f : MathHelper.ToRadians(90f));
			Rectangle rectangle = TextureProjectile.Bounds;
			rectangle.Height /= Main.projFrames[Projectile.type];
			rectangle.Y += Projectile.frame * rectangle.Height;
			Color color = GenshinElementUtils.GetColor(GenshinElement.PYRO) * alpha * 0.6f;
			Color color3 = new Color(255, 255, 155) * alpha * 0.7f;
			Color color2 = GenshinElementUtils.GetColor(GenshinElement.ELECTRO) * alpha * 0.8f;
			Color color4 = new Color(255, 200, 255) * alpha;

			spriteBatch.Draw(TextureProjectile, drawPosition, rectangle, color, rotation, rectangle.Size() * 0.5f, Projectile.scale * 2.2f * (timeSpent / 10f), SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureProjectile, drawPosition, rectangle, color3, rotation, rectangle.Size() * 0.5f, Projectile.scale * 2f * (timeSpent / 10f), SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureProjectile, drawPosition, rectangle, color2, rotation + (float)Math.PI * 0.25f, rectangle.Size() * 0.5f, Projectile.scale * 1.2f * (timeSpent / 10f), SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureProjectile, drawPosition, rectangle, color4, rotation + (float)Math.PI * 0.25f, rectangle.Size() * 0.5f, Projectile.scale * (timeSpent / 10f), SpriteEffects.None, 0f);
		}
	}
}
