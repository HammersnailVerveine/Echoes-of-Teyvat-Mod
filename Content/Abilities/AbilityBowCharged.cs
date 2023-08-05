using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Abilities
{
    public class AbilityBowCharged : GenshinAbility
    {
        private int LinkedProjectile = -1; // Set to the id of the projectile spawned. -1 if projectile isn't spawned.

        public override void SetDefaults()
        {
            KnockBack = 5f;
            UseTime = 20;
            Velocity = 1f;
            AbilityType = AbilityType.CHARGED;
            HoldTimeMax = 150;
            HoldTimeFull = 120;
            Cooldown = 0;
            CooldownHeld = 0;
        }

        public override void OnUse()
        {
            SoundEngine.PlaySound(SoundID.Item5);
        }

        public override void OnHold()
        {
            if (LinkedProjectile == -1)
            { // Spawn the projectile if it isn't there already
                Vector2 direction = Main.MouseWorld - Player.Center;
                direction.Normalize();

                int type = ModContent.ProjectileType<ProjectileBowCharged>();
                LinkedProjectile = SpawnProjectile(VelocityToCursor(), type, Character.WeaponInfusion);
                SoundEngine.PlaySound(SoundID.Item1);
            }

            if (HoldTime > HoldTimeFull) HoldTime = HoldTimeFull;

            Character.RemoveVanityWeapon();
        }

        public override void OnHoldReset()
        {
            if (LinkedProjectile != -1)
            {
                Projectile projectile = Main.projectile[LinkedProjectile];
                if (projectile.type == ModContent.ProjectileType<ProjectileBowCharged>() && projectile.timeLeft > 0)
                {
                    projectile.timeLeft = 0;
                    projectile.netUpdate = true;
                }
                LinkedProjectile = -1;
            }
        }

        public override int GetScaling()
        {
            return (int)(2.92f * Character.EffectiveAttack * LevelScaling);
        }

        public override int GetScaling2()
        {
            return (int)(2.3f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
