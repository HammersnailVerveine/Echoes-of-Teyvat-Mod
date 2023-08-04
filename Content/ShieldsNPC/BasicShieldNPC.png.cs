using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Content.ShieldsNPC
{
    public class BasicShieldNPC : GenshinShieldNPC
    {
        private static Texture2D TextureShieldOut;
        private static Texture2D TextureShieldIn;
        private Color GlowColor;

        public override void OnInitialize(ref GenshinElement element, int value)
        {
            TextureShieldOut ??= ModContent.Request<Texture2D>("GenshinMod/Content/Shields/ShieldCrystallize_Out", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureShieldIn ??= ModContent.Request<Texture2D>("GenshinMod/Content/Shields/ShieldCrystallize_In", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            GlowColor = GenshinElementUtils.GetColor(element);

            GaugeUnit = 32;
            KnockBackResist = 1f;
            ShieldResistance = 1f;
        }

        public override void Draw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawPosition = (NPC.position + new Vector2(NPC.width * 0.5f, NPC.height * 0.5f + NPC.gfxOffY)).Floor();
            drawPosition = Vector2.Transform(drawPosition - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
            //Vector2 drawPosition = new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y);

            float lightFactor = ((float)Math.Sin(TimeSpent * 0.05f) * 0.2f + 0.9f);
            float scaleMult = ((float)Math.Sin(- TimeSpent * 0.02f)) * 0.025f + 1.04f;
            spriteBatch.Draw(TextureShieldIn, drawPosition, null, GlowColor * 0.45f * lightFactor, 0f, TextureShieldIn.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureShieldOut, drawPosition, null, GlowColor * 0.2f * lightFactor, 0f, TextureShieldIn.Size() * 0.5f, 1f * scaleMult, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureShieldOut, drawPosition, null, GlowColor * 0.55f * lightFactor, 0f, TextureShieldIn.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
        }
    }
}

