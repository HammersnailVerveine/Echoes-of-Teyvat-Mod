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
using GenshinMod.Common.GlobalObjets;

namespace GenshinMod.Content.Characters.Albedo.Projectiles
{
    public class AlbedoProjectileSkillMain : GenshinProjectile
	{
        public static float Range = 256f;
        public static Texture2D TextureProjectile;
		public static Texture2D TextureGlow;
		public static Texture2D TextureOutline;
		public static Texture2D TextureBlast;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Solar Isotoma");
		}

		public override void SetDefaults()
		{
			Projectile.width = 256;
			Projectile.height = 256;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
            Projectile.timeLeft = 60 * 30;
			Projectile.scale = 1f;
            ProjectileTrail = true;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
			IgnoreICD = true;
			FirstFrameDamage = true;
			ElementalParticles = 1;
			AttackWeight = AttackWeight.BLUNT;
		}

		public override void OnSpawn(IEntitySource source)
		{
			TextureProjectile = GetTexture();
			TextureGlow ??= ModContent.Request<Texture2D>(Texture + "_GlowMask", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			TextureOutline ??= ModContent.Request<Texture2D>(Texture + "_Outline", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			TextureBlast ??= ModContent.Request<Texture2D>(Texture + "_Blast", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		}

        public override void SafeAI()
		{
			Projectile.rotation += 0.01f;
			if (OwnerCharacter is CharacterAlbedo albedo)
				albedo.skillActive = true;

			Projectile.friendly = FirstFrame;
		}

		public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
		{
			int nbDots = 150;
			float segment = (MathHelper.TwoPi / nbDots);
			float rotationBonus = 0.005f * OwnerGenshinPlayer.Timer;
			float lightFactor = ((float)Math.Sin(OwnerGenshinPlayer.Timer * 0.25f) + 1.5f) * 0.25f;
			float scalemult = (TimeSpent < 15) ? (TimeSpent / 15f) : 1f;
			float scaleMult2 = (((float)Math.Sin(TimeSpent * 0.05f)) * 0.25f + 1f);
			float fadeout = Projectile.timeLeft > 60 ? 1f : Projectile.timeLeft / 60f;

			Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
			spriteBatch.Draw(TextureProjectile, drawPosition, null, Color.Yellow * 0.12f * lightFactor * fadeout, Projectile.rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * scalemult * 1.175f, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureProjectile, drawPosition, null, lightColor * 1.5f * fadeout, Projectile.rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * scalemult, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureGlow, drawPosition, null, Color.White * fadeout, Projectile.rotation, TextureGlow.Size() * 0.5f, Projectile.scale * scalemult, SpriteEffects.None, 0f);
			float mult = (float)Math.Sin(TimeSpent * 0.05f) * 0.25f + 0.2f;
			spriteBatch.Draw(TextureGlow, drawPosition, null, Color.White * mult * fadeout, Projectile.rotation, TextureGlow.Size() * 0.5f, Projectile.scale * scalemult * 1.4f, SpriteEffects.None, 0f);

			Color color = GenshinElementUtils.GetColor(GenshinElement.GEO);

			for (int i = 0; i < nbDots; i++)
			{
				Vector2 direction = (Vector2.UnitY * Range).RotatedBy(segment * i + rotationBonus);
				Vector2 position = Projectile.Center + direction;
				float rotation = direction.ToRotation();
				Vector2 drawPosition2 = Vector2.Transform(position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
				spriteBatch.Draw(TextureOutline, drawPosition2, null, color * lightFactor * 0.5f * fadeout, rotation, TextureOutline.Size() * 0.5f, 0.85f * scaleMult2, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureOutline, drawPosition2, null, color * lightFactor * 0.25f * fadeout, rotation, TextureOutline.Size() * 0.5f, 1.35f * scaleMult2, SpriteEffects.None, 0f);
			}

			if (TimeSpent < 45)
            {
				spriteBatch.Draw(TextureBlast, drawPosition, null, color * (1f - (TimeSpent / 45f)), Projectile.rotation * Projectile.spriteDirection + ((float)Math.PI / 8f), TextureBlast.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureBlast, drawPosition, null, color * (1f - (TimeSpent / 45f)) * 0.75f, Projectile.rotation * Projectile.spriteDirection, TextureBlast.Size() * 0.5f, Projectile.scale * 1.75f, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureBlast, drawPosition, null, color * (1f - (TimeSpent / 45f)) * 0.25f, Projectile.rotation * Projectile.spriteDirection + ((float)Math.PI / 8f), TextureBlast.Size() * 0.5f, Projectile.scale * 2.25f, SpriteEffects.None, 0f);

				spriteBatch.Draw(TextureBlast, drawPosition, null, color * (1f - (TimeSpent / 45f)) * 0.5f, Projectile.rotation * Projectile.spriteDirection, TextureBlast.Size() * 0.5f, Projectile.scale * (TimeSpent / 20f), SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureBlast, drawPosition, null, color * (1f - (TimeSpent / 45f)) * 0.3f, Projectile.rotation * Projectile.spriteDirection + ((float)Math.PI / 8f), TextureBlast.Size() * 0.5f, Projectile.scale * 1.75f * (TimeSpent / 20f), SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureBlast, drawPosition, null, color * (1f - (TimeSpent / 45f)) * 0.1f, Projectile.rotation * Projectile.spriteDirection, TextureBlast.Size() * 0.5f, Projectile.scale * 2.25f * (TimeSpent / 20f), SpriteEffects.None, 0f);
			}
		}
	}
}
