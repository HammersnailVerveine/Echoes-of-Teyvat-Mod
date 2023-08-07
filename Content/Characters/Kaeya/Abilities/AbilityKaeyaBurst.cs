using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Kaeya.Projectiles;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Kaeya.Abilities
{
    public class AbilityKaeyaBurst : GenshinAbility
    {
        public override void SetDefaults()
        {
            KnockBack = 0f;
            UseTime = 45;
            Velocity = Immobile;
            Cooldown = 15 * 60;
            Energy = 80;
            AbilityType = AbilityType.BURST;
        }

        public override void OnUse()
        {
            int type = ModContent.ProjectileType<KaeyaProjectileBurst>();
            int nbProjectile = 3;
            for (int i = 0; i < nbProjectile; i++)
                SpawnProjectile(Vector2.Zero, type, i * (360f / nbProjectile));

            SoundEngine.PlaySound(SoundID.Item28);
        }

        public override int GetScaling()
        {
            return (int)(0.8f * Character.EffectiveAttack * LevelScaling);
        }
    }
}
