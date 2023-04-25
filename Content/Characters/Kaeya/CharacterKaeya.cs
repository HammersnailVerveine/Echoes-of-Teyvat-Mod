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
            AbilitySkill = new KaeyaAbilitySkill().Initialize(this);
            AbilityBurst = new KaeyaAbilityBurst().Initialize(this);
            Autoswing = true;

            BaseHealthMax = 11636;
            BaseAttackMax = 223;
            BaseDefenseMax = 792;
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
