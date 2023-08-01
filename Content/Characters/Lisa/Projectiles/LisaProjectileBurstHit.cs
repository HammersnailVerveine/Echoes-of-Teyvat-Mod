using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace GenshinMod.Content.Characters.Lisa.Projectiles
{
    public class LisaProjectileBurstHit : GenshinProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lamp Zap");
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 2;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            AttackWeight = AttackWeight.LIGHT;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 direction = (Vector2.UnitY * Main.rand.NextFloat(5f, 10f)).RotatedByRandom(MathHelper.ToRadians(360));
                SpawnDust<LisaDustRound>(Projectile.Center, direction, 1f, 1f, 0);
            }
            if (Projectile.ai[0] == 1f) IgnoreICD = true;
        }
    }
}
