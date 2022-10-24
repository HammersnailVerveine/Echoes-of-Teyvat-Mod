using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Common.ModObjects.Weapons.Projectiles
{
	public class WeaponAnchor : GenshinProjectile
	{
		public Texture2D WeaponTexture { get; set; }
		public GenshinWeapon Weapon { get; set; }

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 60;
			Projectile.penetrate = -1;
			Projectile.netImportant = true;
			Projectile.alpha = 255;
		}

		public void OnChangeEquipedItem(GenshinWeapon weapon)
		{
			if (!ModContent.HasAsset(weapon.CombatTexture))
			{
				Projectile.Kill();
				return;
			}
			Weapon = weapon;
			WeaponTexture = ModContent.Request<Texture2D>(weapon.CombatTexture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			Projectile.netUpdate = true;
		}

		public override void SafeAI()
		{
			var owner = Main.player[Projectile.owner];

			if (!owner.active || owner.dead || Weapon == null)
			{
				Projectile.Kill();
				return;
			}

			bool isCatalyst = Weapon is WeaponCatalyst;
			Vector2 targetPosition = owner.Center - new Vector2((isCatalyst ? 32f : 24f) * owner.direction, (isCatalyst ? 16f : 12f));
			Vector2 direction = targetPosition - Projectile.Center;
			float speed = isCatalyst ? 12f : 6f;
			Projectile.velocity = direction / speed;
			Projectile.timeLeft = 30;
			Projectile.spriteDirection = owner.direction;

			Projectile.rotation = Projectile.velocity.X * 0.035f;
			Projectile.rotation = Projectile.rotation > 0.35f ? 0.35f : Projectile.rotation;
			Projectile.rotation = Projectile.rotation < -0.35f ? -0.35f : Projectile.rotation;

			float len = Projectile.velocity.Length();
			if (len < 1.5f)
			{
				Projectile.rotation += (float)(Math.Sin(TimeSpent * 0.04f) / 15f) * (1.5f - len);
			}
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 3; i++)
			{
				Main.dust[Dust.NewDust(Projectile.Center, 0, 0, DustID.Smoke)].velocity *= 0.25f;
			}
			Projectile.active = false;
		}

		public override bool? CanCutTiles() => false;
		public override bool? CanDamage() => null;

		public override bool SafePreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			float lightmult = TimeSpent > 30 ? 1f : 1f * (TimeSpent / 30f);
			var position = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
			var effect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(WeaponTexture, position, null, lightColor * lightmult, Projectile.rotation, WeaponTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
			return false;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(Weapon.Type);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			int type = reader.ReadInt32();
			Item item = new Item();
			item.SetDefaults(type);
			if (item.ModItem is GenshinWeapon weapon)
			{
				WeaponTexture = ModContent.Request<Texture2D>(weapon.CombatTexture).Value;
				Weapon = weapon;
			}
			else Projectile.Kill();
		}
	}
}