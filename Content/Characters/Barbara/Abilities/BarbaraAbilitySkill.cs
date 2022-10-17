using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Barbara.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Barbara.Abilities
{
    public class BarbaraAbilitySkill : GenshinAbility
    {
        public override void SetDefaults()
        {
            Damage = 12;
            KnockBack = 0f;
            UseTime = 40;
            Velocity = 0.00f;
            Cooldown = 60 * 32;
        }

        public override void OnUse()
        {
            SoundEngine.PlaySound(SoundID.Splash);
            int type = ModContent.ProjectileType<BarbaraProjectileSkillDroplet>();
            SpawnProjectile(Vector2.Zero, type);
        }

        public override void OnUseEnd()
        {
        }
    }
}
