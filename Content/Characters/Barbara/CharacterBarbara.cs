using GenshinMod.Common.GameObjects;
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
			AbilityNormal = new BarbaraAbilityNormal().Initialize(this);
			AbilityCharged = new BarbaraAbilityCharged().Initialize(this);
			AbilitySkill = new BarbaraAbilitySkill().Initialize(this);
			AbilityBurst = new BarbaraAbilityBurst().Initialize(this);

			Health = 2;
		}

        public override void SafePostUpdate()
        {
			if (GenshinPlayer.Timer % 600 == 0) GainEnergyFlat(1f);
        }

        public override void SafePreUpdate()
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

        public override void SafeResetEffects()
        {
		}
    }
}
