using GenshinMod.Common.GameObjects;
using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Characters.Amber.Projectiles;
using GenshinMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Amber.Abilities
{
    public class AbilityAmberBurst : GenshinAbility
    {
        private const float RangeRef = 20f; 

        public override void SetDefaults()
        {
            KnockBack = 0f;
            UseTime = 30;
            Velocity = AlmostImmobile;
            Cooldown = 12 * 0;
            AbilityType = AbilityType.BURST;
            Energy = 0;
        }

        public override void OnUse()
        {
            int type = ModContent.ProjectileType<AmberProjectileBurstMain>();

            Vector2 offSet = VelocityToCursor();
            offSet.Normalize();
            offSet *= RangeRef;

            Vector2 position = Player.Center;
            bool foundTarget = false;

            for (int i = 0; i < 20; i++)
            {
                if (!foundTarget)
                {
                    position += offSet;
                    offSet = Collision.TileCollision(position, offSet, 2, 2, true, false, (int)Player.gravDir);
                    if (offSet.Length() < RangeRef - 1f)
                    {
                        break;
                    }

                    for (int k = 0; k < Main.npc.Length; k++)
                    {
                        NPC npc = Main.npc[k];
                        if (GenshinProjectile.IsValidTarget(npc))
                        {
                            if (position.Distance(npc.Center) < npc.width + 32f) // if the NPC is close to the projectile path, snaps to it.
                            {
                                foundTarget = true;
                                position = npc.Center;
                            }
                        }
                    }
                }
            }

            offSet = Vector2.UnitY * RangeRef;

            for (int i = 0; i < 20; i++)
            {
                position += offSet;
                offSet = Collision.TileCollision(position, offSet, 2, 20, true, false, (int)Player.gravDir);
                if (offSet.Length() < RangeRef - 1f)
                {
                    break;
                }
            }


            SpawnProjectile(-Vector2.UnitY, ModContent.ProjectileType<ProjectileBowNormal>());
            SpawnProjectile(position, VelocityToCursor(), type);
            Character.RemoveVanityWeapon();
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing);
        }

        public override int GetScaling()
        { // Hits 18 times
            return (int)(0.281f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
