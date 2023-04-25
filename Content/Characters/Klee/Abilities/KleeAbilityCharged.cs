using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Klee.Projectiles;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Klee.Abilities
{
    public class KleeAbilityCharged : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 10f;
            UseTime = 45;
            Velocity = 10f;
            Stamina = 50;
            Cooldown = 120;
            AbilityType = AbilityType.CHARGED;
        }

        public override void OnUse()
        {
            Vector2 velocity = new Vector2(0f, -10f);
            int type = ModContent.ProjectileType<KleeProjectileChargedMain>();
            SpawnProjectile(velocity, type);
            SoundEngine.PlaySound(SoundID.Item1);
        }

        public override int GetScaling()
        {
            return (int)(1.5f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
