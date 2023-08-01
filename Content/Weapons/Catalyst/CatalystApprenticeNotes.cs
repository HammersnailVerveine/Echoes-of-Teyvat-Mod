using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects.Weapons;

namespace GenshinMod.Content.Weapons.Catalyst
{
    public class CatalystApprenticeNotes : WeaponCatalyst
    {
        public override void SafeSetDefaults()
        {
            Rarity = GenshinRarity.ONESTAR;
            SubstatType = StatType.ATTACK;
            BaseAttack = 185;
            BaseSubstat = 0f;
        }
    }
}

