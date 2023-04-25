using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects.Weapons.Projectiles;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Common.ModObjects.Weapons
{
    public abstract class WeaponCatalyst : GenshinWeapon
    {
        public sealed override void SafeSetDefaultsWeaponType()
        {
            WeaponType = WeaponType.CATALYST;
        }

        public sealed override void SpawnVanityWeapon()
        {
            int anchorType = ModContent.ProjectileType<WeaponAnchor>();

            if (Player.ownedProjectileCounts[anchorType] == 0)
            {
                Vector2 position = Player.Center - new Vector2(32f * Player.direction, 16f);
                var index = Projectile.NewProjectile(null, position, Vector2.Zero, anchorType, 0, 0f, Player.whoAmI);

                var proj = Main.projectile[index];
                if (!(proj.ModProjectile is WeaponAnchor anchor)) proj.Kill();
                else anchor.OnChangeEquipedItem(this);
            }
            else
            {
                var proj = Main.projectile.First(i => i.active && i.owner == Player.whoAmI && i.type == anchorType);
                if (proj != null && proj.ModProjectile is WeaponAnchor anchor)
                {
                    if (anchor.Weapon != Character.Weapon) anchor.OnChangeEquipedItem(this);
                }
            }
        }
    }
}
