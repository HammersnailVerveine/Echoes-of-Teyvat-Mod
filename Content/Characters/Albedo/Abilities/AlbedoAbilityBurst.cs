using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Albedo.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Albedo.Abilities
{
    public class AlbedoAbilityBurst : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 5f;
            UseTime = 60;
            Velocity = AlmostImmobile;
            Cooldown = 12 * 60;
            AbilityType = AbilityType.BURST;
            Energy = 40;
        }

        public override void OnUse()
        {
            Vector2 target = Main.MouseWorld;
            Vector2 velocity = target - Player.Center;
            velocity.Normalize();
            Vector2 position = Player.Center + velocity * 128f;
            velocity *= Velocity;

            int type = ModContent.ProjectileType<AlbedoBurstMain>();
            SpawnProjectile(position, velocity, type);
            SoundEngine.PlaySound(SoundID.Item69);
        }

        public override int GetScaling()
        {
            return (int)(3.67f * Character.EffectiveAttack * LevelScaling);
        }

        public override int GetScaling2()
        {
            return (int)(0.72f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
