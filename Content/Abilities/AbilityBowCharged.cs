using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
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
            KnockBack = 2f;
            UseTime = 20;
            Velocity = 1f;
            AbilityType = AbilityType.CHARGED;
            HoldTimeMax = 150;
            HoldTimeFull = 90;
            Cooldown = 0;
            CooldownHeld = 0;
        }

        public override void OnUse()
        {
            int type = ModContent.ProjectileType<ProjectileBowArrow>();

            Vector2 velocity = VelocityToCursor() * 15f;
            if (HoldTime > 20)
            {
                float velocityMult = ((HoldTime - 20) / (HoldTimeFull - 60)) * 0.33f;
                if (velocityMult > 0.5f) velocityMult = 0.5f;
                velocity *= 1f + velocityMult;
            }

            GenshinElement element = HoldFull ? Character.Element : GenshinElement.NONE;
            int damage = HoldFull ? GetScaling2() : GetScaling();
            
            int projID = SpawnProjectile(velocity, type, damage, element);
            Projectile proj = Main.projectile[projID];
            proj.position = Player.Center + velocity + new Vector2(proj.width, proj.height) * 0.5f - new Vector2(proj.width, proj.height);
            proj.netUpdate = true;
            SoundEngine.PlaySound(SoundID.Item5);
        }

        public override void OnUsePreUpdate()
        {
            Player.velocity.X *= 0.75f;
        }

        public override void OnHold()
        {
            if (LinkedProjectile == -1)
            { // Spawn the projectile if it isn't there already
                Vector2 direction = Main.MouseWorld - Player.Center;
                direction.Normalize();

                int type = ModContent.ProjectileType<ProjectileBowCharged>();
                LinkedProjectile = SpawnProjectile(VelocityToCursor(), type);
                SoundEngine.PlaySound(SoundID.Item1);
            }

            if (HoldTime > HoldTimeFull) HoldTime = HoldTimeFull;

            Character.RemoveVanityWeapon();
            Player.velocity.X *= 0.9f;
        }

        public override void OnHoldReset()
        {
            if (LinkedProjectile != -1)
            {
                Projectile projectile = Main.projectile[LinkedProjectile];
                if (projectile.type == ModContent.ProjectileType<ProjectileBowCharged>() && projectile.timeLeft > 0)
                {
                    projectile.timeLeft = 15;
                    projectile.netUpdate = true;
                }
                LinkedProjectile = -1;
            }
        }

        public override int GetScaling()
        { // 55.6f NA
            return (int)(0.439f * Character.EffectiveAttack * LevelScaling);
        }

        public override int GetScaling2()
        {
            return (int)(1.24 * Character.EffectiveAttack * LevelScaling);
        }
    }
}
