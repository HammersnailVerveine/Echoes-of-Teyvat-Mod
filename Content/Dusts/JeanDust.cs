using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Content.Dusts
{
    public class JeanDust : ModDust
    {
        public int direction = 0;
        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(2) * 10, 10, 10);
            direction = Main.rand.NextBool(2) ? -1 : 1;
            dust.rotation = Main.rand.NextFloat((float)Math.PI * 2f);
            dust.scale *= Main.rand.NextFloat(1, 1.25f);
        }

        public override Color? GetAlpha(Dust dust, Color drawColor)
        {
            return Color.White * dust.scale * 0.5f;
        }

        public override bool Update(Dust dust)
        {
            if (dust.scale < 0.4f) dust.active = false;

            dust.rotation = (float)Math.Sin(dust.scale * 20f) * 0.5f;
            dust.position += dust.velocity;

            dust.scale *= 0.995f;
            dust.velocity *= 0.95f;
            dust.velocity.Y += 0.0025f;

            return false;
        }
    }
}
