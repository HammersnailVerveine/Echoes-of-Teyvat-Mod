using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects.Weapons;

namespace GenshinMod.Content.Weapons.Bow
{
    public class BowHuntersBow : WeaponBow
    {
        public override void SafeSetDefaults()
        {
            Rarity = GenshinRarity.ONESTAR;
            SubstatType = StatType.ATTACK;
            BaseAttack = 185;
            BaseSubstat = 0f;

            StringOffSet = new Microsoft.Xna.Framework.Vector2(5, 14);
            StringColor = new Microsoft.Xna.Framework.Color(229, 217, 197);
        }
    }
}

