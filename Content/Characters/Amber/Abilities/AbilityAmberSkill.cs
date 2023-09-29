using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Amber.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Amber.Abilities
{
    public class AbilityAmberSkill : GenshinAbility
    {
        public static readonly float maxDistance = 352f;

        public override void SetDefaults()
        {
            KnockBack = 10f;
            UseTime = 30;
            Velocity = 10f;
            AbilityType = AbilityType.SKILL;
            Cooldown = 960;
        }

        public override void OnUse()
        {
            int type = ModContent.ProjectileType<AmberProjectileSkillDoll>();

            Vector2 velocity = Main.MouseWorld - Player.Center;
            if (velocity.Length() > maxDistance)
            {
                velocity.Normalize();
                velocity *= maxDistance;
            }

            float refX = (float)Math.Sqrt(Math.Abs(velocity.X)) * Math.Sign(velocity.X);
            float refY = (float)Math.Sqrt(Math.Abs(velocity.Y)) * Math.Sign(velocity.Y);

            velocity.Normalize();
            velocity *= new Vector2(refX, refY).Length() * 0.5f;
            velocity.X = refX * 0.45f;
            velocity.Y -= 3f;

            SpawnProjectile(Player.Center, velocity, type);

            SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing);
        }

        public override int GetScaling()
        {
            return (int)(1.23f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
