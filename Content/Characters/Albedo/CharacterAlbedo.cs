using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Content.Abilities;
using GenshinMod.Content.Characters.Albedo.Abilities;
using GenshinMod.Content.Characters.Albedo.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Albedo
{
    public class CharacterAlbedo : GenshinCharacter
	{
		public bool skillActive;
		public int skillCooldown = 0;

		public override void SetDefaults()
		{
			Name = "Albedo";
			Element = GenshinElement.GEO;
			WeaponType = WeaponType.SWORD;
			AbilityNormal = new AbilitySwordNormal().Initialize(this);
			AbilityCharged = new AbilitySwordCharged().Initialize(this);
			AbilitySkill = new AlbedoAbilitySkill().Initialize(this);
			AbilityBurst = new AlbedoAbilitySkill().Initialize(this);
			Autoswing = true;

			BaseHealthMax = 13226;
			BaseAttackMax = 251;
			BaseDefenseMax = 876;
		}

		public override void SafeResetEffects()
		{
			skillCooldown--;
			int type = ModContent.ProjectileType<AlbedoProjectileSkillMain>();
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
