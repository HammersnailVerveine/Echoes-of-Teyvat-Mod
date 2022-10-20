using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects.Weapons.Projectiles;

namespace GenshinMod.Common.ModObjects.Weapons
{
	public abstract class WeaponCatalyst : GenshinWeapon
	{
		public sealed override void SafeSetDefaultsWeaponType()
		{
			WeaponType = WeaponType.CATALYST;
		}

		public sealed override void WeaponPostUpdateActiveWeaponType()
		{
			int catalystType = ModContent.ProjectileType<WeaponAnchor>();

			if (Player.ownedProjectileCounts[catalystType] == 0)
			{
				var index = Projectile.NewProjectile(null, Player.Center, Vector2.Zero, catalystType, 0, 0f, Player.whoAmI);

				var proj = Main.projectile[index];
				if (!(proj.ModProjectile is WeaponAnchor catalyst)) proj.Kill();
				else catalyst.OnChangeEquipedItem(this);
			}
			else
			{
				var proj = Main.projectile.First(i => i.active && i.owner == Player.whoAmI && i.type == catalystType);
				if (proj != null && proj.ModProjectile is WeaponAnchor catalyst)
				{
					if (catalyst.Weapon != Character.Weapon) catalyst.OnChangeEquipedItem(this);
				}
			}
		}

		public override void KillProjectile()
		{
			var anchorType = ModContent.ProjectileType<WeaponAnchor>();

			var proj = Main.projectile.First(i => i.active && i.owner == Player.whoAmI && i.type == anchorType);
			if (proj != null && proj.ModProjectile is WeaponAnchor catalyst)
				catalyst.Kill(proj.timeLeft);
		}
	}
}
