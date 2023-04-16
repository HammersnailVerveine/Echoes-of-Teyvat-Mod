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
		public static Texture2D texture;
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
			Projectile.tileCollide = true;
			Projectile.aiStyle = 0;
            Projectile.timeLeft = 600;
			Projectile.scale = 1f;
            ProjectileTrail = true;
		}

		public override void OnSpawn(IEntitySource source)
		{
			texture ??= GetTexture();
			Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2f);
			ColorGeo = GenshinElementUtils.GetColor(GenshinElement.GEO);
		}

        public override void SafeAI()
		{
			Projectile.rotation += 0.1f;
		}

		public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
		{
			Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
			spriteBatch.Draw(texture, drawPosition, null, Color.White * 0.8f, -Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.8f, SpriteEffects.None, 0f);
		}
	}
}
