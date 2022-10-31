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
    }
}
