using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
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
			AbilityNormal = new KaeyaAbilityNormal().Initialize(this);
			AbilityCharged = new KaeyaAbilityCharged().Initialize(this);
			AbilitySkill = new KaeyaAbilitySkill().Initialize(this);
			AbilityBurst = new KaeyaAbilityBurst().Initialize(this);
			Autoswing = true;

			BaseAttack = 63;
			BaseDefense = 124;
			BaseHealth = 207;
		}
    }
}
