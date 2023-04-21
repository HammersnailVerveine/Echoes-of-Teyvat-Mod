using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Kaeya.Projectiles;
using GenshinMod.Content.Characters.Noelle.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Noelle.Abilities
{
    public class NoelleAbilityBurst : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 0f;
            UseTime = 70;
            Velocity = Immobile;
            Cooldown = 15 * 60;
            Energy = 60;
            AbilityType = AbilityType.BURST;
        }

        public override void OnUse()
        {
            if (Character is CharacterNoelle noelle)
            {
                noelle.BurstTimer = 15 * 60 + 70;
                noelle.WeaponSize += 0.33f;
                noelle.Infuse(Common.GameObjects.Enums.GenshinElement.GEO, 2, false);
            }

            Vector2 direction = Main.MouseWorld - Player.Center;
            direction.Normalize();
            float angle = 2 * MathHelper.ToDegrees((float)Math.Atan2(1 - direction.Y, 0 - direction.X)) - 100f;

            int type = ModContent.ProjectileType<ProjectileNoelleBurst>();
            SpawnProjectile(VelocityToCursor(), type, Character.WeaponInfusion, angle, 1f);

            SoundEngine.PlaySound(SoundID.Item70);

            Character.RemoveVanityWeapon();
        }

        public override int GetScaling()
        { // Activation Damage (with the atk buff)
            return (int)(0.92f * (Character.EffectiveAttack + GetScaling2())* LevelScaling);
        }

        public override int GetScaling2()
        { // Atk buff
            return (int)(0.4f * Character.EffectiveDefense * LevelScaling);
        }
    }
}
