using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Characters.Barbara.Abilities;
using GenshinMod.Content.Characters.Barbara.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Barbara
{
    public class CharacterBarbara : GenshinCharacter
	{
		public bool skillActive;

        public override void SetDefaults()
		{
			Name = "Barbara";
			Element = GenshinElement.HYDRO;
			WeaponType = WeaponType.CATALYST;
			AbilityNormal = new BarbaraAbilityNormal().Initialize(this);
			AbilityCharged = new BarbaraAbilityCharged().Initialize(this);
			AbilitySkill = new BarbaraAbilitySkill().Initialize(this);
			AbilityBurst = new BarbaraAbilityBurst().Initialize(this);

			BaseAttack = 34;
			BaseDefense = 144;
			BaseHealth = 201;

			TryEquipWeapon(Common.ModObjects.Weapons.GenshinWeapon.GetWeapon(Terraria.ModLoader.ModContent.ItemType<Weapons.Catalyst.CatalystThrillingTales>()));
		}

        public override void SafePostUpdate()
        {
			if (GenshinPlayer.Timer % 600 == 0) GainEnergyFlat(1f);
        }

        public override void SafeResetEffects()
		{
			int type = ModContent.ProjectileType<BarbaraProjectileSkillCircle>();
			foreach (Projectile projectile in Main.projectile)
			{
				if (projectile.active && projectile.type == type && projectile.owner == Player.whoAmI)
				{
					skillActive = true;
					return;
				}
			}
			skillActive = false;
		}
    }
}
