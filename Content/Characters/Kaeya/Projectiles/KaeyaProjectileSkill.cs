using Microsoft.Xna.Framework;
using GenshinMod.Content.Dusts;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.GameObjects.Enums;
using Microsoft.Xna.Framework.Graphics;

namespace GenshinMod.Content.Characters.Kaeya.Projectiles
{
    public class KaeyaProjectileSkill : GenshinProjectile
	{
		public Texture2D TextureProjectile;
		public Texture2D TextureWeapon;

		public float multScale = 0.5f;
		public float multAlpha = 0.2f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Frost Blast");
		}

		public override void SetDefaults()
		{
			Projectile.width = 128;
			Projectile.height = 128;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 60;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
			IgnoreICD = true;
			ElementApplication = ElementApplicationMedium;
		}

		public override void OnSpawn(IEntitySource source)
        {
			TextureWeapon = GetWeaponTexture();
			TextureProjectile = GetTexture();

			//Projectile.direction = Projectile.spriteDirection;

			Projectile.rotation = Projectile.velocity.ToRotation();

			Vector2 direction = Projectile.Center - Owner.Center;
			direction.Normalize();
			direction *= 6f;
			Vector2 position = Owner.Center + direction;

			for (int i = 0; i < 30; i++)
            {
				Vector2 dir = direction.RotatedByRandom(MathHelper.ToRadians(45f)) * Main.rand.NextFloat(0.25f, 1.5f);
				SpawnDust<KaeyaDustFrost>(position, dir, 0f, Main.rand.NextFloat(1.5f, 2.5f), 10);
			}

			for (int i = 0; i < 15; i++)
            {
				Vector2 dir = direction.RotatedByRandom(MathHelper.ToRadians(45f)) * Main.rand.NextFloat(0.25f, 1.5f);
				SpawnDust<KaeyaDustFrostBig>(position, dir, 0f, Main.rand.NextFloat(1.5f, 2.5f), 10);
			}
		}

        public override void SafeAI()
		{
			if (!FirstFrame && Projectile.friendly) {
				Projectile.friendly = false;
				Vector2 velocity = Projectile.velocity;
				velocity.Normalize();
				Projectile.position = Owner.Center - Projectile.Size * 0.5f + velocity * 32f;
			}

			if (multScale < 1f)
            {
				multScale *= 1.1f;
				if (multScale > 1f) multScale = 1f;
			}

			if (timeSpent < 10)
			{
				multAlpha *= 1.2f;
				if (multAlpha > 1f) multAlpha = 1f;
			}
			else multAlpha *= 0.95f;
		}

        public override void Kill(int timeLeft)
		{
		}

		public override void PostDraw(Color lightColor)
		{
			if (!FirstFrame)
			{
				SpriteBatch spriteBatch = Main.spriteBatch;
				Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
				Color color = Color.White * multAlpha;
				float scale = Projectile.scale * multScale;
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation, TextureProjectile.Size() * 0.5f, scale * 2f, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.2f, Projectile.rotation, TextureProjectile.Size() * 0.5f, scale * 2.5f, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation - 0.7f, TextureProjectile.Size() * 0.5f, scale, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.2f, Projectile.rotation - 0.7f, TextureProjectile.Size() * 0.5f, scale * 1.5f, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation + 0.7f, TextureProjectile.Size() * 0.5f, scale, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.2f, Projectile.rotation + 0.7f, TextureProjectile.Size() * 0.5f, scale * 1.5f, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation - 0.4f, TextureProjectile.Size() * 0.5f, scale * 1.5f, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.2f, Projectile.rotation - 0.4f, TextureProjectile.Size() * 0.5f, scale * 2f, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation + 0.4f, TextureProjectile.Size() * 0.5f, scale * 1.5f, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.2f, Projectile.rotation + 0.4f, TextureProjectile.Size() * 0.5f, scale * 2f, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation - 0.2f, TextureProjectile.Size() * 0.5f, scale * 1.75f, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.2f, Projectile.rotation - 0.2f, TextureProjectile.Size() * 0.5f, scale * 2.25f, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation + 0.2f, TextureProjectile.Size() * 0.5f, scale * 1.75f, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.2f, Projectile.rotation + 0.2f, TextureProjectile.Size() * 0.5f, scale * 2.25f, SpriteEffects.None, 0f);

				Vector2 velocity = Projectile.velocity;
				velocity.Normalize();
				drawPosition += velocity * 16f;

				spriteBatch.Draw(TextureWeapon, drawPosition, null, color * 1.5f, Projectile.rotation + SwordRotation, TextureWeapon.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
			}
		}
	}
}
