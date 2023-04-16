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
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace GenshinMod.Content.Characters.Lisa.Projectiles
{
    public class LisaProjectileNormal : GenshinProjectile
	{
		public Texture2D TextureProjectile;
		public List<Vector2> Positions;
		public float lightmult = 1f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lizap");
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 20;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
			AttackWeight = AttackWeight.LIGHT;
		}

		public override void OnSpawn(IEntitySource source)
		{
			TextureProjectile ??= GetTexture();
			CalculateNodes();

			Vector2 direction = Projectile.Center - Owner.Center;
			if (direction.Length() > 30f)
			{
				direction.Normalize();
				Vector2 position = Owner.Center + direction;

				SpawnDust<LisaDustRound>(position + direction * 10f, direction, 1f, 1f, 10, 4);
			}
		}

        public override void SafeAI()
		{
			FirstFrameHit();
			if (TimeSpent % 7 == 0) CalculateNodes();
			if (TimeSpent > 10) lightmult *= 0.9f;
		}

        public override void Kill(int timeLeft)
		{
		}

		public void CalculateNodes()
		{
			Positions = new List<Vector2>();
			Vector2 direction = Projectile.Center - Owner.Center;
			float length = direction.Length();
			float segmentLength = 32f;

			for (int i = 0; i < length; i += (int)segmentLength)
			{
				Vector2 newPosition = direction.RotatedByRandom(MathHelper.ToRadians((i / segmentLength) * 5f));
				newPosition.Normalize();
				newPosition *= length - i;
				newPosition = Owner.Center + newPosition;
				Positions.Add(newPosition);
			}
		}

		public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
		{
			Vector2 lastPosition = Owner.Center;
			Color color = new Color(155, 155, 255) * lightmult;
			foreach (Vector2 position in Positions)
			{
				Vector2 drawPosition = Vector2.Transform(position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, Color.White * 0.4f * lightmult, Main.rand.NextFloat(3.14f), TextureProjectile.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.2f, Main.rand.NextFloat(3.14f), TextureProjectile.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.05f, 0f, TextureProjectile.Size() * 0.5f, Projectile.scale * 2f, SpriteEffects.None, 0f);

				if (lastPosition != Owner.Center)
				{
					Vector2 direction = position - lastPosition;
					float rotation = Main.rand.NextFloat(3.14f);
					float length = direction.Length();
					float factor = (length / (TextureProjectile.Width));
					direction.Normalize();
					for (float i = 0; i < length; i += factor)
					{
						Vector2 drawPosition2 = Vector2.Transform(position - direction * i - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
						spriteBatch.Draw(TextureProjectile, drawPosition2, null, Color.White * 0.5f * lightmult, rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * 0.66f, SpriteEffects.None, 0f);
						spriteBatch.Draw(TextureProjectile, drawPosition2, null, color * 0.2f, rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0f);
						spriteBatch.Draw(TextureProjectile, drawPosition2, null, color * 0.05f, direction.ToRotation(), TextureProjectile.Size() * 0.5f, Projectile.scale * 1.7f, SpriteEffects.None, 0f);

					}
				}
				lastPosition = position;
			}
		}
	}
}
