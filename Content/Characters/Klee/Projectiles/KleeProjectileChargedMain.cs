﻿using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Klee.Projectiles
{
    public class KleeProjectileChargedMain : GenshinProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Klee Charged Flower");
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
            Projectile.scale = 0f;
        }

        public override void SafeAI()
        {
            Projectile.velocity *= 0.9f;
            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 0.3f);
            Projectile.tileCollide = TimeSpent > 2;
            if (TimeSpent < 30) Projectile.scale += (1f / 30);

            if (TimeSpent < 60)
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
                if (TimeSpent > 55) Projectile.scale += (1f / 15);
            }
            else if (TimeSpent == 60)
            {
                Vector2 target = Main.MouseWorld;
                Vector2 velocity = (target - Projectile.Center);
                velocity.Normalize();
                velocity *= 3f;
                Vector2 position = Projectile.Center + velocity * 2;
                int type = ModContent.ProjectileType<KleeProjectileCharged>();
                SpawnProjectile(position, velocity, type, Projectile.damage, Projectile.knockBack);

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
