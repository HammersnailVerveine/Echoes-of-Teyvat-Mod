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

            GaugeUnit = 16;
            KnockBackResist = 1f;
            ShieldResistance = 1f;
        }

        /*
        public void DrawTexture(Texture2D texture, SpriteBatch spriteBatch, NPC npc, int nbElements, ref int offSetX, ref int offSetY, int timeLeft)
        {
            float colorMult = timeLeft > 120 ? 1f : (float)Math.Abs(Math.Sin((timeLeft * 0.5f) / Math.PI / 4f));
            Vector2 position = new Vector2(npc.Center.X + offSetX - Main.screenPosition.X, npc.Center.Y + offSetY - Main.screenPosition.Y);
            spriteBatch.Draw(texture, position, null, Color.White * colorMult, 0f, texture.Size() * 0.5f, 0.875f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position, null, Color.White * 0.5f * colorMult, 0f, texture.Size() * 0.5f, 1.025f, SpriteEffects.None, 0f);
            offSetX += 24;
            if (offSetX > 24) setOffset(ref offSetX, ref offSetY, ref nbElements);
        }
        */

        public override void Draw(SpriteBatch spriteBatch, Color lightColor, NPC npc)
        {
            Vector2 drawPosition = (npc.position + new Vector2(npc.width * 0.5f, npc.height * 0.5f + npc.gfxOffY)).Floor();
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

