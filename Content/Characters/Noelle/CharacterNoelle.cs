using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Abilities;
using GenshinMod.Content.Characters.Noelle.Abilities;

namespace GenshinMod.Content.Characters.Noelle
{
    public class CharacterNoelle : GenshinCharacter
	{
		public int BurstTimer = 0;
		public int HealTimer = 0;

        public override void SetDefaults()
		{
			Name = "Noelle";
			Element = GenshinElement.GEO;
			WeaponType = WeaponType.CLAYMORE;
			AbilityNormal = new AbilityNoelleNormal().Initialize(this);
			AbilityCharged = new AbilityNoelleCharged().Initialize(this);
			AbilitySkill = new NoelleAbilitySkill().Initialize(this);
			AbilityBurst = new NoelleAbilityBurst().Initialize(this);
			Autoswing = true;

			BaseHealthMax = 12071;
			BaseAttackMax = 191;
			BaseDefenseMax = 799;
		}

        public override void SafePostUpdate()
        {
			if (BurstTimer > 0)
			{
				StatAttackFlat += AbilityBurst.GetScaling2();
				WeaponSize += 0.33f;
				Infuse(GenshinElement.GEO, 2, false);
			}
			else Infuse(GenshinElement.DENDRO, 2, false);
		}

        public override void SafeResetEffects()
		{
			HealTimer--;
			BurstTimer--;
		}
    }
}
