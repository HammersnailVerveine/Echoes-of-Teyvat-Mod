using GenshinMod.Common.GameObjects;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace GenshinMod.Common.UI.UIs
{
    public class UIStateHealth : GenshinUIState
    {
        public static Texture2D TextureBackground;
        public static Texture2D TextureTeamHealth;

        public override int InsertionIndex(List<GameInterfaceLayer> layers)
            => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public override void OnInitialize()
        {
            TextureBackground ??= ModContent.Request<Texture2D>("GenshinMod/Common/UI/UIs/Textures/TeamHealthBackground", AssetRequestMode.ImmediateLoad).Value;
            TextureTeamHealth ??= ModContent.Request<Texture2D>("GenshinMod/Common/UI/UIs/Textures/TeamHealthBar", AssetRequestMode.ImmediateLoad).Value;

            Width.Set(0f, 0f);
            Height.Set(0f, 0f);
            Left.Set(Main.screenWidth - 48f, 0f);
            Top.Set(Main.screenHeight * 0.2f, 0f);
        }

        public override void OnUIScaleChanged()
        {
            Left.Set(Main.screenWidth - 48f, 0f);
            Top.Set(Main.screenHeight * 0.2f, 0f);
        }

        public override void OnResolutionChanged(int width, int height)
        {
            Left.Set(Main.screenWidth - 48f, 0f);
            Top.Set(Main.screenHeight * 0.2f, 0f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Recalculate();
            Player player = Main.LocalPlayer;
            GenshinPlayer genshinPlayer = player.GetModPlayer<GenshinPlayer>();
            GenshinCharacter characterCurrent = genshinPlayer.CharacterCurrent;
            CalculatedStyle dimensions = GetDimensions();

            int offsetY = 0;
            foreach (GenshinCharacter character in genshinPlayer.CharacterTeam)
            {
                bool active = character == characterCurrent;
                float scale = active ? 1.2f : 1f;

                // Background
                Vector2 backgroundPosition = new Vector2(dimensions.X - (active ? 7f : 0f), dimensions.Y + offsetY);
                spriteBatch.Draw(TextureBackground, backgroundPosition, null, Color.White * 0.2f, 0f, TextureBackground.Size() * 0.5f, scale, SpriteEffects.None, 0f);

                // Character Icon
                Texture2D Icon = character.TextureIcon;
                Vector2 iconPosition = new Vector2(dimensions.X - (active ? 7f : 0f), dimensions.Y + offsetY);
                Color iconColor = active ? Color.White : Color.LightGray * 0.8f;
                spriteBatch.Draw(Icon, iconPosition, null, Color.Black * 0.1f, 0f, Icon.Size() * 0.5f, 1.05f * scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(Icon, iconPosition, null, iconColor, 0f, Icon.Size() * 0.5f, 0.9f * scale, SpriteEffects.None, 0f);

                //Character name
                string text = character.Name;
                Vector2 textPosition = new Vector2(dimensions.X - ((active ? 7f : 0f) + text.Length * 11f + Icon.Width / 2f + 4) * scale, dimensions.Y + offsetY - Icon.Height * 0.5f);
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, textPosition, iconColor, 0f, Vector2.Zero, new Vector2(scale, scale));

                // Health Bar

                Vector2 healthPosition = new Vector2(dimensions.X - ((active ? 7f : 0f) + 80) * scale, textPosition.Y + 34 * scale);
                Color healthBackgroundColor = new Color(20, 50, 20) * (active ? 1f : 0.9f);
                Color healthColor = new Color(115, 200, 0) * (active ? 1f : 0.9f);
                spriteBatch.Draw(TextureTeamHealth, healthPosition, null, healthBackgroundColor * 0.5f, 0f, TextureTeamHealth.Size() * 0.5f, scale * 1.25f, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureTeamHealth, healthPosition, null, healthBackgroundColor, 0f, TextureTeamHealth.Size() * 0.5f, scale, SpriteEffects.None, 0f);
                Rectangle rectangle = new Rectangle(0, 0, 0, TextureTeamHealth.Height);

                float increment = character.EffectiveHealth / (TextureTeamHealth.Width * 0.5f);
                float health = 0;
                while (health < character.Health - increment)
                {
                    rectangle.Width += 2;
                    health += increment;
                }
                if (character.IsAlive) rectangle.Width += 2;
                if (rectangle.Width > TextureTeamHealth.Width) rectangle.Width = TextureTeamHealth.Width;
                spriteBatch.Draw(TextureTeamHealth, healthPosition, rectangle, healthColor, 0f, TextureTeamHealth.Size() * 0.5f, scale, SpriteEffects.None, 0f);

                // Burst

                if (character.Energy >= character.AbilityBurst.Energy && !active)
                {
                    Texture2D burstBackground = UIStateAbility.TextureBurstEnergy;
                    Vector2 burstPosition = new Vector2(healthPosition.X - 80 * scale, textPosition.Y + 20 * scale);

                    spriteBatch.Draw(burstBackground, burstPosition, null, Color.Black * 0.2f, 0f, burstBackground.Size() * 0.5f, scale * 0.45f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(burstBackground, burstPosition, null, Color.White * 0.2f, 0f, burstBackground.Size() * 0.5f, scale * 0.395f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(burstBackground, burstPosition, null, UIStateAbility.GetBurstColor(character) * 0.8f, 0f, burstBackground.Size() * 0.5f, scale * 0.35f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(character.TextureAbilityBurst, burstPosition, null, Color.White * 0.8f, 0f, character.TextureAbilityBurst.Size() * 0.5f, scale * 0.315f, SpriteEffects.None, 0f);
                }

                offsetY += 80;
            }

        }
    }
}