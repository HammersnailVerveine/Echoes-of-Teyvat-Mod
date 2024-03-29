﻿using GenshinMod.Common.GameObjects;
using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Characters.Lisa.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Lisa.Abilities
{
    public class AbilityLisaSkill : GenshinAbility
    {
        public static float Range = 256f;

        public override void SetDefaults()
        {
            KnockBack = 1f;
            UseTime = 20;
            Velocity = 4f;
            AbilityType = AbilityType.SKILL;
            HoldTimeMax = 300;
            HoldTimeFull = 114;
            Cooldown = 60;
            CooldownHeld = 60 * 16;
        }

        public override void OnUse()
        {
            if (HoldFull)
            {
                int type = ModContent.ProjectileType<LisaProjectileSkillHold>();
                SoundEngine.PlaySound(SoundID.Item60);
                bool firstHit = true;
                if (Character is CharacterLisa lisa)
                {
                    foreach (NPC npc in Main.npc)
                    {
                        Vector2 dir = npc.Center - Player.Center;
                        if (GenshinProjectile.IsValidTarget(npc) && dir.Length() < Range)
                        {
                            dir.Normalize();
                            SpawnProjectileSpecific(GetSource(), npc.Center, dir, type, GetScalingCharged(lisa.GetNPCStacks(npc)), 5f, Character.Player.whoAmI, Character.Element, AbilityType, firstHit ? 1f : 0f);
                            firstHit = false;
                        }
                    }
                }
            }
            else
            {
                SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap);
                int type = ModContent.ProjectileType<LisaProjectileSkill>();
                SpawnProjectile(VelocityToCursor(), type);
            }
        }

        public override void OnUsePreUpdate()
        {
            Player.velocity.X *= 0.9f;
        }

        public override void OnHold()
        {
            if (HoldTime % (HoldFull ? 15 : 30) == 0)
                SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap);
            Player.velocity.X *= 0.9f;
        }

        public override int GetScaling()
        {
            return (int)(0.8f * Character.EffectiveAttack * LevelScaling);
        }

        public int GetScalingCharged(int stacks)
        {
            return (int)((3.2f + 0.52f * stacks) * Character.EffectiveAttack * LevelScaling);
        }
    }
}
