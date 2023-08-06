using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Klee.Projectiles;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Klee.Abilities
{
    public class AbilityKleeNormal : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 5f;
            UseTime = 30;
            Velocity = 10f;
            AbilityType = AbilityType.NORMAL;
        }

        public override void OnUse()
        {
            int type = ModContent.ProjectileType<KleeProjectileNormal>();
            SpawnProjectile(VelocityToCursor(), type);
            SoundEngine.PlaySound(SoundID.Item1);
        }

        public override int GetScaling()
        {
            return (int)(0.7f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
