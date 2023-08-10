using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Klee.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Klee.Abilities
{
    public class AbilityKleeSkill : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 5f;
            UseTime = 30;
            Velocity = 7f;
            Cooldown = 20 * 60;
            ChargesMax = 2;
            AbilityType = AbilityType.SKILL;
        }

        public override void OnUse()
        {
            Vector2 target = Main.MouseWorld;
            Vector2 velocity = target - Player.Center;
            velocity.Normalize();
            velocity *= Velocity;
            if (Math.Abs(velocity.X) > 4f) velocity.X = Math.Sign(velocity.X) * 4f;
            int type = ModContent.ProjectileType<KleeProjectileSkill>();
            SpawnProjectile(velocity, type);
            SoundEngine.PlaySound(SoundID.Item1);
        }

        public override void OnUsePreUpdate()
        {
            Player.velocity.X *= 0.9f;
        }

        public override int GetScaling()
        {
            return (int)(0.9f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
