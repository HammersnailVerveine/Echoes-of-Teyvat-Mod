using GenshinMod.Common.ModObjects.Weapons;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using GenshinMod.Common.ModObjects;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using System;
using GenshinMod.Content.Characters.Noelle.Projectiles;

namespace GenshinMod.Content.Characters.Noelle.Shields
{
	public class ShieldNoelle : GenshinShield
	{
		private static Texture2D TextureShieldOut;
		private static Texture2D TextureShieldIn;
		private Color GlowColor;
		private int damage;
		private GenshinCharacter Noelle;

		public override void OnInitialize(ref int health, ref int duration, ref GenshinElement element, int value)
        {
			TextureShieldOut ??= ModContent.Request<Texture2D>("GenshinMod/Content/Characters/Noelle/Shields/ShieldNoelle_Out", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			TextureShieldIn ??= ModContent.Request<Texture2D>("GenshinMod/Content/Characters/Noelle/Shields/ShieldNoelle_In", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			GlowColor = GenshinElementUtils.GetColor(GenshinElement.GEO);
			damage = GenshinPlayer.CharacterCurrent.AbilitySkill.GetScaling2();
			Noelle = GenshinPlayer.CharacterCurrent;
		}

        public override void Update()
        {
			Main.NewText(Health);
        }

        public override void OnKill(bool killByDamage)
        {
			Noelle.AbilitySkill.SpawnProjectileSpecific(Noelle.AbilitySkill.GetSource(), Player.Center, Vector2.Zero, ModContent.ProjectileType<ProjectileNoelleShield>(), damage, 5f, Player.whoAmI, Noelle.Element, Noelle.AbilitySkill.AbilityType);
		}

        public override void Draw(SpriteBatch spriteBatch, Color lightColor, GenshinPlayer genshinPlayer)
		{
			Player player = genshinPlayer.Player;
			Vector2 drawPosition = (player.position + new Vector2(player.width * 0.5f, player.height * 0.5f + player.gfxOffY)).Floor();
			drawPosition = Vector2.Transform(drawPosition - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);

			float lightFactor = (float)Math.Sin(genshinPlayer.Timer * 0.05f) * 0.2f + 0.9f;
			float scaleMult = ((float)Math.Sin(Duration * 0.02f)) * 0.025f + 1.04f;
			float scaleMult2 = TimeSpent < 9 ? TimeSpent / 9f : 1f;
			spriteBatch.Draw(TextureShieldIn, drawPosition, null, GlowColor * 0.45f * lightFactor * (2f - scaleMult2), 0f, TextureShieldIn.Size() * 0.5f, 1f * scaleMult2, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureShieldOut, drawPosition, null, GlowColor * 0.2f * lightFactor * (2f - scaleMult2), 0f, TextureShieldIn.Size() * 0.5f, 1f * scaleMult * scaleMult2, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureShieldOut, drawPosition, null, GlowColor * 0.55f * lightFactor * (2f - scaleMult2), 0f, TextureShieldIn.Size() * 0.5f, 1f * scaleMult2, SpriteEffects.None, 0f);
		}
	}
}

