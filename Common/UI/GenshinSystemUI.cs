using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace GenshinMod.Common.UI
{
    [Autoload(Side = ModSide.Client)]
    public class GenshinSystemUI : ModSystem
    { // Taken from Orchid Mod
        private static float uiScale = -1f;
        private static bool ignoreHotbarScroll = false;

        private static readonly List<GenshinUIState> uiStates = new();
        private static readonly List<UserInterface> userInterfaces = new();
		
		public static Texture2D PixelTexture;

        public static readonly RasterizerState OverflowHiddenRasterizerState = new()
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };

        public static void RequestIgnoreHotbarScroll()
            => ignoreHotbarScroll = true;

        public static T GetUIState<T>() where T : GenshinUIState
            => uiStates.FirstOrDefault(i => i is T) as T;

        private static void OnResolutionChanged(Vector2 screenSize)
        {
            foreach (var uiState in uiStates)
            {
                uiState.OnResolutionChanged((int)screenSize.X, (int)screenSize.Y);
            }
        }

        private static void ModifyScrollHotbar(Terraria.On_Player.orig_ScrollHotbar orig, Player player, int offset)
        {
            if (ignoreHotbarScroll) return;

            orig(player, offset);
        }

        private static void ResetVariables(GameTime _)
        {
            ignoreHotbarScroll = false;
        }

        // ...

        public override void Load()
        {
            foreach (Type type in Mod.Code.GetTypes())
            {
                if (!type.IsSubclassOf(typeof(GenshinUIState))) continue;

                var uiState = (GenshinUIState)Activator.CreateInstance(type, null);
                uiStates.Add(uiState);
            }

            uiStates.Sort((x, y) => x.Priority.CompareTo(y.Priority));

            foreach (var uiState in uiStates)
            {
                uiState.Mod = Mod;
                uiState.Activate();

                var userInterface = new UserInterface();
                userInterface.SetState(uiState);
                userInterfaces.Add(userInterface);
            }

            Terraria.On_Player.ScrollHotbar += ModifyScrollHotbar;
            Main.OnResolutionChanged += OnResolutionChanged;
            Main.OnPreDraw += ResetVariables;

            PixelTexture = ModContent.Request<Texture2D>("GenshinMod/Common/UI/UIs/Textures/Pixel", AssetRequestMode.ImmediateLoad).Value;
        }

        public override void Unload()
        {
            Main.OnPreDraw -= ResetVariables;
            Main.OnResolutionChanged -= OnResolutionChanged;
            Terraria.On_Player.ScrollHotbar -= ModifyScrollHotbar;

            foreach (var uiState in uiStates)
            {
                uiState.Deactivate();
                uiState.Unload();
            }

            uiStates.Clear();
            userInterfaces.Clear();

            PixelTexture = null;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            for (int i = 0; i < userInterfaces.Count; i++)
            {
                var uiState = uiStates[i];
                var userInterface = userInterfaces[i];

                var index = uiState.InsertionIndex(layers);
                if (index < 0) continue;

                layers.Insert(index, new LegacyGameInterfaceLayer(
                    name: $"{Mod.Name}: {uiState.Name}",
                    drawMethod: () =>
                    {
                        if (uiState.Visible)
                        {
                            userInterface.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
                        }
                        return true;
                    },
                    scaleType: uiState.ScaleType)
                );
            }

            layers.RemoveAll(layer => layer.Name.Equals("Vanilla: Hotbar"));
            layers.RemoveAll(layer => layer.Name.Equals("Vanilla: Inventory"));
            layers.RemoveAll(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            layers.RemoveAll(layer => layer.Name.Equals("Vanilla: Map / Minimap"));
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (uiScale != Main.UIScale)
            {
                uiScale = Main.UIScale;

                foreach (var uiState in uiStates)
                {
                    uiState.OnUIScaleChanged();
                }
            }

            if (Main.mapFullscreen) return;

            foreach (var userInterface in userInterfaces)
            {
                userInterface.Update(gameTime);
            }
        }
    }
}