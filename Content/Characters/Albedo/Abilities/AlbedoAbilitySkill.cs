using GenshinMod.Common.GameObjects;
using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Characters.Albedo.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Albedo.Abilities
{
    public class AlbedoAbilitySkill : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 1f;
            UseTime = 22;
            Velocity = AlmostImmobile;
            AbilityType = AbilityType.SKILL;
        }

        public override void OnUse()
        {
            SoundEngine.PlaySound(SoundID.SplashWeak);
            int type = ModContent.ProjectileType<AlbedoProjectileSkillMain>();

            Vector2 target = Main.MouseWorld;
            Vector2 offSet = target - Player.Center;
            offSet.Normalize();
            offSet *= 16f;

            Vector2 position = Player.Center;

            for (int i = 0; i < 15 ; i++)
            {
                position += offSet;
                offSet = Collision.TileCollision(position, offSet, 2, 2, true, false, (int)Player.gravDir);
                if (offSet.Length() < 15f)
                {
                    break;
                }
            }

            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == Player.whoAmI && proj.type == type)
                    proj.Kill();
            }

            SpawnProjectile(position, VelocityToCursor(), type);
        }

        public override int GetScaling()
        { // spawn
            return (int)(1.3f * Character.EffectiveAttack * LevelScaling);
        }

        public override int GetScaling2()
        { // procs
            return (int)(1.34f * Character.EffectiveDefense * LevelScaling);
        }
    }
}
