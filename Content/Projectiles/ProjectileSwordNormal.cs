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

namespace GenshinMod.Content.Projectiles
{
    public class ProjectileSwordNormal : GenshinProjectile
	{
		public Texture2D WeaponTexture;
		public GenshinWeapon Weapon;
		public float acceleration = 0.8f;
		//public float acceleration = 1f;

		public List<Vector2> OldPosition;
		public List<float> OldRotation;
		public List<int> HitNPC;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sword Slash");
		}

		public override void SetDefaults()
		{
			Projectile.width = 52;
			Projectile.height = 52;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
            Projectile.timeLeft = 30;
			Projectile.scale = 1f;
            ProjectileTrail = true;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
		}

		public override void OnSpawn(IEntitySource source)
		{
			Weapon = Owner.GetModPlayer<GenshinPlayer>().CharacterCurrent.Weapon;
			WeaponTexture = ModContent.Request<Texture2D>(Weapon.Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			Projectile.width = WeaponTexture.Width;
			Projectile.height = WeaponTexture.Height;
			OldPosition = new List<Vector2>();
			OldRotation = new List<float>();
			HitNPC = new List<int>();
		}

        public override void SafeAI()
		{
			Vector2 position = Owner.Center + (Vector2.UnitY * TileLength * 4f).RotatedBy(MathHelper.ToRadians(Projectile.ai[0])) - Projectile.Size * 0.5f;
			Projectile.position = position;

			Vector2 direction = Projectile.Center - Owner.Center;
			Projectile.rotation = direction.ToRotation() + MathHelper.ToRadians(45f);

			Projectile.ai[0] += Projectile.ai[1] * acceleration;
			if (timeSpent > 17) acceleration *= 0.7f;
			if (timeSpent < 4) acceleration *= 2.35f;

			// Afterimages
			if (timeSpent < 30)
			{
				OldPosition.Add(Projectile.Center);
				OldRotation.Add(Projectile.rotation); 
				if (OldPosition.Count > 8)
				{
					OldPosition.RemoveAt(0);
					OldRotation.RemoveAt(0);
				}
			}
			else if (OldPosition.Count > 1)
			{
				OldPosition.RemoveAt(0);
				OldRotation.RemoveAt(0);
			}
		}

        public override void SafeOnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			HitNPC.Add(target.whoAmI);
		}


        public override bool? CanHitNPC(NPC target)
        {
			if (HitNPC.Contains(target.whoAmI)) return false;
			return base.CanHitNPC(target);
        }

        public override void PostDraw(Color lightColor)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
			Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
			spriteBatch.Draw(WeaponTexture, drawPosition, null, lightColor * 1.5f, Projectile.rotation, WeaponTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);


			for (int i = 0; i < OldPosition.Count; i ++)
			{
				Vector2 drawPosition2 = Vector2.Transform(OldPosition[i] - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
				spriteBatch.Draw(WeaponTexture, drawPosition2, null, lightColor * 0.075f * i, OldRotation[i], WeaponTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
			}
		}
	}
}
