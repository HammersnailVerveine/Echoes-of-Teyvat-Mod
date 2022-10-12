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
	public class AbilityUIState : GenshinUIState
	{
		public static Texture2D TextureBackgroundSkill;
		public static Texture2D TextureBackgroundBurst;
		public static Texture2D TextureBurstEnergy;

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
			=> layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

		public override void OnInitialize()
		{
			TextureBackgroundSkill ??= ModContent.Request<Texture2D>("GenshinMod/Content/UI/Textures/CharacterSkill", AssetRequestMode.ImmediateLoad).Value;
			TextureBackgroundBurst ??= ModContent.Request<Texture2D>("GenshinMod/Content/UI/Textures/CharacterBurst", AssetRequestMode.ImmediateLoad).Value;
			TextureBurstEnergy ??= ModContent.Request<Texture2D>("GenshinMod/Content/UI/Textures/CharacterBurst_Energy", AssetRequestMode.ImmediateLoad).Value;

			Width.Set(0f, 0f);
			Height.Set(0f, 0f);
			Left.Set(Main.screenWidth - 48f, 0f);
			Top.Set(Main.screenHeight - 64f, 0f);
		}

		public override void OnUIScaleChanged()
		{
			Left.Set(Main.screenWidth - 48f, 0f);
			Top.Set(Main.screenHeight - 64f, 0f);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Recalculate();
			Player player = Main.LocalPlayer;
			GenshinPlayer genshinPlayer = player.GetModPlayer<GenshinPlayer>();
			GenshinCharacter character = genshinPlayer.CharacterCurrent;

			CalculatedStyle dimensions = GetDimensions();

			// ELEMENTAL SKILL

			int skillCooldown = character.AbilitySkill.CooldownCurrent;
			Vector2 skillLocation = new Vector2(dimensions.X - TextureBackgroundBurst.Width - TextureBackgroundSkill.Width - 32, dimensions.Y - TextureBackgroundSkill.Height);
			float skillColorMult = skillCooldown <= 0 ? 1f : 0.35f;

			spriteBatch.Draw(TextureBackgroundSkill, skillLocation, Color.White * skillColorMult * 0.8f);
			spriteBatch.Draw(character.TextureAbilitySkill, skillLocation, Color.White * skillColorMult);

			if (skillCooldown > 0) // Skill Text Cooldown Display
            {
				string text = (Math.DivRem(skillCooldown, 60).Quotient).ToString();
				Vector2 position = skillLocation + new Vector2(TextureBackgroundSkill.Width + 8f, TextureBackgroundSkill.Height) * 0.32f;
				if (text.Length == 1) position.X += 4f;
				if (text == "1") position.X += 4f;

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, position, Color.White, 0f, Vector2.Zero, new Vector2(1.5f, 1.5f));
			}

			// ELEMENTAL BURST

			bool burstReady = character.Energy >= character.AbilityBurst.Energy;
			int burstCooldown = character.AbilityBurst.CooldownCurrent;
			Vector2 burstLocation = new Vector2(dimensions.X - TextureBackgroundBurst.Width, dimensions.Y - TextureBackgroundBurst.Height);
			Vector2 burstBackgroundLocation = burstLocation + new Vector2(6f, 6f);
			Color burstColorBG = GetBurstColor(character);
			float burstColorMult = burstCooldown <= 0 && burstReady ? 1f : 0.35f;

			if (!burstReady)
            { // Display burst energy level
				Rectangle rectangle = new Rectangle(0, TextureBurstEnergy.Height, TextureBurstEnergy.Width, 0);

				float increment = character.AbilityBurst.Energy / (TextureBurstEnergy.Height / 2f);
				float energy = 0;
				burstBackgroundLocation.Y += TextureBurstEnergy.Height;
				while (energy < character.Energy)
                {
					energy += increment;

					rectangle.Height += 2;
					rectangle.Y -= 2;
					burstBackgroundLocation.Y -= 2;
				}


				spriteBatch.Draw(TextureBurstEnergy, burstBackgroundLocation, rectangle, burstColorBG * burstColorMult * 0.3f);
			}

			spriteBatch.Draw(TextureBackgroundBurst, burstLocation, Color.White * burstColorMult * 0.8f);
			if (burstReady) spriteBatch.Draw(TextureBurstEnergy, burstBackgroundLocation, burstColorBG * burstColorMult);
			spriteBatch.Draw(character.TextureAbilityBurst, burstLocation, Color.White * burstColorMult);

			if (burstCooldown > 0) // Bust Text Cooldown Display
			{
				string text = (Math.DivRem(burstCooldown, 60).Quotient).ToString();
				Vector2 position = burstLocation + new Vector2(TextureBackgroundBurst.Width + 12f, TextureBackgroundBurst.Height) * 0.35f;
				if (text.Length == 1) position.X += 4f;
				if (text == "1") position.X += 4f;

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, position, Color.White, 0f, Vector2.Zero, new Vector2(1.75f, 1.75f));
			}
		}

		public Color GetBurstColor(GenshinCharacter character)
        {
			switch (character.Element)
            {
				case CharacterElement.GEO:
					return new Color(255, 167, 45);
				case CharacterElement.ANEMO:
					return new Color(79, 255, 202);
				case CharacterElement.CRYO:
					return new Color(104, 209, 255);
				case CharacterElement.DENDRO:
					return new Color(146, 255, 50);
				case CharacterElement.ELECTRO:
					return new Color(162, 96, 255);
				case CharacterElement.HYDRO:
					return new Color(30, 139, 255);
				default: // PYRO
					return new Color(255, 102, 68);
            }
        }
	}
}