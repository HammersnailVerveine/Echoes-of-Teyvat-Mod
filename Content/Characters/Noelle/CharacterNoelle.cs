using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Abilities;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Noelle
{
    public class CharacterNoelle : GenshinCharacter
	{
        public override void SetDefaults()
		{
			Name = "Noelle";
			Element = GenshinElement.GEO;
			WeaponType = WeaponType.CLAYMORE;
			AbilityNormal = new AbilitySwordNormal().Initialize(this);
			AbilityCharged = new AbilitySwordCharged().Initialize(this);
			AbilitySkill = new AbilitySwordNormal().Initialize(this);
			AbilityBurst = new AbilitySwordNormal().Initialize(this);
			Autoswing = true;

			BaseHealthMax = 12071;
			BaseAttackMax = 191;
			BaseDefenseMax = 799;
		}

        public override void SafePostUpdate()
        {
        }

        public override void SafeResetEffects()
		{
		}
    }
}
