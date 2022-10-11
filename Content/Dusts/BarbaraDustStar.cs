using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Content.Dusts
{
	public class BarbaraDustStar : ModDust
	{
		public int direction = 0;

		public override void OnSpawn(Dust dust)
		{
			dust.frame = new Rectangle(0, Main.rand.Next(4) * 14, 14, 14);
			direction = Main.rand.NextBool(2) ? -1 : 1;
			dust.rotation = Main.rand.NextFloat(0.5f) * direction;
		}

		public override Color? GetAlpha(Dust dust, Color drawColor)
		{
			return Color.White * dust.scale * 2f;
		}

		public override bool Update(Dust dust)
		{
			if (dust.scale < 0.15f) dust.active = false;

			dust.rotation += 0.001f * direction;
			dust.position += dust.velocity;

			dust.scale *= 0.95f;
			dust.velocity *= 0.95f;


			float light = 0.3f * dust.scale;
			Lighting.AddLight(dust.position, light, light, light);

			return false;
		}
	}
}
