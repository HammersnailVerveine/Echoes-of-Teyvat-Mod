using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Barbara.Projectiles
{
    public class BarbaraProjectileSkillCircle : GenshinProjectile
	{
		public static Texture2D texture;
		public int pulse = -10;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Barbara Circle");
		}

		public override void SetDefaults()
		{
			Projectile.width = 80;
			Projectile.height = 80;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 901;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			Element = GenshinElement.HYDRO;
		}

        public override void OnSpawn(IEntitySource source)
        {
			Projectile.scale = 2.5f;
			Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2f);
			texture ??= GetTexture();
		}

        public override void SafeAI()
		{
			Projectile.position = Owner.Center - new Vector2(Projectile.width / 2, Projectile.height / 2);
			Projectile.rotation -= 0.01f * (Projectile.ai[0] == 0.2f ? -1 : 1);
			if (pulse < 10) pulse++;

			Projectile.scale *= 0.99f;
			if (Projectile.scale < 1f) Projectile.scale = 1f;

			if (timeSpent % 180 == 0)
            {
				OwnerCharacter.Heal(Projectile.damage);

				SpawnDust<BarbaraDustStar>(0.2f, 1f, 10, 12);
				pulse = -10;
			}

			
			SpawnDust<BarbaraDustBubble>(0.5f, 1f, 10, 1, 4);

			if (Projectile.scale < 2f)
			{
				Vector2 position = new Vector2(0f, Main.rand.NextFloat(Projectile.height / 2f - 5f, Projectile.height / 2f + 5f) * Projectile.scale).RotatedByRandom(MathHelper.ToRadians(360));
				SpawnDust<BarbaraDustNote>(Projectile.Center + position, Vector2.Zero, 0.25f, 1.2f, 0, 1, 15);
			}
		}

        public override void Kill(int timeLeft)
        {
			SpawnDust<BarbaraDustStarBig>(0.1f, 1f, 0, 4);
			SpawnDust<BarbaraDustStar>(0.2f, 1f, 0, 4);
        }

        public override void PostDraw(Color lightColor)
        {
			SpriteBatch spriteBatch = Main.spriteBatch;
			Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
			Color color = lightColor * 0.9f * (1 - Projectile.scale / 2);
			float randomRotation = -Projectile.rotation + 0.3f * Projectile.scale;
			float scale = Projectile.scale + (0.25f - (Math.Abs(pulse) * 0.01f));
			spriteBatch.Draw(texture, drawPosition, null, color, randomRotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(texture, drawPosition, null, color, -randomRotation + 0.5f, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(texture, drawPosition, null, color * 0.7f, randomRotation * 0.8f + 0.3f, texture.Size() * 0.5f, scale + 0.2f, SpriteEffects.None, 0f);
			spriteBatch.Draw(texture, drawPosition, null, color * 0.7f, -randomRotation * 0.8f + 1.5f, texture.Size() * 0.5f, scale + 0.2f, SpriteEffects.None, 0f);
			spriteBatch.Draw(texture, drawPosition, null, color * 0.4f, randomRotation * 0.6f + 1.8f, texture.Size() * 0.5f, scale + 0.4f, SpriteEffects.None, 0f);
			spriteBatch.Draw(texture, drawPosition, null, color * 0.4f, -randomRotation * 0.6f + 2.5f, texture.Size() * 0.5f, scale + 0.4f, SpriteEffects.None, 0f);
		}
    }
}
