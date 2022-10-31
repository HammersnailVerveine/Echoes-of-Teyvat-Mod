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
using GenshinMod.Common.ModObjects.Weapons;
using System.Collections.Generic;

namespace GenshinMod.Content.Characters.Albedo.Projectiles
{
    public class AlbedoProjectileSkillMain : GenshinProjectile
	{
        public static float Range = 256f;
        public Texture2D TextureProjectile;
		public Texture2D TextureGlow;
		public Texture2D TextureOutline;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Solar Isotoma");
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
            Projectile.timeLeft = 60 * 30;
			Projectile.scale = 1f;
            ProjectileTrail = true;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
		}

		public override void OnSpawn(IEntitySource source)
		{
			TextureProjectile = GetTexture();
			TextureGlow = ModContent.Request<Texture2D>(Texture + "_GlowMask", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			TextureOutline = ModContent.Request<Texture2D>(Texture + "_Outline", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		}

        public override void SafeAI()
		{
			Projectile.rotation += 0.01f;
			if (OwnerCharacter is CharacterAlbedo albedo)
				albedo.skillActive = true;
		}

        public override void PostDraw(Color lightColor)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
			int nbDots = 150;
			float segment = (MathHelper.TwoPi / nbDots);
			float rotationBonus = 0.005f * OwnerGenshinPlayer.Timer;
			float lightFactor = ((float)Math.Sin(OwnerGenshinPlayer.Timer * 0.25f) + 1.5f) * 0.25f;
			float scaleMult2 = (((float)Math.Sin(TimeSpent * 0.05f)) * 0.25f + 1f);

			Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
			spriteBatch.Draw(TextureProjectile, drawPosition, null, Color.Yellow * 0.12f * lightFactor, Projectile.rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * 1.175f, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureProjectile, drawPosition, null, lightColor * 1.5f, Projectile.rotation, TextureProjectile.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureGlow, drawPosition, null, Color.White, Projectile.rotation, TextureGlow.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
			float mult = (float)Math.Sin(TimeSpent * 0.05f) * 0.25f + 0.2f;
			spriteBatch.Draw(TextureGlow, drawPosition, null, Color.White * mult, Projectile.rotation, TextureGlow.Size() * 0.5f, Projectile.scale * 1.4f, SpriteEffects.None, 0f);

			Color color = GenshinElementUtils.GetColor(GenshinElement.GEO);

			for (int i = 0; i < nbDots; i++)
			{
				Vector2 direction = (Vector2.UnitY * Range).RotatedBy(segment * i + rotationBonus);
				Vector2 position = Projectile.Center + direction;
				float rotation = direction.ToRotation();
				Vector2 drawPosition2 = Vector2.Transform(position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
				spriteBatch.Draw(TextureOutline, drawPosition2, null, color * lightFactor * 0.5f, rotation, TextureOutline.Size() * 0.5f, 0.75f * scaleMult2, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureOutline, drawPosition2, null, color * lightFactor * 0.25f, rotation, TextureOutline.Size() * 0.5f, 1.25f * scaleMult2, SpriteEffects.None, 0f);
			}
		}
	}
}
