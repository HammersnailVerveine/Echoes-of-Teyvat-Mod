using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Content.Dusts
{
    public class LisaDustSpark : ModDust
    {
        public int direction = 0;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 10);
            direction = Main.rand.NextBool(2) ? -1 : 1;
            dust.rotation = Main.rand.NextFloat(0.5f) * direction;
        }

        public override Color? GetAlpha(Dust dust, Color drawColor)
        {
            return Color.White * dust.scale * 0.75f;
        }

        public override bool Update(Dust dust)
        {
            if (dust.scale < 0.15f) dust.active = false;

            dust.rotation += 0.1f * direction;
            dust.position += dust.velocity;

            dust.scale *= 0.95f;
            dust.velocity *= 0.95f;


            float light = 0.2f * dust.scale;
            Lighting.AddLight(dust.position, light * 2f, light, light * 2f);

            return false;
        }
    }
}
