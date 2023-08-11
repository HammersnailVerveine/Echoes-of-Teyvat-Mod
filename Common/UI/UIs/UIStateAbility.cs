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
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace GenshinMod.Common.UI.UIs
{
    public class UIStateAbility : GenshinUIState
    {
        public static Texture2D TextureSkillBackground;
        public static Texture2D TextureSkillCharge;
        public static Texture2D TextureSkillChargeFull;
        public static Texture2D TextureBurstBackground;
        public static Texture2D TextureBurstEnergy;

        public override int InsertionIndex(List<GameInterfaceLayer> layers)
            => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public override void OnInitialize()
        {
            TextureSkillBackground ??= ModContent.Request<Texture2D>("GenshinMod/Common/UI/UIs/Textures/CharacterSkill", AssetRequestMode.ImmediateLoad).Value;
            TextureSkillCharge ??= ModContent.Request<Texture2D>("GenshinMod/Common/UI/UIs/Textures/CharacterSkillCharge", AssetRequestMode.ImmediateLoad).Value;
            TextureSkillChargeFull ??= ModContent.Request<Texture2D>("GenshinMod/Common/UI/UIs/Textures/CharacterSkillCharge_Full", AssetRequestMode.ImmediateLoad).Value;
            TextureBurstBackground ??= ModContent.Request<Texture2D>("GenshinMod/Common/UI/UIs/Textures/CharacterBurst", AssetRequestMode.ImmediateLoad).Value;
            TextureBurstEnergy ??= ModContent.Request<Texture2D>("GenshinMod/Common/UI/UIs/Textures/CharacterBurst_Energy", AssetRequestMode.ImmediateLoad).Value;

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

        public override void OnResolutionChanged(int width, int height)
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
            Vector2 skillLocation = new Vector2(dimensions.X - TextureBurstBackground.Width - TextureSkillBackground.Width - 32, dimensions.Y - TextureSkillBackground.Height);
            float skillColorMult = character.AbilitySkill.CanUse() ? skillCooldown > 0 ? 0.75f : 1f : 0.35f;

            spriteBatch.Draw(TextureSkillBackground, skillLocation, Color.White * skillColorMult * 0.8f);
            spriteBatch.Draw(character.TextureAbilitySkill, skillLocation, Color.White * skillColorMult);

            if (skillCooldown > 0) // Skill Text Cooldown Display
            {
                string text = Math.DivRem(skillCooldown, 60).Quotient.ToString();
                Vector2 position = skillLocation + new Vector2(TextureSkillBackground.Width, TextureSkillBackground.Height) * 0.32f;
                if (text.Length == 1) position.X += 5.5f;
                if (text.StartsWith("1")) position.X += 5f;

                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, position, Color.White, 0f, Vector2.Zero, new Vector2(1.5f, 1.5f));
            }

            int skillChargesMax = character.AbilitySkill.ChargesMax;
            if (skillChargesMax > 1) // Skill Charges display
            {
                int skillChargesCurrent = character.AbilitySkill.ChargesCurrent;
                float increment = TextureSkillBackground.Width / (skillChargesMax + 1);
                Vector2 position = skillLocation + new Vector2(increment - TextureSkillChargeFull.Width / 2f, -TextureSkillChargeFull.Height + 2f);
                for (int i = 0; i < skillChargesMax; i++)
                {
                    float bonusHeight = skillChargesMax < 3 || 0 < i && i < skillChargesMax - 1 ? -4f : 0;
                    Vector2 newPosition = position + new Vector2(0f, bonusHeight);
                    spriteBatch.Draw(TextureSkillCharge, newPosition, Color.White * skillColorMult * 0.8f);
                    if (skillChargesCurrent > i) spriteBatch.Draw(TextureSkillChargeFull, newPosition, Color.White * 0.8f);
                    position.X += increment;
                }
            }

            // ELEMENTAL BURST

            bool burstReady = character.Energy >= character.AbilityBurst.Energy;
            int burstCooldown = character.AbilityBurst.CooldownCurrent;
            Vector2 burstLocation = new Vector2(dimensions.X - TextureBurstBackground.Width, dimensions.Y - TextureBurstBackground.Height);
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

            spriteBatch.Draw(TextureBurstBackground, burstLocation, Color.White * burstColorMult * 0.8f);
            if (burstReady) spriteBatch.Draw(TextureBurstEnergy, burstBackgroundLocation, burstColorBG * burstColorMult);
            spriteBatch.Draw(character.TextureAbilityBurst, burstLocation, Color.White * burstColorMult);

            if (burstCooldown > 0) // Bust Text Cooldown Display
            {
                string text = Math.DivRem(burstCooldown, 60).Quotient.ToString();
                Vector2 position = burstLocation + new Vector2(TextureBurstBackground.Width + 5f, TextureBurstBackground.Height) * 0.35f;
                if (text.Length == 1) position.X += 5f;
                if (text.StartsWith("1")) position.X += 5f;

                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, position, Color.White, 0f, Vector2.Zero, new Vector2(1.75f, 1.75f));
            }
        }

        public static Color GetBurstColor(GenshinCharacter character)
        {
            switch (character.Element)
            {
                case GenshinElement.GEO:
                    return new Color(255, 167, 45);
                case GenshinElement.ANEMO:
                    return new Color(79, 255, 202);
                case GenshinElement.CRYO:
                    return new Color(104, 209, 255);
                case GenshinElement.DENDRO:
                    return new Color(146, 255, 50);
                case GenshinElement.ELECTRO:
                    return new Color(162, 96, 255);
                case GenshinElement.HYDRO:
                    return new Color(30, 139, 255);
                default: // PYRO
                    return new Color(255, 102, 68);
            }
        }
    }
}