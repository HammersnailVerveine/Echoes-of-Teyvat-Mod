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

            BaseHealthMax = 10287;
            BaseAttackMax = 311;
            BaseDefenseMax = 615;

            BurstQuotes[0] = "Sparks 'n' Splash!";
            BurstQuotes[1] = "Blazing Delight!";
            BurstQuotes[2] = "Blow them aaaalll up!";
        }
    }
}
