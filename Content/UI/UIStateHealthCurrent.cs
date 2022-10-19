using GenshinMod.Common.GameObjects;
using GenshinMod.Common.ModObjects;
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

namespace GenshinMod.Content.UI
{
    public class UIStateHealthCurrent : GenshinUIState
	{
		public static Texture2D TextureHealthBar;

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
			=> layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

		public override void OnInitialize()
		{
			TextureHealthBar ??= ModContent.Request<Texture2D>("GenshinMod/Content/UI/Textures/HealthBar", AssetRequestMode.ImmediateLoad).Value;

			Width.Set(0f, 0f);
			Height.Set(0f, 0f);
			Left.Set(Main.screenWidth / 2f, 0f);
			Top.Set(Main.screenHeight - 96f, 0f);
		}

		public override void OnUIScaleChanged()
		{
			Left.Set(Main.screenWidth / 2f, 0f);
			Top.Set(Main.screenHeight - 96f, 0f);
		}

		public override void OnResolutionChanged(int width, int height)
		{
			Left.Set(Main.screenWidth / 2f, 0f);
			Top.Set(Main.screenHeight - 96f, 0f);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Recalculate();
			Player player = Main.LocalPlayer;
			GenshinPlayer genshinPlayer = player.GetModPlayer<GenshinPlayer>();
			GenshinCharacter character = genshinPlayer.CharacterCurrent;
			CalculatedStyle dimensions = GetDimensions();

			Vector2 healthPosition = new Vector2(dimensions.X, dimensions.Y);
			Color healthBackgroundColor = new Color(20, 50, 20);
			Color healthColor = new Color(115, 200, 0);
			spriteBatch.Draw(TextureHealthBar, healthPosition, null, healthBackgroundColor * 0.5f, 0f, TextureHealthBar.Size() * 0.5f, 1.25f, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureHealthBar, healthPosition, null, healthBackgroundColor, 0f, TextureHealthBar.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
			Rectangle rectangle = new Rectangle(0, 0, 0, TextureHealthBar.Height);

			float increment = character.EffectiveHealth / (TextureHealthBar.Width * 0.5f);
			float health = 0;
			while (health < character.Health)
			{
				rectangle.Width += 2;
				health += increment;
			}

			spriteBatch.Draw(TextureHealthBar, healthPosition, rectangle, healthColor, 0f, TextureHealthBar.Size() * 0.5f, 1f, SpriteEffects.None, 0f);



			Vector2 staminaPosition = new Vector2(dimensions.X, dimensions.Y - 16f);
			Color staminaBackgroundColor = new Color(50, 50, 20);
			Color staminaColor = new Color(255, 200, 50);
			spriteBatch.Draw(TextureHealthBar, staminaPosition, null, staminaBackgroundColor * 0.5f, 0f, TextureHealthBar.Size() * 0.5f, 0.9375f, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureHealthBar, staminaPosition, null, staminaBackgroundColor, 0f, TextureHealthBar.Size() * 0.5f, 0.75f, SpriteEffects.None, 0f);
			Rectangle staminaRectangle = new Rectangle(0, 0, 0, TextureHealthBar.Height);

			float staminaIncrement = character.GenshinPlayer.StaminaMax / (TextureHealthBar.Width * 0.5f);
			float stamina = 0;
			while (stamina < character.GenshinPlayer.Stamina - staminaIncrement)
			{
				staminaRectangle.Width += 2;
				stamina += staminaIncrement;
			}

			spriteBatch.Draw(TextureHealthBar, staminaPosition, staminaRectangle, staminaColor, 0f, TextureHealthBar.Size() * 0.5f, 0.75f, SpriteEffects.None, 0f);
		}
	}
}