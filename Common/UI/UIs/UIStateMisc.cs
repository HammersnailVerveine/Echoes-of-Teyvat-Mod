using GenshinMod.Common.GameObjects;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.ModObjects.ModSystems;
using GenshinMod.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace GenshinMod.Common.UI.UIs
{
    public class UIStateMisc : GenshinUIState
    {
        public static Texture2D TextureArrowUp;
        public static Texture2D KeyTexture;
        public static Texture2D KeyTextureCube;
        public static Texture2D KeyTextureOutline;

        public static bool ShowArrowUp = false;

        public override int InsertionIndex(List<GameInterfaceLayer> layers)
            => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public override void OnInitialize()
        {
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
        }
    }
}