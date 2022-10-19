using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Characters.Klee.Abilities;

namespace GenshinMod.Content.Characters.Klee
{
    public class CharacterKlee : GenshinCharacter
	{
        public override void SetDefaults()
		{
			Name = "Klee";
			Element = GenshinElement.PYRO;
			WeaponType = WeaponType.CATALYST;
			AbilityNormal = new KleeAbilityNormal().Initialize(this);
			AbilityCharged = new KleeAbilityCharged().Initialize(this);
			AbilitySkill = new KleeAbilitySkill().Initialize(this);
			AbilityBurst = new KleeAbilityBurst().Initialize(this);

			BaseAttack = 63;
			BaseDefense = 124;
			BaseHealth = 207;
		}
    }
}
