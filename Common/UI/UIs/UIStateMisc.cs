using GenshinMod.Common.GameObjects;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.ModObjects.ModSystems;
using GenshinMod.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace GenshinMod.Common.UI.UIs
{
    public class UIStateMisc : GenshinUIState
    {
        public static Texture2D TextureArrowUp;
        public static Texture2D TextureBorder;
        public static Texture2D KeyTexture;
        public static Texture2D KeyTextureCube;
        public static Texture2D KeyTextureOutline;

        public static bool ShowArrowUp = false;

        public override int InsertionIndex(List<GameInterfaceLayer> layers)
            => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public override void OnInitialize()
        {
            TextureBorder ??= ModContent.Request<Texture2D>("GenshinMod/Common/UI/UIs/Textures/BorderPixel", AssetRequestMode.ImmediateLoad).Value;
            TextureArrowUp ??= ModContent.Request<Texture2D>("GenshinMod/Common/UI/UIs/Textures/ArrowUp", AssetRequestMode.ImmediateLoad).Value;
            KeyTexture ??= ModContent.Request<Texture2D>("GenshinMod/Common/UI/UIs/Textures/AbyssKey", AssetRequestMode.ImmediateLoad).Value;
            KeyTextureOutline ??= ModContent.Request<Texture2D>("GenshinMod/Common/UI/UIs/Textures/AbyssKey_Outline", AssetRequestMode.ImmediateLoad).Value;
            KeyTextureCube ??= ModContent.Request<Texture2D>("GenshinMod/Common/UI/UIs/Textures/AbyssKeyCube", AssetRequestMode.ImmediateLoad).Value;

            Width.Set(0f, 0f);
            Height.Set(0f, 0f);
            Left.Set(Main.screenWidth / 2f, 0f);
            Top.Set(Main.screenHeight / 2f, 0f);
        }

        public override void OnUIScaleChanged()
        {
            Left.Set(Main.screenWidth / 2f, 0f);
            Top.Set(Main.screenHeight / 2f, 0f);
        }

        public override void OnResolutionChanged(int width, int height)
        {
            Left.Set(Main.screenWidth / 2f, 0f);
            Top.Set(Main.screenHeight / 2f, 0f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Recalculate();
            Player player = Main.LocalPlayer;
            GenshinPlayer genshinPlayer = player.GetModPlayer<GenshinPlayer>();
            CalculatedStyle dimensions = GetDimensions();

            if (ShowArrowUp)
            {
                Vector2 arrowPosition = new Vector2(dimensions.X, dimensions.Y - 50 - (genshinPlayer.Timer % 120 < 60 ? 2 : 0));
                spriteBatch.Draw(TextureArrowUp, arrowPosition, null, Color.White, 0f, TextureArrowUp.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            if (genshinPlayer.Challenge != null)
            {
                Vector2 textPosition = new Vector2(dimensions.X - Main.screenWidth / 2f + 20, dimensions.Y - Main.screenHeight / 4f);

                if (genshinPlayer.Challenge.OngoingWave)
                {
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Ongoing Wave", textPosition, Color.White, 0f, Vector2.Zero, new Vector2(1f, 1f));
                    textPosition.Y += 20;
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Remaining enemies : " + genshinPlayer.Challenge.NPCs.Count, textPosition, Color.White, 0f, Vector2.Zero, new Vector2(1f, 1f));
                }
                else ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Next Wave in : " + Math.DivRem(genshinPlayer.Challenge.Waves[0].delay, 60, out _), textPosition, Color.White, 0f, Vector2.Zero, new Vector2(1f, 1f));
            }
        }
    }
}