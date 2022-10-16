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
    public class KleeProjectileSkill : GenshinProjectile
	{
		public int bounces = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Jumpy Dumpty");
		}

		public override Color? GetAlpha(Color lightColor)
		{
			lightColor *= 3f;
			return lightColor;
		}

		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 300;
			Projectile.penetrate = -1;
			Element = Common.GameObjects.CharacterElement.PYRO;
			ElementalParticles = 4;
			ElementApplication = ElementApplicationMedium;
			IgnoreICD = true;
		}

		public override void OnSpawn(IEntitySource source)
        {
		}

        public override void SafeAI()
		{
			Projectile.rotation = Projectile.velocity.Y / 9 * -Projectile.direction;
			if (Projectile.rotation < -0.8f) Projectile.rotation = -0.8f;
			if (Projectile.rotation > 0.8f) Projectile.rotation = 0.8f;
			Projectile.velocity.Y += 0.25f;
			Projectile.spriteDirection = Projectile.direction;

			SpawnDust<KleeSparkleDust>(1f, 1f, 10, 1, 5);
			SpawnDust<KleeSparkleDustBig>(1f, 1f, 10, 1, 20);
			SpawnDust<KleeSparkleDustBigRed>(1f, 1f, 10, 1, 20);
		}

        public override void Kill(int timeLeft)
		{
			int type = ModContent.ProjectileType<KleeExplosionLarge>();
			SpawnProjectile(Projectile.Center + new Vector2(0f, 12f), VelocityImmobile, type, Projectile.damage, Projectile.knockBack);
			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

			int type2 = ModContent.ProjectileType<KleeProjectileSkillBomb>();
			int damage = (int)(Projectile.damage / 15);
			if (damage < 1) damage = 1;
			for (int i = -3; i < 4 ; i ++)
            {
				Vector2 velocity = new Vector2(0f, -1f).RotatedBy(i * 20).RotatedByRandom(2);
				velocity.Y = -1.75f;
				velocity.X *= 1.2f;
				SpawnProjectile(Projectile.Center + new Vector2(0f, 8f), velocity, type2, damage, 0f);
			}

			SpawnDust<KleeSparkleDust>(1f, 1f, 50, 15);
			SpawnDust(DustID.Smoke, 0.75f, 1f, 50, 8);
			SpawnDust<KleeSparkleDustBig>(1f, 1f, 50, 2);
			SpawnDust<KleeSparkleDustBigRed>(1f, 1f, 50, 3);
			SpawnDust<KleeSparkleDustGiant>(1f, 1f, 30);
			SpawnDust<KleeSparkleDustGiantRed>(1f, 1f, 30, 2);
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.velocity.X = -oldVelocity.X * 0.8f;
				return false;
			}

			bounces++;
			if (bounces > 2)
			{
				Projectile.Kill();
			}
			else
			{
				Projectile.velocity.Y = -4f;
				Projectile.velocity.X *= 0.75f;

				int type = ModContent.ProjectileType<KleeExplosionMedium>();
				SpawnProjectile(Projectile.Center + new Vector2(0f, 12f), VelocityImmobile, type, Projectile.damage, Projectile.knockBack);

				for (int i = 0; i < 8; i++) // Dust
				{
					int dustType = ModContent.DustType<KleeSparkleDust>();
					Dust dust = Main.dust[Dust.NewDust(Projectile.position - new Vector2(30, 30), Projectile.width + 60, Projectile.height + 60, dustType)];
					dust.velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
				}

				for (int i = 0; i < 2; i++) // Dust
				{
					int dustType = ModContent.DustType<KleeSparkleDustBigRed>();
					Dust dust = Main.dust[Dust.NewDust(Projectile.position - new Vector2(30, 30), Projectile.width + 60, Projectile.height + 60, dustType)];
					dust.velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
				}

				for (int i = 0; i < 4; i++) // Dust
				{
					Dust dust = Main.dust[Dust.NewDust(Projectile.position - new Vector2(15, 15), Projectile.width + 30, Projectile.height + 30, DustID.Smoke)];
					dust.velocity = new Vector2(Main.rand.NextFloat(-0.75f, 0.75f), Main.rand.NextFloat(-0.75f, 0.75f));
				}

				if (Main.rand.NextBool()) // Dust
				{
					int dustType = ModContent.DustType<KleeSparkleDustGiantRed>();
					Dust dust = Main.dust[Dust.NewDust(Projectile.position - new Vector2(30, 30), Projectile.width + 60, Projectile.height + 60, dustType)];
					dust.velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
				}

				SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
			}
			return false;
		}
    }
}
