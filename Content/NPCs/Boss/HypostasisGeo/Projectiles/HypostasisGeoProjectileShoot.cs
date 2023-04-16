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
    public class HypostasisGeoProjectileShoot : GenshinProjectile
	{
		public static Texture2D TextureProj;
		public static Texture2D TextureProjAdd;
		public Color ColorGeo;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hypostasis Blast");
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
			Projectile.aiStyle = 0;
            Projectile.timeLeft = 180;
			Projectile.scale = 1f;
			PostDrawAdditive = true;
			Projectile.alpha = 255;
			Element = GenshinElement.GEO;
		}

		public override void OnSpawn(IEntitySource source)
		{
			TextureProj ??= GetTexture();
			TextureProjAdd ??= ModContent.Request<Texture2D>("GenshinMod/Content/NPCs/Boss/HypostasisGeo/Projectiles/HypostasisGeoProjectileShoot_Add", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2f);
			ColorGeo = GenshinElementUtils.GetColor(GenshinElement.GEO);
		}

        public override void SafeAI()
		{
			Projectile.rotation += 0.1f;

			for (int length = Projectile.oldPos.Length - 1; length > 0; length--)
			{
				Projectile.oldPos[length] = Projectile.oldPos[length - 1];
			}
			Projectile.oldPos[0] = Projectile.position;

			SpawnDust<HypostasisGeoDustSmall>(Projectile.Center, Projectile.velocity, Main.rand.NextFloat(2.5f) + 2.5f, 1f, 16, 1, 25);
		}

        public override void Kill(int timeLeft)
		{
			SpawnDust<HypostasisGeoDust>(Projectile.Center, Projectile.velocity, Main.rand.NextFloat(2.5f) + 2.5f, 1f, 16, 1);
			SpawnDust<HypostasisGeoDustSmall>(Projectile.Center, Projectile.velocity, Main.rand.NextFloat(2.5f) + 2.5f, 1f, 16, 2);
			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
		}

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
		{
			Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
			spriteBatch.Draw(TextureProj, drawPosition, null, ColorGeo, - Projectile.rotation, TextureProj.Size() * 0.5f, Projectile.scale * 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureProj, drawPosition, null, ColorGeo, - Projectile.rotation + MathHelper.Pi / 4f, TextureProj.Size() * 0.5f, Projectile.scale * 1f, SpriteEffects.None, 0f);
		}

        public override void SafePostDrawAdditive(Color lightColor, SpriteBatch spriteBatch)
		{
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPosition = Vector2.Transform(Projectile.oldPos[k] - Main.screenPosition + new Vector2(Projectile.width / 2f, Projectile.height / 2f + Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
				spriteBatch.Draw(TextureProjAdd, drawPosition, null, ColorGeo * (0.75f / (k + 1)), Projectile.rotation, TextureProjAdd.Size() * 0.5f, Projectile.scale * 0.8f - (0.075f * k), SpriteEffects.None, 0.3f);
			}
		}
    }
}
