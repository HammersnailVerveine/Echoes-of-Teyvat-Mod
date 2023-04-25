﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Content.Dusts
{
    public class KleeSparkleDustBigRed : ModDust
    {
        public int direction = 0;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(2) * 14, 14, 14);
            direction = Main.rand.NextBool(2) ? -1 : 1;
            dust.rotation = Main.rand.NextFloat((float)Math.PI * 2f);
        }

        public override Color? GetAlpha(Dust dust, Color drawColor)
        {
            return Color.White * dust.scale * 2f;
        }

        public override bool Update(Dust dust)
        {
            if (dust.scale < 0.15f) dust.active = false;

            dust.rotation += 0.025f * direction;
            dust.position += dust.velocity;

            dust.scale *= 0.95f;
            dust.velocity *= 0.9f;


            float light = 0.4f * dust.scale;
            Lighting.AddLight(dust.position, light * 1.2f, light * 0.75f, light * 0.75f);

            return false;
        }
    }
}
