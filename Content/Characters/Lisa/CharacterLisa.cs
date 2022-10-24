using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Characters.Lisa.Abilities;

namespace GenshinMod.Content.Characters.Lisa
{
    public class CharacterLisa : GenshinCharacter
	{
		public bool skillActive;

        public override void SetDefaults()
		{
			Name = "Lisa";
			Element = GenshinElement.ELECTRO;
			WeaponType = WeaponType.CATALYST;
			AbilityNormal = new LisaAbilityNormal().Initialize(this);
			AbilityCharged = new LisaAbilityNormal().Initialize(this);
			AbilitySkill = new LisaAbilityNormal().Initialize(this);
			AbilityBurst = new LisaAbilityNormal().Initialize(this);

			BaseAttack = 34;
			BaseDefense = 144;
			BaseHealth = 201;
		}
    }
}
