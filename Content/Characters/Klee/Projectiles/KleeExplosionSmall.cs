using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;

namespace GenshinMod.Content.Characters.Klee.Projectiles
{
    public class KleeExplosionSmall : GenshinProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Klee Explosion");
		}

		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 20;
			Projectile.penetrate = -1;
			Main.projFrames[Projectile.type] = 5;
		}

        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2f);
			ResetImmunity();
		}

        public override void SafeAI()
		{
			Projectile.friendly = FirstFrame;
			Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.2f);
			if (timeSpent % 2 == 0)
			{
				Projectile.frame++;
				if (Projectile.frame > 4)
					Projectile.Kill();
			}
		}
    }
}
