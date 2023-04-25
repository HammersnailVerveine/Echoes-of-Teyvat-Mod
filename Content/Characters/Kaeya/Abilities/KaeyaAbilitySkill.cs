using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Kaeya.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Kaeya.Abilities
{
    public class KaeyaAbilitySkill : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 5f;
            UseTime = 30;
            Velocity = AlmostImmobile;
            Cooldown = 6 * 60;
            AbilityType = AbilityType.SKILL;
        }

        public override void OnUse()
        {
            Vector2 target = Main.MouseWorld;
            Vector2 velocity = target - Player.Center;
            velocity.Normalize();
            Vector2 position = Player.Center + velocity * 64f;
            velocity *= Velocity;

            int type = ModContent.ProjectileType<KaeyaProjectileSkill>();
            SpawnProjectile(position, velocity, type);
            SoundEngine.PlaySound(SoundID.Item50);
            Character.RemoveVanityWeapon();
        }

        public override int GetScaling()
        {
            return (int)(1.9f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
