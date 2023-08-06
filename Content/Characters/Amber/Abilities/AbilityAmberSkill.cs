using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Amber.Projectiles;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Amber.Abilities
{
    public class AbilityAmberSkill : GenshinAbility
    {
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
            SpawnProjectile(VelocityToCursor(), type);
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing);
        }

        public override int GetScaling()
        {
            return (int)(1.23f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
