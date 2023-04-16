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
    public class BarbaraProjectileSkillDroplet : GenshinProjectile
	{
		public static Texture2D texture;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Barbara Droplet");
		}

		public override void SetDefaults()
		{
			Projectile.width = 116;
			Projectile.height = 116;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 100;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			AttackWeight = AttackWeight.LIGHT;
		}

        public override void OnSpawn(IEntitySource source)
        {
			texture ??= ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		}

        public override void SafeAI()
		{
			Projectile.position = Owner.Center - new Vector2(Projectile.width / 2, Projectile.height / 2);
			Projectile.rotation -= 0.1f;
			if (TimeSpent > 60)
			{
				Projectile.rotation -= 0.05f;
				Projectile.scale *= 0.975f;
				Projectile.friendly = false;
			}

			if (TimeSpent == 60)
            {
				int type = ModContent.ProjectileType<BarbaraProjectileSkillCircle>();
				SpawnProjectile(Owner.Center, VelocityImmobile, type, OwnerCharacter.AbilitySkill.GetScaling3(), 0f, 0f);
				Projectile.friendly = true;
			}

			if (Main.rand.NextBool(3)) // Dust special case
			{
				int dustType = ModContent.DustType<BarbaraDustBubble>();
				Dust dust = Main.dust[Dust.NewDust(Projectile.Center - new Vector2(Projectile.height / 2,Projectile.height / 2) * Projectile.scale, (int)(Projectile.height * Projectile.scale), (int)(Projectile.height * Projectile.scale), dustType)];
				dust.velocity = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f));
			}

			if (Main.rand.NextBool(10)) // Dust special case
			{
				int dustType = ModContent.DustType<BarbaraDustStar>();
				Dust dust = Main.dust[Dust.NewDust(Projectile.Center - new Vector2(Projectile.height / 3, Projectile.height / 3) * Projectile.scale, (int)(Projectile.height * Projectile.scale * 0.66), (int)(Projectile.height * Projectile.scale * 0.66), dustType)];
				dust.velocity = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.25f, 0.25f));
			}

			if (Main.rand.NextBool(20)) // Dust special case
			{
				int dustType = ModContent.DustType<BarbaraDustStarBig>();
				Dust dust = Main.dust[Dust.NewDust(Projectile.Center - new Vector2(Projectile.height / 3, Projectile.height / 3) * Projectile.scale, (int)(Projectile.height * Projectile.scale * 0.66), (int)(Projectile.height * Projectile.scale * 0.66), dustType)];
				dust.velocity = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.25f, 0.25f));
			}
		}

		public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
		{
			Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
			Color color = Color.White * 0.4f * Projectile.scale;
			spriteBatch.Draw(texture, drawPosition, null, color, Projectile.rotation + (MathHelper.TwoPi / 3), texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(texture, drawPosition, null, color, Projectile.rotation + (MathHelper.TwoPi / 3) * 2, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(texture, drawPosition, null, color, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
		}
	}
}
