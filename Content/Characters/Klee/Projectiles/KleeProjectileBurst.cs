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
    public class KleeProjectileBurst : GenshinProjectile
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
			Projectile.width = 22;
			Projectile.height = 6;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 20;
			ProjectileTrail = true;
			CanReact = false;
			CanDealDamage = false;
		}

		public override void OnSpawn(IEntitySource source)
		{
			SpawnDust<KleeSparkleDust>(Projectile.position, Projectile.velocity * 2, 0f, 1.5f, 8, 3);
		}

        public override void SafeAI()
		{
			Projectile.tileCollide = TimeSpent > 6;
			Projectile.velocity *= 1.1f;
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.direction = Projectile.spriteDirection;
			Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.2f);
		}

        public override void Kill(int timeLeft)
        {
			int type = ModContent.ProjectileType<KleeExplosionMedium>();
			SpawnProjectile(Projectile.Center, VelocityImmobile, type, Projectile.damage, Projectile.knockBack);
			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

			SpawnDust<KleeSparkleDust>(1f, 1f, 50, 6);
			SpawnDust(DustID.Smoke, 0.75f, 1f, 30, 4);
			SpawnDust<KleeSparkleDustBig>(1f, 1f, 50, 2);
			SpawnDust<KleeSparkleDustBigRed>(1f, 1f, 50, 2);
		}
	}
}
