using Microsoft.Xna.Framework;
using GenshinMod.Content.Dusts;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework.Graphics;
using GenshinMod.Common.GameObjects.Enums;

namespace GenshinMod.Content.NPCs.Boss.HypostasisGeo.Projectiles
{
    public class HypostasisGeoProjectileCube : GenshinProjectile
	{
		public Color ColorGeo;
		public Color ColorBrown;
		public Texture2D TextureCube => HypostasisGeo.TextureCube;
		public Texture2D TextureGlow => HypostasisGeo.TextureGlow;
		public int TextureIndex;
		public int RandomDelay;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hypostasis Cube");
		}

		public override void SetDefaults()
		{
			Projectile.width = 25;
			Projectile.height = 25;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
			Projectile.aiStyle = 0;
            Projectile.timeLeft = 3000;
			Projectile.scale = 1f;
			PostDrawAdditive = true;
			Projectile.alpha = 255;
			Element = GenshinElement.GEO;
		}

		public override void OnSpawn(IEntitySource source)
		{
			HypostasisGeo.TextureCube ??= ModContent.Request<Texture2D>("GenshinMod/Content/NPCs/Boss/HypostasisGeo/HypostasisGeoCube", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			HypostasisGeo.TextureGlow ??= ModContent.Request<Texture2D>("GenshinMod/Content/NPCs/Boss/HypostasisGeo/HypostasisGeoGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			ColorGeo = GenshinElementUtils.GetColor(GenshinElement.GEO);
			ColorBrown = new Color(115, 75, 45);
			Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2f);
			TextureIndex = Main.rand.Next(4);
			RandomDelay = Main.rand.Next(30);
		}

        public override void SafeAI()
		{
			if (TimeSpent < 120 + RandomDelay)
				Projectile.rotation += 0.15f;
			else if (TimeSpent < 180 + RandomDelay)
				Projectile.rotation += 0.15f - (0.15f / 60 * (TimeSpent - 120 - RandomDelay));

			if (TimeSpent > 210 + RandomDelay)
            {
				if (Projectile.tileCollide)
					Projectile.velocity.Y += 0.5f;
				else Projectile.velocity.Y *= 0.7f;
			}

			SpawnDust<HypostasisGeoDustSmall>(0f, 1f, 16, 1, 60);
		}

        public override void Kill(int timeLeft)
		{
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.tileCollide = false;
			SoundEngine.PlaySound(SoundID.NPCDeath43, Projectile.Center);
			SpawnDust<HypostasisGeoDust>(Projectile.Center, - Projectile.velocity, Main.rand.NextFloat(2.5f) + 2.5f, 1f, 16, 2);
			SpawnDust<HypostasisGeoDustSmall>(Projectile.Center, - Projectile.velocity, Main.rand.NextFloat(2.5f) + 2.5f, 1f, 16, 5);
			return false;
		}

		public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
		{
			Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
			float scaleMultGlow = ((float)Math.Sin(TimeSpent * 0.05f)) * 0.05f + 1.1f;
			Rectangle rectangle = TextureGlow.Bounds;
			rectangle.Height /= 4;
			rectangle.Y += rectangle.Height * TextureIndex;
			float Glow = 0.4f - ((float)Math.Sin(TimeSpent * 0.02f) * 0.15f + 0.15f);
			float Fade = TimeSpent < 45 ? 1f - (1f / 45 * (45 - TimeSpent)) : Projectile.timeLeft < 45 ? 1f - (1f / 45 * (45 - Projectile.timeLeft)) : 1f;

			spriteBatch.Draw(TextureCube, drawPosition, null, ColorGeo * Glow * Fade, Projectile.rotation, TextureCube.Size() * 0.5f, Projectile.scale * 1.15f * scaleMultGlow, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureCube, drawPosition, null, ColorBrown * Fade, Projectile.rotation, TextureCube.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureGlow, drawPosition, rectangle, ColorGeo * Fade, Projectile.rotation, rectangle.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureGlow, drawPosition, rectangle, ColorGeo * 0.2f * Fade, Projectile.rotation, rectangle.Size() * 0.5f, Projectile.scale * scaleMultGlow, SpriteEffects.None, 0f);
		}
    }
}
