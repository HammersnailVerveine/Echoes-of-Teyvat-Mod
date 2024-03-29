using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;

namespace GenshinMod.Common.Static
{
    class MethodSwaps
    {
        internal static void ApplyMethodSwaps()
        {
            On_Main.DrawMenu += DrawMenuUI;
        }

        public static void DrawMenuUI(On_Main.orig_DrawMenu orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);

            if (Main.menuMode == 0)
            {
                
                Main.spriteBatch.Begin();
                string text = "Remember to download the map from the Workshop page!";
                Vector2 textPosition = new Vector2(Main.screenWidth / 2f * Main.UIScale - FontAssets.MouseText.Value.MeasureString(text).X, Main.screenHeight / 3f * 2.3f * Main.UIScale);
                Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(textPosition.X + 4, textPosition.Y + 4), Color.Black, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
                Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, textPosition, Color.Red, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);

                text = "Play with a new character, your equipment WILL BE DELETED";
                textPosition = new Vector2(Main.screenWidth / 2f * Main.UIScale - FontAssets.MouseText.Value.MeasureString(text).X, Main.screenHeight / 3f * 2.3f * Main.UIScale + FontAssets.MouseText.Value.MeasureString(text).Y + 15);
                Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(textPosition.X + 4, textPosition.Y + 4), Color.Black, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
                Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, textPosition, Color.Red, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
                Main.spriteBatch.End();
            }
        }
    }
}