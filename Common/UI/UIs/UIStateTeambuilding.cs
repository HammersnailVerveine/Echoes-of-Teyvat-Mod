using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace GenshinMod.Common.UI.UIs
{
    public class UIStateTeambuilding : GenshinUIState
    {
        private static Color BackgroundBlue = new Color(26, 46, 85);
        private static Color BackgroundBlack = new Color(26, 26, 26);
        private static Color BackgroundRed = new Color(100, 30, 45);
        private static Color LightBlue = new Color(44, 75, 126);

        public static int SelectedSlot = -5;
        public static List<GenshinCharacter> PlayerTeam;
        public static List<GenshinCharacter> PlayerCharacters;

        public static bool Available = true;

        public override int InsertionIndex(List<GameInterfaceLayer> layers)
            => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public override void OnInitialize()
        {
            Width.Set(0f, 0f);
            Height.Set(0f, 0f);
            Left.Set(Main.screenWidth / 2f, 0f);
            Top.Set(Main.screenHeight / 5f, 0f);
        }

        public override void OnUIScaleChanged()
        {
            Left.Set(Main.screenWidth / 2f, 0f);
            Top.Set(Main.screenHeight / 5f, 0f);
        }

        public override void OnResolutionChanged(int width, int height)
        {
            Left.Set(Main.screenWidth / 2f, 0f);
            Top.Set(Main.screenHeight / 5f, 0f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Recalculate();
            Player player = Main.LocalPlayer;

            if (Available)
            {
                UnlockablesPlayer unlockables = player.GetModPlayer<UnlockablesPlayer>();
                GenshinPlayer genshinPlayer = player.GetModPlayer<GenshinPlayer>();

                if (PlayerCharacters == null)
                {
                    PlayerCharacters = new List<GenshinCharacter>();
                    foreach (GenshinCharacter character in unlockables.UnlockedCharacters) PlayerCharacters.Add(character);
                }

                if (PlayerTeam == null)
                {
                    PlayerTeam = new List<GenshinCharacter>();
                    foreach (GenshinCharacter character in genshinPlayer.CharacterTeam) PlayerTeam.Add(character);
                }

                int nbCharacters = PlayerCharacters.Count;
                int nbLines = Math.DivRem(nbCharacters - PlayerTeam.Count, 5, out int rest);
                if (rest > 0) nbLines++;

                // Draw the lower UI, where the player character list is

                Rectangle characterProfile = new(0, 0, 46, 74);
                int characterProfileOffset = 8;
                int UiWidth = characterProfile.Width * 5 + characterProfileOffset * 6; // contains 5 character profiles horizontally
                int UIHeight = (characterProfile.Height + characterProfileOffset) * nbLines + characterProfileOffset; // contains 2 character profiles vertically
                Rectangle drawRect = new(Main.screenWidth / 2 - UiWidth / 2, Main.screenHeight / 3, UiWidth, UIHeight);

                DrawBackgroundRectangle(spriteBatch, drawRect, BackgroundBlue);

                for (int j = 0; j < nbLines; j++)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (nbCharacters > 0)
                        {
                            GenshinCharacter character = PlayerCharacters[nbCharacters - 1];
                            if (!PlayerTeam.Contains(character))
                            {
                                int offSetX = characterProfileOffset + i * (characterProfileOffset + characterProfile.Width);
                                int offSetY = characterProfileOffset + j * (characterProfileOffset + characterProfile.Height);
                                Rectangle rectangle = new Rectangle(drawRect.X + offSetX, drawRect.Y + offSetY, characterProfile.Width, characterProfile.Height);
                                DrawBackgroundRectangle(spriteBatch, rectangle, Color.Black * 0.5f);
                                DrawCharacterSheet(spriteBatch, rectangle, character);

                                if (SelectedSlot == nbCharacters)
                                    DrawSelectionRectangle(spriteBatch, rectangle);

                                if (rectangle.Contains(Main.MouseScreen.ToPoint()) && Main.mouseLeft && Main.mouseLeftRelease)
                                {
                                    if (SelectedSlot < 0 && SelectedSlot > -5)
                                    {
                                        int teamSlot = SelectedSlot + 4;
                                        if (PlayerTeam.Count > teamSlot)
                                            PlayerTeam[teamSlot] = character;
                                        else
                                            PlayerTeam.Add(character);
                                        SelectedSlot = -5;
                                        SoundEngine.PlaySound(SoundID.MenuOpen);
                                    }
                                    else
                                    {
                                        SelectedSlot = nbCharacters;
                                        SoundEngine.PlaySound(SoundID.MenuTick);
                                    }
                                }
                            }
                            else i--;
                        }

                        nbCharacters--;
                    }
                }

                // Draw the upper UI, where the player selected team is

                UiWidth = characterProfile.Width * 4 + characterProfileOffset * 5;
                UIHeight = characterProfile.Height + characterProfileOffset * 2;
                drawRect = new Rectangle(Main.screenWidth / 2 - UiWidth / 2, Main.screenHeight / 3 - UIHeight - characterProfileOffset, UiWidth, UIHeight);

                DrawBackgroundRectangle(spriteBatch, drawRect, BackgroundBlue);
                for (int i = 0; i < 4; i++)
                {
                    int offSetX = characterProfileOffset + i * (characterProfileOffset + characterProfile.Width);
                    Rectangle rectangle = new Rectangle(drawRect.X + offSetX, drawRect.Y + characterProfileOffset, characterProfile.Width, characterProfile.Height);
                    DrawBackgroundRectangle(spriteBatch, rectangle, Color.Black * 0.5f);

                    if (i < PlayerTeam.Count)
                        DrawCharacterSheet(spriteBatch, rectangle, PlayerTeam[i]);

                    if (SelectedSlot + 4 == i)
                        DrawSelectionRectangle(spriteBatch, rectangle);

                    if (rectangle.Contains(Main.MouseScreen.ToPoint()) && Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        if (SelectedSlot > 0)
                        {
                            if (i < PlayerTeam.Count) // there is a character in this slot, swap it
                                PlayerTeam[i] = PlayerCharacters[SelectedSlot - 1];
                            else
                                PlayerTeam.Add(PlayerCharacters[SelectedSlot - 1]);
                            SelectedSlot = -5;
                            SoundEngine.PlaySound(SoundID.MenuOpen);
                        }
                        else if (SelectedSlot > -5 && SelectedSlot + 4 < PlayerTeam.Count && SelectedSlot != i - 4 && i < PlayerTeam.Count)
                        {
                            GenshinCharacter buffer = PlayerTeam[SelectedSlot + 4];
                            PlayerTeam[SelectedSlot + 4] = PlayerTeam[i];
                            PlayerTeam[i] = buffer;
                            SelectedSlot = -5;
                            SoundEngine.PlaySound(SoundID.MenuOpen);
                        }
                        else
                        {
                            SelectedSlot = i - 4;
                            SoundEngine.PlaySound(SoundID.MenuTick);
                        }
                    }
                }

                // Draw Buttons

                bool differentTeam = false;
                if (PlayerTeam.Count == genshinPlayer.CharacterTeam.Count)
                {
                    for (int i = 0; i < PlayerTeam.Count; i++)
                    {
                        if (PlayerTeam[i] != genshinPlayer.CharacterTeam[i])
                        {
                            differentTeam = true;
                        }
                    }
                }
                else differentTeam = true;

                if (differentTeam && PlayerTeam.Count > 0) // Validate team button
                {
                    drawRect = new Rectangle(Main.screenWidth / 2 - UiWidth / 2 + characterProfileOffset, Main.screenHeight / 3 - UIHeight - characterProfileOffset * 2 - 30, UiWidth / 2 - characterProfileOffset * 2, 30);
                    DrawBackgroundRectangle(spriteBatch, drawRect, BackgroundBlue);

                    Vector2 textPosition = new Vector2(drawRect.X + 14, drawRect.Y + 5);
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Confirm", textPosition, Color.White, 0f, Vector2.Zero, new Vector2(1f, 1f));

                    if (drawRect.Contains(Main.MouseScreen.ToPoint()) && Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        ValidateTeamChange();
                        SelectedSlot = -5;
                        SoundEngine.PlaySound(SoundID.MenuOpen);
                    }
                }

                if (SelectedSlot > -5 && SelectedSlot < 0 && SelectedSlot + 4 < PlayerTeam.Count) // Remove button
                {
                    drawRect = new Rectangle(Main.screenWidth / 2 + characterProfileOffset, Main.screenHeight / 3 - UIHeight - characterProfileOffset * 2 - 30, UiWidth / 2 - characterProfileOffset * 2, 30);
                    DrawBackgroundRectangle(spriteBatch, drawRect, BackgroundBlue);

                    Vector2 textPosition = new Vector2(drawRect.X + 18, drawRect.Y + 5);
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Remove", textPosition, Color.White, 0f, Vector2.Zero, new Vector2(1f, 1f));

                    if (drawRect.Contains(Main.MouseScreen.ToPoint()) && Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        PlayerTeam.RemoveAt(SelectedSlot + 4);
                        SelectedSlot = -5;
                        SoundEngine.PlaySound(SoundID.MenuOpen);
                    }
                }
            }
            else
            {
                Rectangle drawRect = new(Main.screenWidth / 2 - 150, Main.screenHeight / 3, 300, 80);
                DrawBackgroundRectangle(spriteBatch, drawRect, BackgroundBlack);
                Vector2 textPosition = new Vector2(drawRect.X + 26, drawRect.Y + drawRect.Height / 2 - 12);
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Teambuilding unavailable", textPosition, Color.White, 0f, Vector2.Zero, new Vector2(1.2f, 1.2f));

                drawRect = new Rectangle(Main.screenWidth / 2 - 80, drawRect.Y - 38, 160, 30);
                DrawBackgroundRectangle(spriteBatch, drawRect, BackgroundBlue);
                textPosition = new Vector2(drawRect.X + 13, drawRect.Y + 5);
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Return to Spawn", textPosition, Color.White, 0f, Vector2.Zero, new Vector2(1f, 1f));

                if (drawRect.Contains(Main.MouseScreen.ToPoint()) && Main.mouseLeft && Main.mouseLeftRelease)
                {
                    player.Teleport(new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16), TeleportationStyleID.TeleporterTile);
                }
            }
        }

        public void ValidateTeamChange()
        {
            GenshinPlayer genshinPlayer = Main.LocalPlayer.GetModPlayer<GenshinPlayer>();
            GenshinCharacter selectedCharacter = genshinPlayer.CharacterCurrent;

            genshinPlayer.CharacterTeam.Clear();
            genshinPlayer.CharacterTeam = PlayerTeam;

            if (!PlayerTeam.Contains(selectedCharacter))
                genshinPlayer.CharacterCurrent = PlayerTeam[0];

            PlayerTeam = new List<GenshinCharacter>();
            foreach (GenshinCharacter character in genshinPlayer.CharacterTeam)
            {
                character.SetupFull();
                PlayerTeam.Add(character);
            }
        }

        public void DrawBackgroundRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color backgroundColor)
        {
            Texture2D MagicPixel = TextureAssets.MagicPixel.Value;

            // Draw background
            spriteBatch.Draw(MagicPixel, new Rectangle(rectangle.X - 2, rectangle.Y - 2, rectangle.Width + 4, rectangle.Height + 4), Color.Black * 0.5f);
            spriteBatch.Draw(MagicPixel, rectangle, backgroundColor * 0.9f);

            // Draw contour lines
            spriteBatch.Draw(MagicPixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, 2), LightBlue);
            spriteBatch.Draw(MagicPixel, new Rectangle(rectangle.X, rectangle.Y, 2, rectangle.Height), LightBlue);
            spriteBatch.Draw(MagicPixel, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - 2, rectangle.Width, 2), LightBlue);
            spriteBatch.Draw(MagicPixel, new Rectangle(rectangle.X + rectangle.Width - 2, rectangle.Y, 2, rectangle.Height), LightBlue);
        }

        public void DrawSelectionRectangle(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            Texture2D MagicPixel = TextureAssets.MagicPixel.Value;

            // Draw contour lines
            spriteBatch.Draw(MagicPixel, new Rectangle(rectangle.X + 2, rectangle.Y - 2, rectangle.Width - 6, 4), Color.White * 0.8f);
            spriteBatch.Draw(MagicPixel, new Rectangle(rectangle.X - 2, rectangle.Y - 2, 4, rectangle.Height - 2), Color.White * 0.8f);
            spriteBatch.Draw(MagicPixel, new Rectangle(rectangle.X - 2, rectangle.Y + rectangle.Height - 4, rectangle.Width - 2, 4), Color.White * 0.8f);
            spriteBatch.Draw(MagicPixel, new Rectangle(rectangle.X + rectangle.Width - 4, rectangle.Y - 2, 4, rectangle.Height + 2), Color.White * 0.8f);
        }

        public void DrawCharacterSheet(SpriteBatch spriteBatch, Rectangle rectangle, GenshinCharacter character)
        {
            Texture2D MagicPixel = TextureAssets.MagicPixel.Value;

            // Draw background
            //spriteBatch.Draw(MagicPixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height), GenshinRarityUtils.GetColor(character.Rarity));
            spriteBatch.Draw(MagicPixel, new Rectangle(rectangle.X + 2, rectangle.Y + 2, rectangle.Width - 4, rectangle.Height - 4), GenshinRarityUtils.GetColor(character.Rarity));

            // Character icon
            Texture2D characterTexture = character.TextureFull;
            int width = characterTexture.Width;
            int height = characterTexture.Height;
            spriteBatch.Draw(characterTexture, new Rectangle(rectangle.X + (rectangle.Width - width) / 2 - 4, rectangle.Y + (rectangle.Height - height) / 2 - 6, width + 8, height + 8), Color.Black * 0.3f);
            spriteBatch.Draw(characterTexture, new Rectangle(rectangle.X + (rectangle.Width - width) / 2, rectangle.Y + (rectangle.Height - height) / 2 - 2, characterTexture.Width, characterTexture.Height), Color.White);

            // Character Element
            Texture2D elementTexture = GenshinElementUtils.GetTexture(character.Element);
            int iconWidth = (int)(elementTexture.Width * 0.8);
            int iconHeight = (int)(elementTexture.Height * 0.8);
            spriteBatch.Draw(elementTexture, new Rectangle(rectangle.X + 2, rectangle.Y + 2, iconWidth + 4, iconHeight + 4), Color.Black * 0.5f);
            spriteBatch.Draw(elementTexture, new Rectangle(rectangle.X + 4, rectangle.Y + 4, iconWidth, iconHeight), Color.White);
        }
    }
}