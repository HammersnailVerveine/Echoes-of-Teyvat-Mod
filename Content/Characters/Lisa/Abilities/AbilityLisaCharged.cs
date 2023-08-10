using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Lisa.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Lisa.Abilities
{
    public class AbilityLisaCharged : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 1f;
            UseTime = 40;
            Stamina = 50;
            Velocity = AlmostImmobile;
            AbilityType = AbilityType.CHARGED;
        }

        public override void OnUse()
        {
            SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap);
            int type = ModContent.ProjectileType<LisaProjectileCharged>();

            Vector2 target = Main.MouseWorld;
            Vector2 offSet = target - Player.Center;
            offSet.Normalize();
            offSet *= 15f;

            Vector2 position = Player.Center;

            for (int i = 0; i < 15; i++)
            {
                position += offSet;
                offSet = Collision.TileCollision(position, offSet, 2, 2, true, false, (int)Player.gravDir);
                if (offSet.Length() < 15f - 1f)
                {
                    break;
                }
            }

            SpawnProjectile(position, VelocityToCursor(), type);
        }

        public override void OnUsePreUpdate()
        {
            Player.velocity.X *= 0.8f;
        }

        public override int GetScaling()
        {
            return (int)(1.77f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
