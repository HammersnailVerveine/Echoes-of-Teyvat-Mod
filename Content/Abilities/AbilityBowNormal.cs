using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Abilities
{
    public class AbilityBowNormal : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 1f;
            UseTime = 1;
            Velocity = 1f;
            AbilityType = AbilityType.NORMAL;
            Cooldown = 0;
        }

        public override void OnUse()
        {
            int type = ModContent.ProjectileType<ProjectileBowArrow>();
            int projID = SpawnProjectile(VelocityToCursor().RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-5, 5))) * 15f, type, GenshinElement.NONE);
            Projectile proj = Main.projectile[projID];
            proj.position = Player.Center + new Vector2(proj.width, proj.height) * 0.5f - new Vector2(proj.width, proj.height);
            proj.netUpdate = true;
            SpawnProjectile(VelocityToCursor(), ModContent.ProjectileType<ProjectileBowNormal>());
            SoundEngine.PlaySound(SoundID.Item5);
            Character.RemoveVanityWeapon();
        }

        public override int GetScaling()
        { // average damage of fischl atk string
            return (int)(0.556f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
