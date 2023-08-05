using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Jean.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Jean.Abilities
{
    public class JeanAbilityCharged : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 1f;
            UseTime = 40;
            Velocity = AlmostImmobile;
            AbilityType = AbilityType.CHARGED;
            Stamina = 20;
        }

        public override void OnUse()
        {
            int type = ModContent.ProjectileType<ProjectileJeanCharged>();

            foreach (Projectile p in Main.projectile)
                if (p.active && p.owner == Player.whoAmI && p.type == type)
                    p.Kill();

            int stabDir = (Main.MouseWorld - Player.Center).X > 0 ? -1 : 1;
            float angle = stabDir == 1 ? 7f * MathHelper.Pi / 4f : 5f * MathHelper.Pi / 4f;
            SpawnProjectile(VelocityToCursor(), type, Character.WeaponInfusion, angle, stabDir);
            GenshinPlayer.ReverseUseArmDirection = true;
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing);
            Character.RemoveVanityWeapon(); 
        }

        public override int GetScaling()
        {
            return (int)(1.62f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
