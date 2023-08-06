using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Abilities;
using GenshinMod.Content.Characters.Kaeya.Abilities;

namespace GenshinMod.Content.Characters.Kaeya
{
    public class CharacterKaeya : GenshinCharacter
    {
        public override void SetDefaults()
        {
            Name = "Kaeya";
            Element = GenshinElement.CRYO;
            WeaponType = WeaponType.SWORD;
            AbilityNormal = new AbilitySwordNormal().Initialize(this);
            AbilityCharged = new AbilitySwordCharged().Initialize(this);
            AbilitySkill = new AbilityKaeyaSkill().Initialize(this);
            AbilityBurst = new AbilityKaeyaBurst().Initialize(this);

            BaseHealthMax = 11636;
            BaseAttackMax = 223;
            BaseDefenseMax = 792;

			BurstQuotes[0] = "Don't get frostbite";
			BurstQuotes[1] = "Oh, so sorry!";
			BurstQuotes[2] = "This moment will be frozen in time";
		}

        /* INFUSIONS TEST
		public override void SafeResetEffects()
		{
			if (TimerWeaponInfusion < 2)
            {
				TimerWeaponInfusion = 180;
				switch(WeaponInfusion)
                {
					case GenshinElement.CRYO:
						WeaponInfusion = GenshinElement.DENDRO;
						break;
					case GenshinElement.DENDRO:
						WeaponInfusion = GenshinElement.HYDRO;
						break;
					case GenshinElement.HYDRO:
						WeaponInfusion = GenshinElement.GEO;
						break;
					case GenshinElement.GEO:
						WeaponInfusion = GenshinElement.ELECTRO;
						break;
					case GenshinElement.ELECTRO:
						WeaponInfusion = GenshinElement.ANEMO;
						break;
					case GenshinElement.ANEMO:
						WeaponInfusion = GenshinElement.PYRO;
						break;
					default:
						WeaponInfusion = GenshinElement.CRYO;
						break;
                }
			}
			GenshinPlayer.Stamina = GenshinPlayer.StaminaMax;
		}
		*/
    }
}
