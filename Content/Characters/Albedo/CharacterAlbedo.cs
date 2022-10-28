using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Characters.Kaeya.Abilities;

namespace GenshinMod.Content.Characters.Albedo
{
    public class CharacterAlbedo : GenshinCharacter
	{
        public override void SetDefaults()
		{
			Name = "Albedo";
			Element = GenshinElement.GEO;
			WeaponType = WeaponType.SWORD;
			AbilityNormal = new KaeyaAbilityNormal().Initialize(this);
			AbilityCharged = new KaeyaAbilityCharged().Initialize(this);
			AbilitySkill = new KaeyaAbilitySkill().Initialize(this);
			AbilityBurst = new KaeyaAbilityBurst().Initialize(this);
			Autoswing = true;

			BaseHealthMax = 13226;
			BaseAttackMax = 251;
			BaseDefenseMax = 876;
		}
    }
}
