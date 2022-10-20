using GenshinMod.Common.ModObjects.Weapons;
using GenshinMod.Common.GameObjects.Enums;

namespace GenshinMod.Content.Weapons.Sword
{
	public class SwordDullBlade : WeaponSword
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
			DisplayName.SetDefault("Dull Blade");
		}
    }
}

