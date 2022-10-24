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
    public class KleeProjectileBurstFollow : GenshinProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Clover");
		}

		public override Color? GetAlpha(Color lightColor)
		{
			lightColor *= 2f * (1f - (Projectile.alpha / 255f));
			return lightColor;
		}

		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 601;
		}

		public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2f);
			Projectile.frame = Main.rand.Next(2);
			Projectile.velocity *= 0.8f + Main.rand.NextFloat(0.2f);
			Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(10));
		}

        public override void SafeAI()
		{
			Vector2 target = Main.player[Projectile.owner].Center;
			target.Y -= 48f;
			Projectile.velocity = (target - Projectile.Center) / 20f;
			Projectile.rotation += 0.03f;
			Lighting.AddLight(Projectile.Center, 0.1f, 0.1f, 0.1f);

			if (IsLocalOwner)
			{
				if (TimeSpent % 95 == 0)
				{
					float range = 320f;
					NPC enemyTarget = null;
					for (int k = 0; k < Main.npc.Length; k++) // Select target
					{
						NPC npc = Main.npc[k];
						if (CanHomeInto(npc))
						{
							Vector2 newMove = Main.npc[k].Center - Projectile.Center;
							float distanceTo = newMove.Length();
							if (distanceTo < range)
							{
								enemyTarget = npc;
								range = distanceTo;
							}
						}
					}

					for (int i = 0; i < 4; i++)
					{
						if (enemyTarget != null) // There is a target
						{
							int type = ModContent.ProjectileType<KleeProjectileBurstMain>();
							Vector2 position = Projectile.Center - new Vector2(Main.rand.NextFloat(40f, 80f)).RotatedByRandom(MathHelper.ToRadians(135));
							SpawnProjectile(position, VelocityImmobile, type, Projectile.damage, Projectile.knockBack, enemyTarget.whoAmI);
						}

					}
				}
			}

			if (TimeSpent > 540) Projectile.alpha += 4;

				SpawnDust<KleeSparkleDust>(1f, 1f, 10, 1, 5);
			SpawnDust<KleeSparkleDustBig>(1f, 1f, 10, 1, 20);
			SpawnDust<KleeSparkleDustBigRed>(1f, 1f, 10, 1, 20);
		}

        public override void Kill(int timeLeft)
		{

			SpawnDust<KleeSparkleDust>(1f, 1f, 30, 8);
			SpawnDust<KleeSparkleDustBig>(1f, 1f, 30, 2);
			SpawnDust<KleeSparkleDustBigRed>(1f, 1f, 30, 2);
		}
    }
}
