﻿using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Characters.Klee.Abilities;

namespace GenshinMod.Content.Characters.Kaeya
{
    public class CharacterKaeya : GenshinCharacter
	{
        public override void SetDefaults()
		{
			Name = "Kaeya";
			Element = GenshinElement.CRYO;
			WeaponType = WeaponType.CATALYST;
			AbilityNormal = new KleeAbilityNormal().Initialize(this);
			AbilityCharged = new KleeAbilityCharged().Initialize(this);
			AbilitySkill = new KleeAbilitySkill().Initialize(this);
			AbilityBurst = new KleeAbilityBurst().Initialize(this);

			BaseAttack = 63;
			BaseDefense = 124;
			BaseHealth = 207;
		}
    }
}
