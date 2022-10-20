using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Barbara.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Barbara.Abilities
{
    public class BarbaraAbilitySkill : GenshinAbility
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

        public override int GetScaling() // Droplets damage
        {
            return (int)(0.58f * Character.EffectiveAttack * LevelScaling);
        }

        public override int GetScaling2() // NA Hit (CA = 4NA)
        {
            return (int)(0.006f * Character.EffectiveHealth * LevelScaling + 6 * LevelScaling);
        }

        public override int GetScaling3() // Continuous Regeneration
        {
            return (int)(0.03f * Character.EffectiveHealth * LevelScaling + 30 * LevelScaling);
        }
    }
}
