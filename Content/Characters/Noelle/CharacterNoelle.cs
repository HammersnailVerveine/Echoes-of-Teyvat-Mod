using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Abilities;
using GenshinMod.Content.Characters.Noelle.Abilities;

namespace GenshinMod.Content.Characters.Noelle
{
    public class CharacterNoelle : GenshinCharacter
	{
        public override void SetDefaults()
		{
			Name = "Noelle";
			Element = GenshinElement.GEO;
			WeaponType = WeaponType.CLAYMORE;
			AbilityNormal = new AbilityClaymoreNormal().Initialize(this);
			AbilityCharged = new AbilityClaymoreCharged().Initialize(this);
			AbilitySkill = new NoelleAbilitySkill().Initialize(this);
			AbilityBurst = new AbilitySwordNormal().Initialize(this);
			Autoswing = true;

			BaseHealthMax = 12071;
			BaseAttackMax = 191;
			BaseDefenseMax = 799;
		}

        public override void SafePostUpdate()
        {
        }

        public override void SafeResetEffects()
		{
			WeaponInfusion = GenshinElement.GEO;
			TimerWeaponInfusion = 60;
		}
    }
}
