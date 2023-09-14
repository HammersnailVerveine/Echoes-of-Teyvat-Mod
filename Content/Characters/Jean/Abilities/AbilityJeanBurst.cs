using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Jean.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Jean.Abilities
{
    public class AbilityJeanBurst : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 5f;
            UseTime = 60;
            Velocity = AlmostImmobile;
            Cooldown = 20 * 60;
            AbilityType = AbilityType.BURST;
            Energy = 80;
        }

        public override void OnUse()
        {
            int type = ModContent.ProjectileType<ProjectileJeanBurst>();
            SpawnProjectile(Player.Center, VelocityToCursor() * AlmostImmobile, type);
            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast);
            Player.velocity.X *= 0.15f;

            foreach (GenshinCharacter character in GenshinPlayer.CharacterTeam)
                character.Heal(GetScaling3());
        }

        public override int GetScaling()
        {
            return (int)(4.25f * Character.EffectiveAttack * LevelScaling);
        }

        public override int GetScaling2()
        {
            return (int)(0.784f * Character.EffectiveAttack * LevelScaling);
        }

        public override int GetScaling3()
        {
            return (int)((2.51f * Character.EffectiveAttack + 1540) * Character.EffectiveHealing * LevelScaling);
        }
    }
}
