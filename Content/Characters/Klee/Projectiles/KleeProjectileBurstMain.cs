using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Klee.Projectiles
{
    public class KleeProjectileBurstMain : GenshinProjectile
    {
        NPC enemyTarget;

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
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 45;
            Projectile.scale = 1f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0f;
            enemyTarget = Main.npc[(int)Projectile.ai[0]];
        }

        public override void SafeAI()
        {
            Lighting.AddLight(Projectile.Center, 0.1f, 0.1f, 0.1f);
            if (TimeSpent < 15) Projectile.scale += (1f / 15);

            if (TimeSpent < 30)
            {
                Vector2 direction = enemyTarget.Center - Projectile.Center;
                Projectile.rotation = direction.ToRotation();
            }
            else if (TimeSpent == 30)
            {
                Vector2 target = enemyTarget.Center;
                Vector2 velocity = (target - Projectile.Center);
                velocity.Normalize();
                velocity *= 5f;
                Vector2 position = Projectile.Center + velocity * 2;
                int type = ModContent.ProjectileType<KleeProjectileBurst>();
                SpawnProjectile(position, velocity, type, Projectile.damage, Projectile.knockBack);

                SpawnDust<KleeSparkleDust>(1f, 1f, 30, 5);
                SpawnDust<KleeSparkleDustBig>(1f, 1f, 10);
                SpawnDust<KleeSparkleDustBigRed>(1f, 1f, 10);
                SpawnDust<KleeSparkleDustGiantRed>(1f, 1f, 10, 1, 2);
                SpawnDust<KleeSparkleDustGiant>(1f, 1f, 10, 1, 2);
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
