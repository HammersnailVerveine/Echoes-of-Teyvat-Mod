using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Abilities;

namespace GenshinMod.Content.Characters.Amber
{
    public class CharacterAmber : GenshinCharacter
    {
        public override void SetDefaults()
        {
            Name = "Amber";
            Element = GenshinElement.PYRO;
            WeaponType = WeaponType.BOW;
            AbilityNormal = new AbilitySwordNormal().Initialize(this);
            AbilityCharged = new AbilityBowCharged().Initialize(this);
            AbilitySkill = new AbilitySwordNormal().Initialize(this);
            AbilityBurst = new AbilitySwordNormal().Initialize(this);
            Autoswing = true;

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
