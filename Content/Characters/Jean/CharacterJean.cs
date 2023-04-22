using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Abilities;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Jean
{
    public class CharacterJean : GenshinCharacter
	{
        public override void SetDefaults()
		{
			Name = "Jean";
			Element = GenshinElement.ANEMO;
			WeaponType = WeaponType.SWORD;
			AbilityNormal = new AbilitySwordNormal().Initialize(this);
			AbilityCharged = new AbilitySwordCharged().Initialize(this);
			AbilitySkill = new AbilitySwordNormal().Initialize(this);
			AbilityBurst = new AbilitySwordNormal().Initialize(this);
			Autoswing = true;

			BaseHealthMax = 14695;
			BaseAttackMax = 239;
			BaseDefenseMax = 769;
		}

        public override void SafePostUpdate()
		{
			Infuse(GenshinElement.ANEMO, 2, false);
		}

        public override void SafeResetEffects()
		{
		}
    }
}
