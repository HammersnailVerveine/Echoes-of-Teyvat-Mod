using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Barbara.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Barbara.Abilities
{
    public class BarbaraAbilityBurst : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 0f;
            UseTime = 60;
            Velocity = Immobile;
            Cooldown = 1200;
            Energy = 80;
            AbilityType = AbilityType.BURST;
        }

        public override void OnUse()
        {
            SoundEngine.PlaySound(SoundID.Splash);
            int type = ModContent.ProjectileType<BarbaraProjectileBurstCircle>();
            SpawnProjectile(Vector2.Zero, type);
        }

        public override int GetScaling() // Healing
        {
            return (int)((0.176f * Character.EffectiveHealth + 1694) * Character.EffectiveHealing * LevelScaling);
        }
    }
}
