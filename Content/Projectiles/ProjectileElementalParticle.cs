using Microsoft.Xna.Framework;
using GenshinMod.Content.Dusts;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.GameObjects;

namespace GenshinMod.Content.Projectiles
{
    public class ProjectileElementalParticle : GenshinProjectile
	{
		public Color GlowColor;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Elemental Energy");
		}

		public override Color? GetAlpha(Color lightColor) => GlowColor * 0.75f;

		public override void SetDefaults()
		{
			Projectile.width = 12;
			Projectile.height = 12;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 600;
			ProjectileTrail = true;
		}

		public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2f);
			Projectile.velocity += new Vector2(Main.rand.NextFloat(3f, 5f)).RotatedByRandom(MathHelper.ToRadians(360));
			GlowColor = GetColor(GetElement());
		}

        public override void SafeAI()
		{
			Player player = Main.LocalPlayer;
			if (!player.dead)
            {
				Vector2 direction = player.Center - Projectile.Center;
				if (direction.Length() < player.width)
                {
					player.GetModPlayer<GenshinPlayer>().GiveTeamEnergy(GetElement(), Projectile.ai[1]);
					Projectile.active = false;
                }

				direction.Normalize();
				direction *= 1.2f;
				Projectile.velocity += direction;
				if (Projectile.velocity.Length() > 7f)
				{
					Projectile.velocity.Normalize();
					Projectile.velocity *= 5f;
				}
			}
			Projectile.rotation += 0.2f;
			Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 0.3f);
		}

        public override void Kill(int timeLeft)
        {
		}

		public CharacterElement GetElement()
        {
			switch (Projectile.ai[0])
            {
				case 1f:
					return CharacterElement.GEO;
				case 2f:
					return CharacterElement.ANEMO;
				case 3f:
					return CharacterElement.CRYO;
				case 4f:
					return CharacterElement.ELECTRO;
				case 5f:
					return CharacterElement.DENDRO;
				case 6f:
					return CharacterElement.HYDRO;
				case 7f:
					return CharacterElement.PYRO;
				default:
					return CharacterElement.NONE;
            }
        }

		public Color GetColor(CharacterElement element)
		{
			switch (element)
			{
				case CharacterElement.GEO:
					return new Color(255, 167, 45);
				case CharacterElement.ANEMO:
					return new Color(79, 255, 202);
				case CharacterElement.CRYO:
					return new Color(104, 209, 255);
				case CharacterElement.DENDRO:
					return new Color(146, 255, 50);
				case CharacterElement.ELECTRO:
					return new Color(162, 96, 255);
				case CharacterElement.HYDRO:
					return new Color(30, 139, 255);
				case CharacterElement.PYRO:
					return new Color(255, 102, 68);
				default:
					return new Color(235, 235, 255);
			}
		}
	}
}
