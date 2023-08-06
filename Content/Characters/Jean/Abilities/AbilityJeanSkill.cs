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
    public class AbilityJeanSkill : GenshinAbility
    {
        private int LinkedProjectile = -1; // Set to the id of the projectile spawned. -1 if projectile isn't spawned.

        public override void SetDefaults()
        {
            KnockBack = 5f;
            UseTime = 25;
            Velocity = 1f;
            AbilityType = AbilityType.SKILL;
            HoldTimeMax = 370;
            HoldTimeFull = 0;
            Cooldown = 60 * 6;
            CooldownHeld = 60 * 6;
        }

        public override void OnUse()
        {
            int type = ModContent.ProjectileType<ProjectileJeanSkillStab>();

            foreach (Projectile p in Main.projectile)
                if (p.active && p.owner == Player.whoAmI && (p.type == type || p.type == ModContent.ProjectileType<ProjectileJeanCharged>()))
                    p.Kill();

            Vector2 velocity = VelocityToCursor();
            int projID = SpawnProjectile(Player.Center, velocity * 20f, type);
            Projectile proj = Main.projectile[projID];
            proj.position = Player.Center + velocity * 48f + new Vector2(proj.width, proj.height) * Character.WeaponSize * 0.5f - new Vector2(proj.width, proj.height);
            proj.netUpdate = true;
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing);
        }

        public override void OnHold()
        {
            if (HoldTime > 10)
            {
                if (LinkedProjectile == -1)
                { // Spawn the projectile if it isn't there already
                    Vector2 direction = Main.MouseWorld - Player.Center;
                    direction.Normalize();

                    int type = ModContent.ProjectileType<ProjectileJeanSkill>();
                    LinkedProjectile = SpawnProjectile(VelocityToCursor(), type, Character.WeaponInfusion);
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing);
                }

                else if (HoldTime < HoldTimeMax)
                { // Try to consume stamina every second
                    if (HoldTime % 3 == 0)
                    { // Consumes 20 per second
                        if (!GenshinPlayer.TryUseStamina(1))
                            HoldTime = HoldTimeMax - 1; // Hold time ends on next frame
                    }
                    if (HoldTime % 25 == 0)
                        SoundEngine.PlaySound(SoundID.DD2_BookStaffCast);
                }
            }
            Character.RemoveVanityWeapon();
        }

        public override void OnHoldReset()
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
