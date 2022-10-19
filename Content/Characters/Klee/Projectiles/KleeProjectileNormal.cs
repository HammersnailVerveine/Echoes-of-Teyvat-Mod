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
    public class KleeProjectileNormal : GenshinProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dodoco Bomb");
		}

		public override Color? GetAlpha(Color lightColor)
		{
			lightColor *= 3f;
			return lightColor;
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 180;
			Main.projFrames[Projectile.type] = 2;
			Element = GenshinElement.PYRO;
			CanReact = false;
		}

		public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2f);
			Projectile.frame = Main.rand.Next(2);
			Projectile.velocity *= 0.8f + Main.rand.NextFloat(0.2f);
			Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(10));

			SpawnDust<KleeSparkleDust>(Projectile.position, Projectile.velocity * 0.2f, 1f, 1f, Projectile.width, 3);
			SpawnDust(DustID.Smoke, Projectile.position, Projectile.velocity * 0.1f, 1f, 1f, Projectile.width, 3);
		}

        public override void SafeAI()
		{
			if (timeSpent > 8)
			{
				Projectile.velocity.Y += 0.25f;
				Projectile.velocity.X *= 0.97f;
			}

			SpawnDust<KleeSparkleDust>(Projectile.Center, Projectile.velocity * 0.2f, 1f, 0.75f, 0, 1, 3);
			SpawnDust<KleeSparkleDustBig>(Projectile.position, Projectile.velocity * 0.33f, 0.5f, 0.75f, Projectile.width, 1, 45);
			
			Projectile.spriteDirection = Projectile.direction;
			Projectile.rotation += Projectile.velocity.Length() / 75f * Projectile.direction;
		}

        public override void Kill(int timeLeft)
		{
			if (OwnerCharacter is CharacterKlee klee)
			{
				int type = ModContent.ProjectileType<KleeExplosionMedium>();
				SpawnProjectile(Projectile.Center, VelocityImmobile, type, klee.AbilityNormal.GetScaling2(), Projectile.knockBack);
			}

			SpawnDust<KleeSparkleDust>(1f, 1f, 30, 8);
			SpawnDust(DustID.Smoke, 1f, 1f, 15, 4);
			SpawnDust<KleeSparkleDustBigRed>(0.75f, 1f, 30, 2);
			SpawnDust<KleeSparkleDustGiantRed>(1f, 1f, 30, 1, 2);

			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
		}
    }
}
