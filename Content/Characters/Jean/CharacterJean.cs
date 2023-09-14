using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Abilities;
using GenshinMod.Content.Characters.Jean.Abilities;

namespace GenshinMod.Content.Characters.Jean
{
    public class CharacterJean : GenshinCharacter
    {
        public override void SetDefaults()
        {
            Name = "Jean";
            Rarity = GenshinRarity.FIVESTAR;
            Element = GenshinElement.ANEMO;
            WeaponType = WeaponType.SWORD;
            AbilityNormal = new AbilitySwordNormal().Initialize(this);
            AbilityCharged = new AbilityJeanCharged().Initialize(this);
            AbilitySkill = new AbilityJeanSkill().Initialize(this);
            AbilityBurst = new AbilityJeanBurst().Initialize(this);

            BaseHealthMax = 14695;
            BaseAttackMax = 239;
            BaseDefenseMax = 769;

            BurstQuotes[0] = "Wind, hear me!";
            BurstQuotes[1] = "I swear by my sword!";
            BurstQuotes[2] = "Barbatos, guide us";

            HeightOffset = 0;
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
