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
            Damage = 0;
            KnockBack = 2f;
            UseTime = 60;
            Velocity = 0f;
            Cooldown = 900;
            Energy = 80;
        }

        public override void OnUse()
        {
        }

        public override void OnUseEnd()
        {
        }
    }
}
