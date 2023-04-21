using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Characters.Kaeya.Projectiles;
using GenshinMod.Content.Characters.Noelle.Projectiles;
using GenshinMod.Content.Characters.Noelle.Shields;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Noelle.Abilities
{
    public class NoelleAbilitySkill : GenshinAbility
    {
        public override void SetDefaults()
        {
            UseTime = 15;
            Cooldown = 24 * 60;
            AbilityType = AbilityType.SKILL;
        }

        public override void OnUse()
        {
            int duration = 12 * 60; // 12 seconds
            GenshinShield shield = new ShieldNoelle().Initialize(GenshinPlayer, GetScaling2(), duration, GenshinElement.GEO);
            GenshinPlayer.AddShield(shield);
            SoundEngine.PlaySound(SoundID.Item69);
            SpawnProjectile(Vector2.Zero, ModContent.ProjectileType<ProjectileNoelleShield>(), 1f);
        }

        public override int GetScaling()
        { // Damage
            return (int)(1.2f * Character.EffectiveDefense * LevelScaling);
        }

        public override int GetScaling2()
        { // Shield health
            return (int)((1.6f * Character.EffectiveDefense + 769) * LevelScaling);
        }

        public override int GetScaling3()
        { // Healing
            return (int)((0.213f * Character.EffectiveDefense + 102) * LevelScaling);
        }
    }
}
