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
    public class LisaProjectileBurst : GenshinProjectile
	{
		public static Texture2D TextureProjectile;
		public static Texture2D TextureGlow;
		public static Texture2D TextureDot;
		public static float Range = 160f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lamp");
		}

		public override void SetDefaults()
		{
			Projectile.width = 1;
			Projectile.height = 1;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 60 * 15;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
		}

		public override void OnSpawn(IEntitySource source)
		{
			for (int i = 0; i < 35; i++)
			{
				Vector2 direction = (Vector2.UnitY * Main.rand.NextFloat(5f, 15f)).RotatedByRandom(MathHelper.ToRadians(360));
				SpawnDust<LisaDustRound>(Projectile.Center + direction * 2f, direction, 1f, 1f, 0);
			}

			TextureProjectile ??= GetTexture();
			TextureGlow ??= GetTexture(Texture + "_Glowmask");
			TextureDot ??= GetTexture("GenshinMod/Content/Characters/Lisa/Projectiles/LisaProjectileSkill");
		}

		public override void SafeAI()
		{
			SpawnDust<LisaDustRound>(1f, 1f, 50, 1, 10);
			Projectile.velocity *= 0.95f;
			if (Main.rand.NextBool(10))
            {
				Vector2 direction = (Vector2.UnitY * Main.rand.NextFloat(5f, 15f)).RotatedByRandom(MathHelper.ToRadians(360));
				GenshinProjectile.SpawnDust<LisaDustRound>(Projectile.Center + direction * 2f, direction, 1f, 1f, 10);
			}

			if (TimeSpent % 30 == 0)
            {
				NPC target = null;
				float range = Range;
				foreach (NPC npc in Main.npc)
				{
					Vector2 dir = npc.Center - Projectile.Center;
					if (CanHomeInto(npc) && dir.Length() < range)
					{
						target = npc;
						range = dir.Length();
					}
				}

				if (target != null)
				{
					SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap);
					int type = ModContent.ProjectileType<LisaProjectileBurstHit>();
					SpawnProjectile(target.Center, Vector2.Zero, type, Projectile.damage, Projectile.knockBack);
				}
			}

			if (FirstFrame)
			{
				foreach (NPC npc in Main.npc)
				{
					Vector2 dir = npc.Center - Projectile.Center;
					if (CanHomeInto(npc) && dir.Length() < Range)
					{
						int type = ModContent.ProjectileType<LisaProjectileBurstHit>();
						SpawnProjectile(npc.Center, Vector2.Zero, type, Projectile.damage, Projectile.knockBack, 1f);
					}
				}
			}
		}

		public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
		{
			Color color = new Color(155, 155, 255);
			float colorMult = (((float)Math.Sin(TimeSpent * 0.2f)) * 0.25f + 0.7f);
			float scaleMult = (((float)Math.Sin(TimeSpent * 0.1f)) * 0.075f + 1.1f);
			float heightOffset = (float)Math.Sin(TimeSpent * 0.03f) * 8f;

			Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + heightOffset), Main.GameViewMatrix.EffectMatrix);
			spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * 0.8f * scaleMult, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureProjectile, drawPosition, null, lightColor, Projectile.rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * 0.8f, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureGlow, drawPosition, null, color * colorMult, Projectile.rotation, TextureGlow.Size() * 0.5f, Projectile.scale * 0.8f, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureGlow, drawPosition, null, color * colorMult * 0.25f, Projectile.rotation, TextureGlow.Size() * 0.5f, Projectile.scale * 0.9f, SpriteEffects.None, 0f);


			int nbDots = 150;
			float segment = (MathHelper.TwoPi / nbDots);
			float rotationBonus = - 0.011519178f * OwnerGenshinPlayer.Timer;
			float lightFactor = ((float)Math.Sin(OwnerGenshinPlayer.Timer * 0.25f) + 1.5f) * 0.25f;
			float scaleMult2 = (((float)Math.Sin(TimeSpent * 0.05f)) * 0.25f + 0.75f);

			for (int i = 0; i < nbDots; i++)
			{
				Vector2 position = Projectile.Center + (Vector2.UnitY * Range).RotatedBy(segment * i + rotationBonus);
				Vector2 drawPosition2 = Vector2.Transform(position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
				spriteBatch.Draw(TextureDot, drawPosition2, null, color * lightFactor * 0.5f, Main.rand.NextFloat(3.14f), TextureDot.Size() * 0.5f, 0.5f * scaleMult2, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureDot, drawPosition2, null, color * lightFactor * 0.25f, Main.rand.NextFloat(3.14f), TextureDot.Size() * 0.5f, 1.2f * scaleMult2, SpriteEffects.None, 0f);

				GenshinProjectile.SpawnDust<LisaDustRound>(position, Vector2.Zero, 1f, 0.75f, 10, 1, 180);
			}
		}
	}
}
