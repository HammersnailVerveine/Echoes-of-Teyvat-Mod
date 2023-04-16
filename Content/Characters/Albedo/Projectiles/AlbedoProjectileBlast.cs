using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using GenshinMod.Common.ModObjects;
using System;
using Terraria.Audio;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using GenshinMod.Common.GameObjects.Enums;

namespace GenshinMod.Content.Characters.Albedo.Projectiles
{
    public class AlbedoProjectileBlast : GenshinProjectile
	{
		public static Texture2D TextureSelf;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Solar Isotoma");
		}

		public override void SetDefaults()
		{
			Projectile.width = 64;
			Projectile.height = 64;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 20;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
			ElementalParticles = 1;
			ElementalParticleChance = 67;
			AttackWeight = AttackWeight.BLUNT;
		}

		public override void SafeAI()
		{
			Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.1f);
			Projectile.rotation += 0.115f * Projectile.spriteDirection;
			Projectile.scale *= 1.1f;
			if (FirstFrame)
            {
				Projectile.friendly = true;
				ResetImmunity();
				SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
			}
			else
				Projectile.friendly = false;
		}

		public override void OnSpawn(IEntitySource source)
		{
			TextureSelf ??= ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			Projectile.rotation = Main.rand.NextFloat(-(float)Math.PI / 4f, (float)Math.PI / 4f);
			Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;
			Projectile.scale = 0.2f;

			if (Projectile.ai[0] != 0f) ElementalParticles = 0;
		}

		public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
		{
			Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
			Color color = GenshinElementUtils.GetColor(GenshinElement.GEO);
			spriteBatch.Draw(TextureSelf, drawPosition, null, color * (1.5f - Projectile.scale), Projectile.rotation * Projectile.spriteDirection + ((float)Math.PI / 8f), TextureSelf.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureSelf, drawPosition, null, color * (1.2f - Projectile.scale), Projectile.rotation * Projectile.spriteDirection, TextureSelf.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureSelf, drawPosition, null, color * (1.2f - Projectile.scale) * 0.15f, Projectile.rotation * Projectile.spriteDirection + ((float)Math.PI / 8f), TextureSelf.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0f);
		}
	}
}
