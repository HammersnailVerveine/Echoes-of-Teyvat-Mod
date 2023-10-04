using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Klee.Projectiles
{
    public class KleeProjectileSkillBomb : GenshinProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dodoco mine");
        }

        public override Color? GetAlpha(Color lightColor)
        {
            lightColor *= 3f;
            return lightColor;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 900; // 15 sec
            Main.projFrames[Projectile.type] = 8;
            CanReact = false;
            CanDealDamage = false;
        }

        public override void OnFirstFrame()
        {
            Projectile.frame = Main.rand.Next(8);
            Projectile.timeLeft -= Main.rand.Next(60);
            if (IsLocalOwner) Projectile.netUpdate = true;
        }

        public override void SafeAI()
        {
            Projectile.velocity.Y += 0.10f;
            Projectile.velocity.X *= 0.975f;

            Projectile.friendly = TimeSpent > 60 + Projectile.ai[0];

            if (TimeSpent % 6 == 0)
            {
                Projectile.frame++;
                if (Projectile.frame > 7) Projectile.frame = 0;
            }

            SpawnDust<KleeSparkleDust>(0.5f, 0.5f, 10, 1, 60);
            SpawnDust<KleeSparkleDustBigRed>(0.5f, 1f, 10, 1, 300);
        }

        public override void OnKill(int timeLeft)
        {
            int type = ModContent.ProjectileType<KleeExplosionSmall>();
            SpawnProjectile(Projectile.Center, VelocityImmobile, type, Projectile.damage * 5, Projectile.knockBack);

            SpawnDust<KleeSparkleDust>(0.75f, 1f, 10, 6);
            SpawnDust(DustID.Smoke, 0.5f, 1f, 5, 3);

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y *= 0f;
            return false;
        }
    }
}
