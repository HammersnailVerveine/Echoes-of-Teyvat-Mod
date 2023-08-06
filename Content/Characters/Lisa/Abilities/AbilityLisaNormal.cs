using GenshinMod.Common.GameObjects;
using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Characters.Lisa.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Lisa.Abilities
{
    public class AbilityLisaNormal : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 1f;
            UseTime = 25;
            Velocity = AlmostImmobile;
            AbilityType = AbilityType.NORMAL;
        }

        public override void OnUse()
        {
            SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap);
            int type = ModContent.ProjectileType<LisaProjectileNormal>();

            Vector2 target = Main.MouseWorld;
            Vector2 offSet = target - Player.Center;
            offSet.Normalize();
            offSet = offSet.RotatedByRandom(MathHelper.ToRadians(5));

            float rangeRand = Main.rand.NextFloat(14f, 16f);
            offSet *= rangeRand;

            Vector2 position = Player.Center;

            for (int i = 0; i < 15; i++)
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
                    if (GenshinProjectile.CanHomeInto(npc))
                    {
                        if (position.Distance(npc.Center) < npc.width + 32f) // if the NPC is close to the projectile path, snaps to it.
                        {
                            SpawnProjectile(npc.Center, VelocityToTarget(npc.Center), type);
                            return;
                        }
                    }
                }
            }

            SpawnProjectile(position, VelocityToCursor(), type);
        }

        public override int GetScaling()
        {
            return (int)(0.42f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
