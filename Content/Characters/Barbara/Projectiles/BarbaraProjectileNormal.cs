using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Barbara.Projectiles
{
    public class BarbaraProjectileNormal : GenshinProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Barbara Splash");
		}

		public override void SetDefaults()
		{
			Projectile.width = 34;
			Projectile.height = 34;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 40;
			Projectile.penetrate = -1;
			Main.projFrames[Projectile.type] = 3;
			AttackWeight = AttackWeight.LIGHT;
		}

        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White * 1.2f * (1.4f - Projectile.scale);

		}

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat(-(float)Math.PI / 4f, (float)Math.PI / 4f);
			Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1; 
			Projectile.frame = Main.rand.Next(3);
			Projectile.scale = 0.2f;
			ResetImmunity();

			SpawnDust<BarbaraDustBubble>(0.5f, 1f, 10, 7);
			SpawnDust<BarbaraDustStar>(0.2f, 1f, 0, 5);
			SpawnDust<BarbaraDustStarBig>(0.1f, 1f, 0, 2);

			Vector2 direction = Projectile.Center - Owner.Center;
			if (direction.Length() > 30f)
			{
				direction.Normalize();
				direction *= 3f;
				Vector2 position = Owner.Center + direction;

				SpawnDust<BarbaraDustBubble>(position, direction, 0.5f, 1f, 10, 4);
				SpawnDust<BarbaraDustStar>(position, direction * 0.5f, 0.2f, 1f, 10, 3);
				SpawnDust<BarbaraDustStar>(position, Vector2.Zero, 0.2f, 1f, 10, 2);
			}
		}

        public override void OnFirstHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (OwnerCharacter is CharacterBarbara barbara)
			{
				if (barbara.skillActive)
				{
					foreach (GenshinCharacter character in OwnerGenshinPlayer.CharacterTeam)
						character.Heal(barbara.AbilitySkill.GetScaling2());
				}
			}
		}

        public override void SafeAI()
		{
			Projectile.friendly = FirstFrame;
			Lighting.AddLight(Projectile.Center, 0.1f, 0.1f, 0.2f);
			Projectile.scale *= 1.085f;
		}
    }
}
