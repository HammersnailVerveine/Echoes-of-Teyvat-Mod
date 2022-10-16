﻿using GenshinMod.Common.GameObjects;
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
            KnockBack = 0f;
            UseTime = 60;
            Velocity = 0f;
            Cooldown = 1200;
            Energy = 80;
        }

        public override void OnUse()
        {
            SoundEngine.PlaySound(SoundID.Splash);
            int type = ModContent.ProjectileType<BarbaraProjectileBurstCircle>();
            SpawnProjectile(Vector2.Zero, type);
        }

        public override void OnUseEnd()
        {
        }
    }
}
