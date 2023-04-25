using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Lisa.Projectiles;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Lisa.Abilities
{
    public class LisaAbilityBurst : GenshinAbility
    {
        public static float Range = 256f;

        public override void SetDefaults()
        {
            KnockBack = 1f;
            UseTime = 20;
            Velocity = 4f;
            AbilityType = AbilityType.BURST;
            Cooldown = 60 * 20;
            Energy = 80;
        }

        public override void OnUse()
        {
            SoundEngine.PlaySound(SoundID.Item60);
            int type = ModContent.ProjectileType<LisaProjectileBurst>();
            SpawnProjectile(Vector2.UnitY * -4f, type);
        }

        public override int GetScaling()
        {
            return (int)(0.366f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
