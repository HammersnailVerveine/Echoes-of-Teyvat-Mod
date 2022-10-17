using Microsoft.Xna.Framework;
using GenshinMod.Content.Dusts;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using GenshinMod.Common.ModObjects;

namespace GenshinMod.Content.Characters.Klee.Projectiles
{
    public class KleeProjectileChargedMain : GenshinProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Klee Charged Flower");
		}

		public override Color? GetAlpha(Color lightColor)
		{
			lightColor *= 3f;
			return lightColor;
		}

		public override void SetDefaults()
		{
			Projectile.width = 42;
			Projectile.height = 42;
			Projectile.friendly = false;
			Projectile.tileCollide = true;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 75;
			Projectile.scale = 1f;
			Element = Common.GameObjects.GenshinElement.PYRO;
		}

		public override void OnSpawn(IEntitySource source)
        {
			Projectile.scale = 0f;
		}

        public override void SafeAI()
		{
			Projectile.velocity *= 0.9f;
			Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 0.3f);
			Projectile.tileCollide = timeSpent > 2;
			if (timeSpent < 30) Projectile.scale += (1f / 30);

			if (timeSpent < 60)
			{
				if (IsLocalOwner)
				{
					Vector2 target = Main.MouseWorld;
					Vector2 direction = target - Projectile.Center;
					Projectile.rotation = direction.ToRotation();
					if (Math.Abs(Projectile.rotation - Projectile.ai[0]) > 0.4f) // Rotation sync from owner
					{
						Projectile.ai[0] = Projectile.rotation;
						Projectile.netUpdate = true;
					}
				}
				else Projectile.rotation = Projectile.ai[0]; // Rotation sync on other clients
				Projectile.direction = Projectile.spriteDirection;
				if (timeSpent > 55) Projectile.scale += (1f / 15);
			}
			else if (timeSpent == 60)
			{
				Vector2 target = Main.MouseWorld;
				Vector2 velocity = (target - Projectile.Center);
				velocity.Normalize();
				velocity *= 3f;
				Vector2 position = Projectile.Center + velocity * 2;
				int type = ModContent.ProjectileType<KleeProjectileCharged>();
				int damage = (int)(Projectile.damage / 5);
				if (damage < 1) damage = 1;
				SpawnProjectile(position, velocity, type, damage, Projectile.knockBack);

				SpawnDust<KleeSparkleDust>(1f, 1f, 30, 10);
				SpawnDust<KleeSparkleDustBig>(1f, 1f, 10, 2);
				SpawnDust<KleeSparkleDustBigRed>(1f, 1f, 10, 2);
				SpawnDust<KleeSparkleDustGiant>(1f, 1f, 10);
				SpawnDust<KleeSparkleDustGiantRed>(1f, 1f, 10, 2);
			}
			else Projectile.scale *= 0.8f;

			SpawnDust<KleeSparkleDust>(1f, 1f, 10, 1, 5);
			SpawnDust<KleeSparkleDustBigRed>(1f, 1f, 10, 1, 15);
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			Projectile.velocity *= 0f;
			return false;
		}
    }
}
