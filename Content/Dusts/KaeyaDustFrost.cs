using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Content.Dusts
{
    public class KaeyaDustFrost : ModDust
    {
        public int direction = 0;
        public float rotationSpeed = 0f;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 10);
            direction = Main.rand.NextBool(2) ? -1 : 1;
            dust.rotation = Main.rand.NextFloat(0.5f) * direction;
            rotationSpeed = Main.rand.NextFloat(0.1f);
        }

        public override Color? GetAlpha(Dust dust, Color drawColor)
        {
            return Color.White * dust.scale * 0.75f;
        }

        public override bool Update(Dust dust)
        {
            if (dust.scale < 0.15f) dust.active = false;

            dust.rotation += rotationSpeed * direction;
            dust.position += dust.velocity;

            dust.scale *= 0.95f;
            dust.velocity *= 0.95f;


            float light = 0.3f * dust.scale;
            Lighting.AddLight(dust.position, light, light, light);

            return false;
        }
    }
}
