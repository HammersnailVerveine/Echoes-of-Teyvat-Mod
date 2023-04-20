using GenshinMod.Common.ModObjects.Weapons;
using GenshinMod.Common.GameObjects.Enums;

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

		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Apprentice Notes");
		}
    }
}

