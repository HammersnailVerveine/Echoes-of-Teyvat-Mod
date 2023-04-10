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
using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Shields;

namespace GenshinMod.Content.Projectiles
{
    public class ProjectileCrystallize : GenshinProjectile
	{
		public static Texture2D TextureSelf;
		public Color GlowColor;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crytallize");
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
            Projectile.timeLeft = 1050;
			Projectile.scale = 1f;
            ProjectileTrail = true;
			Projectile.alpha = 255;
			Projectile.tileCollide = true;
		}

		public override void OnSpawn(IEntitySource source)
		{
			TextureSelf ??= GetTexture();
			GlowColor = GetColor(GetElement());
		}

        public override void SafeAI()
		{
			Projectile.velocity.Y += 0.1f;
			Projectile.velocity.X *= 0.95f;

			 // shield = 15 sec
			 // Only newer 3 crystals remain
			 // cannot be picked up instantly when it spawns
			foreach (Player player in Main.player)
            {
				if (player.Center.Distance(Projectile.Center) < 32f)
                {
					Projectile.Kill();
					GenshinPlayer genshinPlayer = player.GetModPlayer<GenshinPlayer>();
					GenshinShield shield = new ShieldCrystallize().Initialize((int)Projectile.ai[1], 900, GetElement());
					genshinPlayer.AddShield(shield);
                }
            }
		}

		public override void PostDraw(Color lightColor)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
			float lightFactor = (float)Math.Sin(OwnerGenshinPlayer.Timer * 0.05f) * 0.2f + 0.9f;
			float lightFactorDisappear = Projectile.timeLeft < 60 ? Projectile.timeLeft / 60f : 1f;
			float scaleMult = (((float)Math.Sin(TimeSpent * 0.05f)) * 0.1f + 0.8f);
			float floatOffset = (((float)Math.Sin(TimeSpent * 0.05f)) * 4f + 10f);
			Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
			drawPosition.Y -= floatOffset;
			spriteBatch.Draw(TextureSelf, drawPosition, null, GlowColor * lightFactorDisappear, Projectile.rotation, TextureSelf.Size() * 0.5f, Projectile.scale * 0.6f, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureSelf, drawPosition, null, GlowColor * lightFactor * lightFactorDisappear, Projectile.rotation, TextureSelf.Size() * 0.5f, Projectile.scale * 0.8f * scaleMult, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureSelf, drawPosition, null, GlowColor * 0.4f * lightFactor * lightFactorDisappear, Projectile.rotation, TextureSelf.Size() * 0.5f, Projectile.scale * 1.15f * scaleMult, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureSelf, drawPosition, null, GlowColor * 0.2f * lightFactor * lightFactorDisappear, Projectile.rotation + TimeSpent * 0.05f, TextureSelf.Size() * 0.5f, Projectile.scale * 1.1f * scaleMult, SpriteEffects.None, 0f);
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			//HitGround = true;
			return false;
        }

        public GenshinElement GetElement()
        {
			switch (Projectile.ai[0])
            {
				case 1f:
					return GenshinElement.GEO;
				case 2f:
					return GenshinElement.ANEMO;
				case 3f:
					return GenshinElement.CRYO;
				case 4f:
					return GenshinElement.ELECTRO;
				case 5f:
					return GenshinElement.DENDRO;
				case 6f:
					return GenshinElement.HYDRO;
				case 7f:
					return GenshinElement.PYRO;
				default:
					return GenshinElement.NONE;
            }
        }

		public Color GetColor(GenshinElement element)
		{
			switch (element)
			{
				case GenshinElement.GEO:
					return new Color(255, 167, 45);
				case GenshinElement.ANEMO:
					return new Color(79, 255, 202);
				case GenshinElement.CRYO:
					return new Color(104, 209, 255);
				case GenshinElement.DENDRO:
					return new Color(146, 255, 50);
				case GenshinElement.ELECTRO:
					return new Color(162, 96, 255);
				case GenshinElement.HYDRO:
					return new Color(30, 139, 255);
				case GenshinElement.PYRO:
					return new Color(255, 116, 61);
				default:
					return new Color(215, 215, 255);
			}
		}
	}
}
