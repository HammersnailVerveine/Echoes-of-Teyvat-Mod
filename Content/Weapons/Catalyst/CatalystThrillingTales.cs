using GenshinMod.Common.ModObjects.Weapons;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.GameObjects;

namespace GenshinMod.Content.Weapons.Catalyst
{
	public class CatalystThrillingTales : WeaponCatalyst
	{
		public bool BuffReady = false;
		public int TimerBuffCooldown = 0;
		public int TimerBuffActive = 0;
		public GenshinCharacter BuffedCharacter;

		public override void SafeSetDefaults()
		{
			Rarity = GenshinRarity.THREESTAR;
			SubstatType = StatType.HEALTH;
			BaseAttack = 94;
			BaseSubstat = 13.5f;
		}

		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Thrilling Tales of Dragon Slayers");
		}

		public override void WeaponOnSwapOut()
		{
			if (TimerBuffCooldown <= 0)
			{
				BuffReady = true;
			}
		}

		public override void WeaponUpdate()
		{
			if (BuffReady && genshinPlayer.CharacterCurrent != Character)
			{
				BuffReady = false;
				BuffedCharacter = genshinPlayer.CharacterCurrent;
				TimerBuffCooldown = 60 * 20;
				TimerBuffActive = 60 * 10;
			}

			if (BuffedCharacter != null)
			{
				BuffedCharacter.StatAttack += RefineValue(0.24f);
			}
		}

		public override void WeaponResetEffects()
		{
			TimerBuffCooldown--;
			TimerBuffActive--;
			if (TimerBuffActive <= 0) BuffedCharacter = null;
		}
	}
}

