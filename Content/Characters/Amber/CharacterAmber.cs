using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Abilities;
using GenshinMod.Content.Characters.Amber.Abilities;

namespace GenshinMod.Content.Characters.Amber
{
    public class CharacterAmber : GenshinCharacter
    {
        public override void SetDefaults()
        {
            Name = "Amber";
            Element = GenshinElement.PYRO;
            WeaponType = WeaponType.BOW;
            AbilityNormal = new AbilityBowNormal().Initialize(this);
            AbilityCharged = new AbilityBowCharged().Initialize(this);
            AbilitySkill = new AbilityAmberSkill().Initialize(this);
            AbilityBurst = new AbilitySwordNormal().Initialize(this);

            BaseHealthMax = 9461;
            BaseAttackMax = 223;
            BaseDefenseMax = 601;

            BurstQuotes[0] = "No one escapes my sights!";
            BurstQuotes[1] = "Let it rain!";
            BurstQuotes[2] = "No escape!";
        }

        public override void SafePostUpdate()
        {
            //Infuse(GenshinElement.ANEMO, 2);
        }

        public override void SafeResetEffects()
        {
        }
    }
}
