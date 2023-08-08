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
            AbilityNormal = new AbilityKleeNormal().Initialize(this);
            AbilityCharged = new AbilityKleeCharged().Initialize(this);
            AbilitySkill = new AbilityKleeSkill().Initialize(this);
            AbilityBurst = new AbilityKleeBurst().Initialize(this);

            BaseHealthMax = 10287;
            BaseAttackMax = 311;
            BaseDefenseMax = 615;

            BurstQuotes[0] = "Sparks 'n' Splash!";
            BurstQuotes[1] = "Blazing Delight!";
            BurstQuotes[2] = "Blow them aaaalll up!";

            HeightOffset = -6;
        }
    }
}
