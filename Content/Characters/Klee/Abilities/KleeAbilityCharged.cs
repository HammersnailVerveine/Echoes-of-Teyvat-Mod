using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Klee.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
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
        }

        public override void OnUse()
        {
            Vector2 velocity = new Vector2(0f, -10f);
            Vector2 position = Player.Center;
            int type = ModContent.ProjectileType<KleeProjectileChargedMain>();
            SpawnProjectile(position, velocity, type);
            SoundEngine.PlaySound(SoundID.Item1);
        }

        public override int GetScaling()
        {
            return (int)(GetScaling2() * 0.2f);
        }

        public override int GetScaling2()
        {
            return (int)(1.5f * Character.EffectiveAttack * Level);
        }
    }
}
