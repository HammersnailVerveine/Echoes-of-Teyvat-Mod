using Microsoft.Xna.Framework;
using GenshinMod.Content.Dusts;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.GameObjects.Enums;

namespace GenshinMod.Content.Characters.Klee.Projectiles
{
    public class KleeProjectileCharged : GenshinProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Klee Blast");
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 30;
			ProjectileTrail = true;
			CanReact = false;
		}

		public override void OnSpawn(IEntitySource source)
		{
			for (int i = 0; i < 5; i++) // Dust
			{
				int dustType = ModContent.DustType<KleeSparkleDust>();
				Dust dust = Main.dust[Dust.NewDust(Projectile.position - new Vector2(8, 8), Projectile.width + 16, Projectile.height + 16, dustType)];
				dust.velocity = Projectile.velocity * 2;
				dust.scale *= 1.25f;
			}
		}

        public override void SafeAI()
		{
			Projectile.velocity *= 1.08f;
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.direction = Projectile.spriteDirection;
			Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 0.3f);
		}

        public override void Kill(int timeLeft)
		{
			int type = ModContent.ProjectileType<KleeExplosionLarge>();
			SpawnProjectile(Projectile.Center, VelocityImmobile, type, OwnerCharacter.AbilityCharged.GetScaling2(), Projectile.knockBack);
			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

			SpawnDust<KleeSparkleDust>(1f, 1f, 50, 15);
			SpawnDust(DustID.Smoke, 0.75f, 1f, 30, 8);
			SpawnDust<KleeSparkleDustBig>(1f, 1f, 50, 2);
			SpawnDust<KleeSparkleDustBigRed>(1f, 1f, 50, 3);
			SpawnDust<KleeSparkleDustGiant>(1f, 1f, 30);
			SpawnDust<KleeSparkleDustGiantRed>(1f, 1f, 30, 2);
		}
	}
}
