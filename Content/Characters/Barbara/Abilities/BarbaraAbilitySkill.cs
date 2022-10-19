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
        }

        public override void OnUse()
        {
            SoundEngine.PlaySound(SoundID.Splash);
            int type = ModContent.ProjectileType<BarbaraProjectileSkillDroplet>();
            SpawnProjectile(Vector2.Zero, type);
        }

        public override int GetScaling() // Continuous Regeneration
        {
            int level = Level;
            return (int)(0.03f * Character.EffectiveHealth * level + 30 * level);
        }

        public override int GetScaling2() // NA Hit (CA = 4NA)
        {
            int level = Level;
            return (int)(0.006f * Character.EffectiveHealth * level + 6 * level);
        }
    }
}
