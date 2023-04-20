using GenshinMod.Common.ModObjects.Weapons;
using GenshinMod.Common.GameObjects.Enums;

namespace GenshinMod.Content.Weapons.Claymore
{
	public class ClaymoreWasterGreatsword : WeaponClaymore
	{
		public override void SafeSetDefaults()
		{
			Rarity = GenshinRarity.ONESTAR;
			SubstatType = StatType.ATTACK;
			BaseAttack = 27;
			BaseSubstat = 0f;
		}

		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Waster Greatsword");
		}
    }
}

