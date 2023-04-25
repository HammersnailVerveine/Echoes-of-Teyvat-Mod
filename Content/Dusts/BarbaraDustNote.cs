using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Content.Dusts
{
    public class BarbaraDustNote : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 14, 10, 14);
            dust.rotation = Main.rand.NextFloat(0.1f) * (Main.rand.NextBool(3) ? -1 : 1);
        }

        public override Color? GetAlpha(Dust dust, Color drawColor)
        {
            return Color.White * dust.scale * 2f;
        }

        public override bool Update(Dust dust)
        {
            if (dust.scale < 0.15f) dust.active = false;

            dust.position += dust.velocity;

            dust.scale *= 0.98f;
            dust.velocity *= 0.95f;


            float light = 0.2f * dust.scale;
            Lighting.AddLight(dust.position, light, light, light * 2f);

            return false;
        }
    }
}
