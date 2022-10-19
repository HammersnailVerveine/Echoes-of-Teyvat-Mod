using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Common.ModObjects.Weapons.Projectiles
{
	public class CatalystAnchor : GenshinProjectile
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

			Projectile.rotation = Projectile.velocity.X * 0.035f;
			Projectile.rotation = Projectile.rotation > 0.35f ? 0.35f : Projectile.rotation;
			Projectile.rotation = Projectile.rotation < -0.35f ? -0.35f : Projectile.rotation;
			Projectile.spriteDirection = owner.direction;

			Vector2 targetPosition = owner.Center - new Vector2(32f * owner.direction, 16f);
			Vector2 direction = targetPosition - Projectile.Center;
			Projectile.velocity = direction / 12f;
			Projectile.timeLeft = 30;
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 3; i++)
			{
				Main.dust[Dust.NewDust(Projectile.Center, 0, 0, DustID.Smoke)].velocity *= 0.25f;
			}
		}

		public override bool? CanCutTiles() => false;
		public override bool? CanDamage() => null;

		public override bool SafePreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			var position = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
			var effect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(WeaponTexture, position, null, lightColor, Projectile.rotation, WeaponTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
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
				this.WeaponTexture = ModContent.Request<Texture2D>(weapon.CombatTexture).Value;
				this.Weapon = weapon;
			}
			else Projectile.Kill();
		}
	}
}