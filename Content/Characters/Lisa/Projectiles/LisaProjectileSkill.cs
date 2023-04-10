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
    public class LisaProjectileSkill : GenshinProjectile
	{
		public Texture2D TextureProjectile;
		public List<Vector2> Positions;
		public float lightmult = 1f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Electro Orb");
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 180;
			Projectile.alpha = 255;
			Projectile.penetrate = 1;
			AttackWeight = AttackWeight.LIGHT;
		}

		public override void OnSpawn(IEntitySource source)
		{
			TextureProjectile ??= GetTexture();
			Positions = new List<Vector2>();
		}

        public override void SafeAI()
		{
			SpawnDust<LisaDustRound>(1f, 1f, 0, 1, 20);
			if (TimeSpent > 140) lightmult *= 0.925f;

			Positions.Add(Projectile.Center);
			if (Positions.Count > 5)
				Positions.RemoveAt(0);
		}

        public override void Kill(int timeLeft)
		{
			if (timeLeft > 0)
				SpawnDust<LisaDustRound>(1f, 1f, 10, 10);
		}

        public override void SafeOnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (OwnerCharacter is CharacterLisa lisa) lisa.TryApplyStackLisa(target);
        }

        public override void PostDraw(Color lightColor)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
			Vector2 lastPosition = Owner.Center;
			Color color = new Color(155, 155, 255);
			float lightFactor = lightmult * ((float)Math.Sin(TimeSpent * 0.25f) + 1.5f) * 0.25f;
			float sizeFactor = ((float)Math.Sin(TimeSpent * 0.125f)) * 0.33f + 0.8f;
			Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
			spriteBatch.Draw(TextureProjectile, drawPosition, null, Color.White * lightFactor, Main.rand.NextFloat(3.14f), TextureProjectile.Size() * 0.5f, Projectile.scale * 1.75f * sizeFactor, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureProjectile, drawPosition, null, color * lightFactor * 0.5f, Main.rand.NextFloat(3.14f), TextureProjectile.Size() * 0.5f, Projectile.scale * 2.5f * sizeFactor, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureProjectile, drawPosition, null, color * lightFactor * 0.125f, Main.rand.NextFloat(3.14f), TextureProjectile.Size() * 0.5f, Projectile.scale * 3f * sizeFactor, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureProjectile, drawPosition, null, Color.White * lightFactor * 0.5f, Main.rand.NextFloat(3.14f), TextureProjectile.Size() * 0.5f, Projectile.scale * 1.25f * sizeFactor, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureProjectile, drawPosition, null, color * lightFactor * 0.25f, Main.rand.NextFloat(3.14f), TextureProjectile.Size() * 0.5f, Projectile.scale * 2f * sizeFactor, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureProjectile, drawPosition, null, color * lightFactor * 0.0675f, Main.rand.NextFloat(3.14f), TextureProjectile.Size() * 0.5f, Projectile.scale * 2.5f * sizeFactor, SpriteEffects.None, 0f);

			float mult = 0.15f;
			foreach (Vector2 position in Positions)
			{
				Vector2 drawPosition2 = Vector2.Transform(position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
				spriteBatch.Draw(TextureProjectile, drawPosition2, null, Color.White * lightFactor * mult, Main.rand.NextFloat(3.14f), TextureProjectile.Size() * 0.5f, Projectile.scale * 1.75f * sizeFactor * mult * 1.5f, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition2, null, color * lightFactor * 0.5f * mult, Main.rand.NextFloat(3.14f), TextureProjectile.Size() * 0.5f, Projectile.scale * 2.5f * sizeFactor * mult * 1.5f, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureProjectile, drawPosition2, null, color * lightFactor * 0.125f * mult, Main.rand.NextFloat(3.14f), TextureProjectile.Size() * 0.5f, Projectile.scale * 3f * sizeFactor * mult * 1.5f, SpriteEffects.None, 0f);
				mult += 0.125f;
			}
		}
	}
}
