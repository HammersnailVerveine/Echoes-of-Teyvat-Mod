using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Content.Dusts
{
	public class HypostasisGeoDustSmall : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.frame = new Rectangle(0, Main.rand.Next(2) * 14, 14, 14);
			dust.rotation = Main.rand.NextFloat((float)Math.PI * 2f);
		}

		public override Color? GetAlpha(Dust dust, Color drawColor)
		{
			return Color.White * dust.scale;
		}

		public override bool Update(Dust dust)
		{
			if (dust.scale < 0.15f) dust.active = false;

			dust.rotation += 0.025f;
			dust.position += dust.velocity;

			dust.scale *= 0.985f;
			dust.velocity *= 0.75f;


			float light = 0.4f * dust.scale;
			Lighting.AddLight(dust.position, light * 1.2f, light * 1f, light * 0.4f);

			return false;
		}
	}
}
