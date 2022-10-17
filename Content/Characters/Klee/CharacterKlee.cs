using GenshinMod.Common.GameObjects;
using GenshinMod.Content.Characters.Klee.Abilities;

namespace GenshinMod.Content.Characters.Klee
{
    public class CharacterKlee : GenshinCharacter
	{
        public override void SetDefaults()
		{
			Name = "Klee";
			Element = GenshinElement.PYRO;
			AbilityNormal = new KleeAbilityNormal().Initialize(this);
			AbilityCharged = new KleeAbilityCharged().Initialize(this);
			AbilitySkill = new KleeAbilitySkill().Initialize(this);
			AbilityBurst = new KleeAbilityBurst().Initialize(this);

			FlatHealth = 100;
			Energy = 0;
		}
    }
}
