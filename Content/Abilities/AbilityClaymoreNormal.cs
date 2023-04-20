using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Abilities
{
    public class AbilityClaymoreNormal : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 1f;
            UseTime = 45;
            Velocity = AlmostImmobile;
            AbilityType = AbilityType.NORMAL;
        }

        public override void OnUse()
        {
            Vector2 direction = Main.MouseWorld - Player.Center;
            direction.Normalize();
            float angle = 2 * MathHelper.ToDegrees((float)Math.Atan2(1 - direction.Y, 0 - direction.X));
            float mult = Main.rand.NextBool() ? 1f : -1f;
            angle -= mult * 100f;

            int type = ModContent.ProjectileType<ProjectileClaymoreNormal>();
            SpawnProjectile(VelocityToCursor(), type, GenshinElement.NONE, angle, mult);

            SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing);
            Character.RemoveVanityWeapon();
            if (mult * (direction.X > 0 ? 1 : -1) < 0f) GenshinPlayer.ReverseUseArmDirection = true;
        }

        public override int GetScaling()
        {
            return (int)(0.75f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
