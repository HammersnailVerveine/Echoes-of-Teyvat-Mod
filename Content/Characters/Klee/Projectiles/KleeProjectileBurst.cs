﻿using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Klee.Projectiles
{
    public class KleeProjectileBurst : GenshinProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Klee Blast");
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

        public override void OnFirstFrame()
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

        public override void OnKill(int timeLeft)
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
