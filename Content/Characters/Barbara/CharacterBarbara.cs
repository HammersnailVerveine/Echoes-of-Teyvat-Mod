using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Barbara.Abilities;

namespace GenshinMod.Content.Characters.Barbara
{
    public class CharacterBarbara : GenshinCharacter
	{
        public override void SetDefaults()
		{
			Element = CharacterElement.HYDRO;
			AbilityNormal = new BarbaraAbilityNormal().Initialize(this);
			AbilityCharged = new BarbaraAbilityCharged().Initialize(this);
			AbilitySkill = new BarbaraAbilitySkill().Initialize(this);
			AbilityBurst = new BarbaraAbilityBurst().Initialize(this);
		}
	}
}
