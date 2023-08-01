using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects.Weapons;

namespace GenshinMod.Content.Weapons.Claymore
{
    public class ClaymoreWasterGreatsword : WeaponClaymore
    {
        public override void SafeSetDefaults()
        {
            Rarity = GenshinRarity.ONESTAR;
            SubstatType = StatType.ATTACK;
            BaseAttack = 185;
            BaseSubstat = 0f;
        }

        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Waster Greatsword");
        }
    }
}

