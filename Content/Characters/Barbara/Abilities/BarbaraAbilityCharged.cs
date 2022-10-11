using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Barbara.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Barbara.Abilities
{
    public class BarbaraAbilityCharged : GenshinAbility
    {
        public override void SetDefaults()
        {
            Damage = 30;
            KnockBack = 4f;
            UseTime = 50;
            Velocity = 0.01f;
        }

        public override void OnUse()
        {
            SoundEngine.PlaySound(SoundID.Splash);
            int type = ModContent.ProjectileType<BarbaraProjectileCharged>();

            Vector2 target = Main.MouseWorld;
            Vector2 offSet = target - Player.Center;
            offSet.Normalize();
            offSet = offSet.RotatedByRandom(MathHelper.ToRadians(5));

            float rangeRand = Main.rand.NextFloat(14f, 16f); 
            offSet *= rangeRand;

            Vector2 position = Player.Center;

            for (int i = 0; i < 15 ; i++)
            {
                position += offSet;
                offSet = Collision.TileCollision(position, offSet, 2, 2, true, false, (int)Player.gravDir);
                if (offSet.Length() < rangeRand - 1f)
                {
                    break;
                }

                for (int k = 0; k < Main.npc.Length; k++)
                {
                    NPC npc = Main.npc[k];
                    if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.lifeMax > 5)
                    {
                        if (position.Distance(npc.Center) < npc.width + 32f) // if the NPC is close to the projectile path, snaps to it.
                        {
                            SpawnProjectile(npc.Center, VelocityToTarget(npc.Center), type, npc.whoAmI);
                            return;
                        }
                    }
                }
            }

            SpawnProjectile(position, VelocityToCursor(), type);
        }

        public override void OnUseEnd()
        {
        }
    }
}
