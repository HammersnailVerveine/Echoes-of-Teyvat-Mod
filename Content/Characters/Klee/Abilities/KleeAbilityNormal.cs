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
    public class KleeAbilityNormal : GenshinAbility
    {
        public override void SetDefaults()
        {
            Damage = 8;
            KnockBack = 5f;
            UseTime = 30;
            Velocity = 10f;
        }

        public override void OnUse()
        {
            int type = ModContent.ProjectileType<KleeProjectileNormal>();
            SpawnProjectile(VelocityToCursor(), type);
            SoundEngine.PlaySound(SoundID.Item1);
        }

        public override void OnUseEnd()
        {
        }
    }
}
