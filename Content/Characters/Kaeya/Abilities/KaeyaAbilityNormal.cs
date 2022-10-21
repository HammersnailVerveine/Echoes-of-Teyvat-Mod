using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Kaeya.Abilities
{
    public class KaeyaAbilityNormal : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 1f;
            UseTime = 30;
            Velocity = 1f;
            AbilityType = AbilityType.NORMAL;
        }

        public override void OnUse()
        {
            Vector2 direction = Main.MouseWorld - Player.Center;
            direction.Normalize();
            float angle = 2 * MathHelper.ToDegrees((float)Math.Atan2(1 - direction.Y, 0 - direction.X));
            float mult = Main.rand.NextBool() ? 1f : -1f;
            angle -= mult * 100f;

            int type = ModContent.ProjectileType<ProjectileSwordNormal>();
            SpawnProjectile(Vector2.Zero, type, GenshinElement.NONE, angle, mult);

            SoundEngine.PlaySound(SoundID.Item1);

            Character.RemoveVanityWeapon(UseTime * 2);
            if (mult * (direction.X > 0 ? 1 : -1) < 0f) GenshinPlayer.ReverseUseArmDirection = true;
        }

        public override int GetScaling()
        {
            return (int)(0.3f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
