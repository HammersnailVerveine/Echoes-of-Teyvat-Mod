using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Noelle.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Noelle.Abilities
{
    public class AbilityNoelleCharged : GenshinAbility
    {
        private int LinkedProjectile = -1; // Set to the id of the projectile spawned. -1 if projectile isn't spawned.

        public override void SetDefaults()
        {
            KnockBack = 1f;
            UseTime = 40;
            Velocity = AlmostImmobile;
            AbilityType = AbilityType.CHARGED;
            HoldTimeMax = 360;
            HoldTimeFull = 360;
        }

        public override bool CanUse() => ChargesCurrent > 0 && (GenshinPlayer.Stamina >= 20 || LinkedProjectile != -1);

        public override void OnUse()
        {
            if (LinkedProjectile != -1)
            {
                if (HoldTime < 60) GenshinPlayer.TryUseStamina((int)((60 - HoldTime) / 3f));
                Projectile projectile = Main.projectile[LinkedProjectile];
                if (projectile.type == ModContent.ProjectileType<ProjectileNoelleCharged>() && projectile.timeLeft > 30)
                {
                    projectile.timeLeft = 30;
                    projectile.netUpdate = true;
                }
                LinkedProjectile = -1;
            }
        }

        public override void OnUsePreUpdate()
        {
            Player.velocity.X *= 0.9f;
        }

        public override void OnHold()
        {
            if (LinkedProjectile == -1)
            { // Start swinging
                Vector2 direction = Main.MouseWorld - Player.Center;
                direction.Normalize();
                float angle = 2 * MathHelper.ToDegrees((float)Math.Atan2(1 - direction.Y, 0 - direction.X));
                float mult = Main.rand.NextBool() ? 1f : -1f;
                angle -= mult * 100f;

                int type = ModContent.ProjectileType<ProjectileNoelleCharged>();
                LinkedProjectile = SpawnProjectile(VelocityToCursor(), type, Character.WeaponInfusion, angle, mult);
                if (mult * (direction.X > 0 ? 1 : -1) < 0f) GenshinPlayer.ReverseUseArmDirection = true;
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing);
            }
            else if (HoldTime < 360)
            { // Try to consume stamina every second
                if (HoldTime % 3 == 0)
                { // Consumes 20 per second
                    if (!GenshinPlayer.TryUseStamina(1))
                        HoldTime = HoldTimeMax - 1; // Hold time ends on next frame
                    else if (HoldTime % 60 == 0)
                        SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing);
                }
            }
            Character.RemoveVanityWeapon();
            Player.velocity.X *= 0.9f;
        }

        public override void OnHoldReset()
        {
            if (LinkedProjectile != -1)
            {
                if (HoldTime < 60) GenshinPlayer.TryUseStamina((int)((60 - HoldTime) / 3f));
                Projectile projectile = Main.projectile[LinkedProjectile];
                if (projectile.type == ModContent.ProjectileType<ProjectileNoelleCharged>() && projectile.timeLeft > 30)
                {
                    projectile.timeLeft = 30;
                    projectile.netUpdate = true;
                }
                LinkedProjectile = -1;
            }
        }

        public override int GetScaling()
        {
            return (int)(0.85f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
