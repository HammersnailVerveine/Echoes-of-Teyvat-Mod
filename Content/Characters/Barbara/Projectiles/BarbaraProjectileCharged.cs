using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Barbara.Projectiles
{
    public class BarbaraProjectileCharged : GenshinProjectile
	{
		public static Texture2D texture;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Water Blast");
		}

		public override void SetDefaults()
		{
			Projectile.width = 80;
			Projectile.height = 80;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 70;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			Main.projFrames[Projectile.type] = 3;
		}

        public override void OnSpawn(IEntitySource source)
		{
			texture ??= ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			Projectile.rotation = Main.rand.NextFloat(-(float)Math.PI / 4f, (float)Math.PI / 4f);
			Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;
			Projectile.scale = 0.8f;

			/*
			SpawnDust<BarbaraDustBubble>(0.5f, 1f, 10, 7);
			SpawnDust<BarbaraDustStar>(0.2f, 1f, 0, 5);
			SpawnDust<BarbaraDustStarBig>(0.1f, 1f, 0, 2);
			*/

			Vector2 direction = Projectile.Center - Owner.Center;
			if (direction.Length() > 30f)
			{
				direction.Normalize();
				direction *= 3f;
				Vector2 position = Owner.Center + direction;

				SpawnDust<BarbaraDustBubble>(position, direction, 0.5f, 1f, 10, 6);
				SpawnDust<BarbaraDustStar>(position, direction * 0.5f, 0.2f, 1f, 10, 5);
				SpawnDust<BarbaraDustStar>(position, Vector2.Zero, 0.5f, 1f, 10, 4);
			}
		}

        public override void SafeAI()
		{
			Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.4f);
			Projectile.rotation += 0.115f * Projectile.spriteDirection;

			if (timeSpent == 30)
			{
				SoundEngine.PlaySound(SoundID.SplashWeak, Projectile.Center);
				Projectile.friendly = true;
				ResetImmunity();
			}
			else if (timeSpent < 30)
            {
				Projectile.scale *= 0.9f;
				SpawnDust<BarbaraDustBubble>(0.5f, 1f, 0, 1, 5);
				SpawnDust<BarbaraDustStar>(0.5f, 1f, 0, 1, 8);
			}
			else
			{
				Projectile.friendly = false;
				Projectile.scale *= 1.125f;
				SpawnDust<BarbaraDustBubble>(0.5f, 1f, 0, 1, 3);
				SpawnDust<BarbaraDustStar>(0.5f, 1f, 30, 1, 5);
				SpawnDust<BarbaraDustStarBig>(0.5f, 1f, 15, 1, 10);
			}
			if (Projectile.ai[0] != 0f)
				Projectile.position = Main.npc[(int)Projectile.ai[0]].Center - Projectile.Size * 0.5f;
		}

		public override void PostDraw(Color lightColor)
		{
			if (timeSpent > 10)
			{
				SpriteBatch spriteBatch = Main.spriteBatch;
				Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
				float mult = 0.6f;
				if (timeSpent < 30) mult *= 0.4f;
				Color color = Color.White * (1.65f - (Projectile.scale > 0.55f ? 1.65f : Projectile.scale * 3f)) * mult;
				Color color2 = Color.White * (1.445f - (Projectile.scale > 0.85f ? 1.445f : Projectile.scale * 1.7f)) * mult;
				Color color3 = Color.White * (1.3f - (Projectile.scale > 1.4f ? 1.3f : Projectile.scale)) * mult;
				Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height / 3);
				spriteBatch.Draw(texture, drawPosition, rectangle, color , Projectile.rotation - ((float)Math.PI / 6f) * Projectile.spriteDirection, rectangle.Size() * 0.5f, Projectile.scale * 4f, SpriteEffects.None, 0f);
				spriteBatch.Draw(texture, drawPosition, rectangle, color , Projectile.rotation - ((float)Math.PI / 6f) * Projectile.spriteDirection, rectangle.Size() * 0.5f, Projectile.scale * 3.5f, SpriteEffects.None, 0f);
				rectangle.Y += texture.Height / 3;
				spriteBatch.Draw(texture, drawPosition, rectangle, color2 , Projectile.rotation * Projectile.spriteDirection, rectangle.Size() * 0.5f, Projectile.scale * 2.25f, SpriteEffects.None, 0f);
				rectangle.Y += texture.Height / 3;
				spriteBatch.Draw(texture, drawPosition, rectangle, color3, Projectile.rotation + ((float)Math.PI / 6f) * Projectile.spriteDirection, rectangle.Size() * 0.5f, Projectile.scale * 1f, SpriteEffects.None, 0f);
			}
		}
	}
}
