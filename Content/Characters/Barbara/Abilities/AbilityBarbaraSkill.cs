using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Barbara.Projectiles;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Barbara.Abilities
{
    public class AbilityBarbaraSkill : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 0f;
            UseTime = 40;
            Velocity = Immobile;
            Cooldown = 60 * 32;
            AbilityType = AbilityType.SKILL;
        }

        public override void OnUse()
        {
            SoundEngine.PlaySound(SoundID.Splash);
            int type = ModContent.ProjectileType<BarbaraProjectileSkillDroplet>();
            SpawnProjectile(Vector2.Zero, type);
        }

        public override void OnUsePreUpdate()
        {
            Player.velocity.X *= 0.9f;
        }

        public override int GetScaling() // Droplets damage
        {
            return (int)(0.58f * Character.EffectiveAttack * LevelScaling);
        }

        public override int GetScaling2() // NA Hit Healing (CA = 4NA)
        {
            return (int)((0.0075f * Character.EffectiveHealth + 72) * Character.EffectiveHealing * LevelScaling);
        }

        public override int GetScaling3() // Continuous Regeneration
        {
            return (int)((0.04f * Character.EffectiveHealth + 385) * Character.EffectiveHealing * LevelScaling);
        }
    }
}
