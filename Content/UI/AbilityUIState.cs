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

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
			=> layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

		public override void OnInitialize()
		{
			TextureBackgroundSkill ??= ModContent.Request<Texture2D>("GenshinMod/Content/UI/Textures/CharacterSkill", AssetRequestMode.ImmediateLoad).Value;
			TextureBackgroundBurst ??= ModContent.Request<Texture2D>("GenshinMod/Content/UI/Textures/CharacterBurst", AssetRequestMode.ImmediateLoad).Value;

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

			CalculatedStyle dimensions = GetDimensions();

			// ELEMENTAL SKILL

			Vector2 skillLocation = new Vector2(dimensions.X - TextureBackgroundBurst.Width - TextureBackgroundSkill.Width - 32, dimensions.Y - TextureBackgroundSkill.Height);
			int skillCooldown = genshinPlayer.CharacterCurrent.AbilitySkill.CooldownCurrent;
			spriteBatch.Draw(TextureBackgroundSkill, skillLocation, Color.White);
			spriteBatch.Draw(genshinPlayer.CharacterCurrent.TextureAbilitySkill, skillLocation, skillCooldown <= 0 ? Color.White : Color.DarkGray * 0.5f);

			if (skillCooldown > 0) // Skill Text Cooldown Display
            {
				string text = (Math.DivRem(skillCooldown, 60).Quotient).ToString();
				Vector2 position = skillLocation + new Vector2(TextureBackgroundSkill.Width + 8f, TextureBackgroundSkill.Height) * 0.32f;
				if (text.Length == 1) position.X += 4f;
				if (text == "1") position.X += 4f;

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, position, Color.White, 0f, Vector2.Zero, new Vector2(1.5f, 1.5f));
			}

			// ELEMENTAL BURST

			Vector2 burstLocation = new Vector2(dimensions.X - TextureBackgroundBurst.Width, dimensions.Y - TextureBackgroundBurst.Height);
			int burstCooldown = genshinPlayer.CharacterCurrent.AbilityBurst.CooldownCurrent;
			spriteBatch.Draw(TextureBackgroundBurst, burstLocation, Color.White);
			spriteBatch.Draw(genshinPlayer.CharacterCurrent.TextureAbilityBurst, burstLocation, burstCooldown <= 0 ? Color.White : Color.DarkGray * 0.5f);

			if (burstCooldown > 0) // Bust Text Cooldown Display
			{
				string text = (Math.DivRem(burstCooldown, 60).Quotient).ToString();
				Vector2 position = burstLocation + new Vector2(TextureBackgroundBurst.Width + 12f, TextureBackgroundBurst.Height) * 0.35f;
				if (text.Length == 1) position.X += 4f;
				if (text == "1") position.X += 4f;

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, position, Color.White, 0f, Vector2.Zero, new Vector2(1.75f, 1.75f));
			}
		}
	}
}