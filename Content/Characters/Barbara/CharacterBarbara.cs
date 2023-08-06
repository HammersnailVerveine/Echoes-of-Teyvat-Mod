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
            AbilityNormal = new AbilityBarbaraNormal().Initialize(this);
            AbilityCharged = new AbilityBarbaraCharged().Initialize(this);
            AbilitySkill = new AbilityBarbaraSkill().Initialize(this);
            AbilityBurst = new AbilityBarbaraBurst().Initialize(this);

            BaseHealthMax = 9787;
            BaseAttackMax = 159;
            BaseDefenseMax = 669;

            BurstQuotes[0] = "♪La la-la-la-la~";
            BurstQuotes[1] = "Ready, steady, go!";
            BurstQuotes[2] = "Come on, we can do it!";

            //TryEquipWeapon(Common.ModObjects.Weapons.GenshinWeapon.GetWeapon(Terraria.ModLoader.ModContent.ItemType<Weapons.Catalyst.CatalystThrillingTales>()));
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
