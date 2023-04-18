using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.GlobalObjets;
using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.NPCs.Boss.HypostasisGeo
{
	public class HypostasisGeoPillar : GenshinNPC
	{
		// Visuals Fields

		private static Texture2D TextureCore;
		private static Texture2D TextureOut;
		private static Texture2D TextureGlow;

		int Frame = 0;
		int Frame2 = 0;

		public Color ColorGeo => HypostasisGeo.ColorGeo;
		public Color ColorBrown => HypostasisGeo.ColorBrown;

		public override void SafeSetStaticDefaults() {
			DisplayName.SetDefault("Geo Hypostasis Pillar");
		}

        public override void SafeSetDefaults()
		{
			NPC.aiStyle = -1;
			NPC.width = 70;
			NPC.height = 106;
			NPC.damage = 0;
			NPC.lifeMax = 1250;
			NPC.HitSound = SoundID.NPCHit41;
			NPC.DeathSound = SoundID.NPCDeath43;
			NPC.knockBackResist = 0f;

			GenshinGlobalNPC.Element = GenshinElement.GEO;
			GenshinGlobalNPC.ElementSymbolDrawOffset = 32;
			GenshinGlobalNPC.BluntTarget = true;
			GenshinGlobalNPC.GiveEnergyParticlesLife = false;
		}

        public override void OnSpawn(IEntitySource source)
        {
			TextureCore ??= ModContent.Request<Texture2D>("GenshinMod/Content/NPCs/Boss/HypostasisGeo/HypostasisGeoPillar_In", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			TextureOut ??= ModContent.Request<Texture2D>("GenshinMod/Content/NPCs/Boss/HypostasisGeo/HypostasisGeoPillar_Out", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			TextureGlow ??= ModContent.Request<Texture2D>("GenshinMod/Content/NPCs/Boss/HypostasisGeo/HypostasisGeoPillar_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

			Frame = Main.rand.Next(4);
			Frame2 = (Frame + Main.rand.Next(3) + 1) % 4;
		}

        public override void SafeAI()
		{
			SpawnDust<HypostasisGeoDust>(0f, 1f, 0, 1, 200);
			SpawnDust<HypostasisGeoDustSmall>(0f, 1f, 16, 1, 70);

			NPC.timeLeft = 60;

			if (NPC.ai[1] > 0f)
            {
				NPC.ai[1]++;
				if (TimeAlive > 60) TimeAlive = 60;
				TimeAlive -= 2;
				if (TimeAlive <= 0) NPC.active = false;
			}
		}

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
			if (NPC.ai[1] > 0f) return false;
            return base.CanBeHitByProjectile(projectile);
        }


        public override bool? CanBeHitByItem(Player player, Item item)
        {
			if (NPC.ai[1] > 0f) return false;
            return base.CanBeHitByItem(player, item);
		}

        public override bool SafePreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Vector2 drawPosition = Vector2.Transform((NPC.position + new Vector2(NPC.width * 0.5f, NPC.height * 0.5f + NPC.gfxOffY)) - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);

			float GlowMult = 0.3f - (float)Math.Sin(TimeAlive * 0.02f) * 0.15f + 0.15f;
			float scaleMultGlow = ((float)Math.Sin(TimeAlive * 0.01f)) * 0.01f + 1.01f;
			float scaleMult = ((float)Math.Sin(TimeAlive * 0.05f)) * 0.05f + 1.1f;

			float ColorMult1 = TimeAlive > 30 ? 1f : TimeAlive / 30f; // Core fadein on spawn
			float ColorMult2 = TimeAlive < 20 ? 0f : TimeAlive > 60 ? 1f : (TimeAlive - 20) / 40f; //walls fadein

			Rectangle rectangle = TextureGlow.Bounds;
			rectangle.Height /= 4;
			rectangle.Y += rectangle.Height * Frame;

			Rectangle rectangle2 = TextureGlow.Bounds;
			rectangle2.Height /= 4;
			rectangle2.Y += rectangle2.Height * Frame2;

			spriteBatch.Draw(TextureOut, drawPosition, rectangle2, ColorBrown * ColorMult2, NPC.rotation, rectangle2.Size() * 0.5f, 1f, SpriteEffects.None, 0f);

			spriteBatch.Draw(TextureCore, drawPosition, null, ColorGeo * 0.7f * ColorMult1, NPC.rotation, TextureCore.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureCore, drawPosition, null, ColorGeo * 0.2f * ColorMult1, NPC.rotation, TextureCore.Size() * 0.5f, 1f * scaleMult, SpriteEffects.None, 0f);

			spriteBatch.Draw(TextureOut, drawPosition, rectangle, ColorBrown * ColorMult2, NPC.rotation, rectangle.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureGlow, drawPosition, rectangle, ColorGeo * GlowMult * ColorMult2, NPC.rotation, rectangle.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureGlow, drawPosition, rectangle, ColorGeo * 0.5f * GlowMult * ColorMult2, NPC.rotation, rectangle.Size() * 0.5f, 1f * scaleMultGlow, SpriteEffects.None, 0f);

			return false;
		}

		public override void ResetEffects()
		{
		}
		
		public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
    }
}
