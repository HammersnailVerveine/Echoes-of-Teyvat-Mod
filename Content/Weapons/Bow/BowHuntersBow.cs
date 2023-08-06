using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects.Weapons;
using Microsoft.Xna.Framework;

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

            StringOffSet = new Vector2(4, 10);
            StringColor = new Color(229, 217, 197);
        }
    }
}

