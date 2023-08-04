using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Jean.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Jean.Abilities
{
    public class JeanAbilitySkill : GenshinAbility
    {
        private int LinkedProjectile = -1; // Set to the id of the projectile spawned. -1 if projectile isn't spawned.

        public override void SetDefaults()
        {
            KnockBack = 1f;
            UseTime = 40;
            Velocity = AlmostImmobile;
            AbilityType = AbilityType.SKILL;
            HoldTimeMax = 360;
            HoldTimeFull = 0;
            Cooldown = 60;
            CooldownHeld = 60;
        }

        public override bool CanUse() => ChargesCurrent > 0 && (GenshinPlayer.Stamina >= 20 || LinkedProjectile != -1);

        public override void OnUse()
        {
            if (LinkedProjectile != -1)
            {
                Projectile projectile = Main.projectile[LinkedProjectile];
                if (projectile.type == ModContent.ProjectileType<ProjectileJeanSkill>() && projectile.timeLeft > 0)
                {
                    projectile.timeLeft = 0;
                    projectile.netUpdate = true;
                }
                LinkedProjectile = -1;
            }
        }

        public override void OnHold()
        {
            if (LinkedProjectile == -1)
            { // Start swinging
                Vector2 direction = Main.MouseWorld - Player.Center;
                direction.Normalize();

                int type = ModContent.ProjectileType<ProjectileJeanSkill>();
                LinkedProjectile = SpawnProjectile(VelocityToCursor(), type, Character.WeaponInfusion);
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing);
            }
            else if (HoldTime < 360)
            { // Try to consume stamina every second
                if (HoldTime % 20 == 0) // %3
                { // Consumes 20 per second
                    if (!GenshinPlayer.TryUseStamina(1))
                        HoldTime = HoldTimeMax - 1; // Hold time ends on next frame
                }
                if (HoldTime % 25 == 0)
                    SoundEngine.PlaySound(SoundID.DD2_BookStaffCast);
            }
            Character.RemoveVanityWeapon();
        }

        public override void OnHoldReset()
        {
        }

        public override int GetScaling()
        {
            return (int)(0.85f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
