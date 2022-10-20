using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Klee.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Klee.Abilities
{
    public class KleeAbilityBurst : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 2f;
            UseTime = 60;
            Velocity = 7f;
            Cooldown = 900;
            Energy = 60;
            AbilityType = AbilityType.BURST;
        }

        public override void OnUse()
        {
            Vector2 position = new Vector2(Player.Center.X, Player.Center.Y - 20);
            int type = ModContent.ProjectileType<KleeProjectileBurstFollow>();
            SpawnProjectile(position, Vector2.Zero, type);
            SoundEngine.PlaySound(SoundID.Item1);
        }

        public override int GetScaling()
        {
            return (int)(0.4f * Character.EffectiveAttack * Level);
        }
    }
}
