using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.GlobalObjets;
using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Dusts;
using GenshinMod.Content.NPCs.Boss.HypostasisGeo.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.NPCs.Boss.HypostasisGeo
{
	public class HypostasisGeo : GenshinNPC
	{
		// Visuals Fields

		private static Texture2D TextureCore;
		private static Texture2D TextureCoreCenter;
		private static Texture2D TexturePixel;
		private static Texture2D TextureSymbol;
		public static Texture2D TextureCube;
		public static Texture2D TextureGlow;

		public static Color ColorGeo;
		public static Color ColorBrown;

		public float ScaleCore = 0.65f;
		public float ScaleCoreTarget = 0.65f;

		public float SymbolGlow = 0f;
		public float SymbolGlowTarget = 0f;

		public float Glow = 1f;
		public float GlowTarget = 1f;

		public HypostasisGeoCube[] Cubes;

		// Gameplay Fields

		public Vector2 SpawnPosition;

		public int CombatTime = 0;
		public bool Exposed = false;

		private int Timer = 0;
		private int StateCombat = 0;
		private int StateCubes = 0;
		private bool StateChanged = false;

		public List<NPC> Pillars;
		public NPC PillarSelected;
		public int RemainingPillars = 3;

		private Vector2 PositionTarget;

		List<int> OwnedProjectileTypes;

		public bool InCombat => StateCombat != 0;
		public Vector2 SpawnCenter => SpawnPosition + new Vector2(NPC.width / 2f, NPC.height / 2f);

		public override void SafeSetStaticDefaults() {
			DisplayName.SetDefault("Geo Hypostasis");
		}

        public override void SafeSetDefaults()
		{
			NPC.aiStyle = -1;
			NPC.width = 40;
			NPC.height = 40;
			NPC.damage = 200;
			NPC.lifeMax = 750;
			NPC.HitSound = SoundID.NPCHit41;
			NPC.DeathSound = SoundID.NPCDeath43;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;

			NPC.boss = true;

			GenshinGlobalNPC.Element = GenshinElement.GEO;
			GenshinGlobalNPC.ResistanceGeo = 1f;
			GenshinGlobalNPC.ElementSymbolDrawOffset = 32;
		}

        public override void OnSpawn(IEntitySource source)
        {
			TextureCore ??= ModContent.Request<Texture2D>("GenshinMod/Content/NPCs/Boss/HypostasisGeo/HypostasisGeoCore", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			TextureCube ??= ModContent.Request<Texture2D>("GenshinMod/Content/NPCs/Boss/HypostasisGeo/HypostasisGeoCube", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			TextureGlow ??= ModContent.Request<Texture2D>("GenshinMod/Content/NPCs/Boss/HypostasisGeo/HypostasisGeoGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			TexturePixel ??= ModContent.Request<Texture2D>("GenshinMod/Content/NPCs/Boss/HypostasisGeo/HypostasisGeoPixel", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			TextureSymbol ??= ModContent.Request<Texture2D>("GenshinMod/Content/NPCs/Boss/HypostasisGeo/HypostasisGeoSymbol", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			TextureCoreCenter ??= ModContent.Request<Texture2D>("GenshinMod/Content/NPCs/Boss/HypostasisGeo/HypostasisGeoCoreCenter", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			ColorGeo = GenshinElementUtils.GetColor(GenshinElement.GEO);
			ColorBrown = new Color(115, 75, 45);

			SpawnPosition = NPC.position;
			PositionTarget = NPC.position;

			OwnedProjectileTypes = new List<int>();
			OwnedProjectileTypes.Add(ModContent.ProjectileType<HypostasisGeoProjectileShoot>());
			OwnedProjectileTypes.Add(ModContent.ProjectileType<HypostasisGeoProjectileTarget>());
			OwnedProjectileTypes.Add(ModContent.ProjectileType<HypostasisGeoProjectileCube>());

			Pillars = new List<NPC>();
			Cubes = new HypostasisGeoCube[8];
			for (int i = 0; i < 8; i++)
			{
				Cubes[i] = new HypostasisGeoCube();
				HypostasisGeoCube cube = Cubes[i];
				cube.RotationOffSet = Main.rand.NextFloat(MathHelper.TwoPi);
				cube.RotationOffSet2 = MathHelper.ToRadians(90f * (i + 1));
				cube.FrontDraw = i < 4;
				cube.ShadeIntensity = cube.FrontDraw ? 0f : 0.7f;
				cube.ShadeIntensityTarget = cube.ShadeIntensity;
				cube.Position = NPC.Center + new Vector2(0f, 24f).RotatedBy(MathHelper.ToRadians(45f + 90f * i));
				cube.PositionTarget += cube.Position;
				cube.GlowIndex = Main.rand.Next(4);
			}
		}

        public override void SafeAI()
		{
			Timer++;
			if (InCombat)
			{
				TargetPosition = PlayerTarget.Center;

				if (PillarSelected != null && !PillarSelected.active)
				{
					ChangeCombatState(4);
					PillarSelected = null;
				}

				switch (StateCubes)
				{
					case 0: // "In combat" IDLE
						for (int i = 0; i < 8; i++)
						{
							HypostasisGeoCube cube = Cubes[i];
							cube.PositionTarget = NPC.Center + new Vector2(0f, cube.FrontDraw ? 30f : 34f).RotatedBy(MathHelper.ToRadians(45f + 90f * i));
							cube.RotationTarget = 0f;
							cube.ShadeIntensityTarget = cube.FrontDraw ? 0f : 0.5f;
						}
						break;
					case 1: // Spinning
						for (int i = 0; i < 8; i++)
						{
							HypostasisGeoCube cube = Cubes[i];
							cube.RotationTarget += 0.01f + Main.rand.NextFloat(0.0025f * i);

							if (i < 2)
							{
								cube.PositionTarget = NPC.Center + new Vector2(0f, 42f * (i == 0 ? 1f : -1f));
								cube.FrontDraw = true;
							}
							else
							{
								bool top = i % 2 == 0;
								float value = TimeAlive * 0.025f + (MathHelper.TwoPi / 6f) * i;
								float offset = (float)Math.Sin(value);
								float cosValue = (float)Math.Cos(value);
								cube.FrontDraw = cosValue > 0;
								cube.ShadeIntensityTarget = cosValue > 0 ? 0f : -cosValue * 0.5f;
								cube.PositionTarget = NPC.Center + new Vector2(offset * 42f, 18f * (top ? 1f : -1f));
							}
						}
						break;
					case 2: // Fast circular spin then disappear. Requires Timer to be reset
						for (int i = 0; i < 8; i++)
						{
							HypostasisGeoCube cube = Cubes[i];
							float value = Timer * 0.08f + (MathHelper.TwoPi / 8f) * i;
							float distmult = 1f + (Timer < 90 ? Timer * 0.1f : 9f);
							float offset = (float)Math.Sin(value) * 32f * distmult;
							float cosOffset = (float)Math.Cos(value);
							cube.FrontDraw = cosOffset > 0;
							cube.ShadeIntensityTarget = cosOffset > 0 ? 0f : -cosOffset * 0.5f;
							cube.PositionTarget = NPC.Center + new Vector2(offset, cosOffset * 12f);

							if (Timer > 150)
								cube.ScaleTarget *= 0.95f;
						}
						break;
					case 3: // flat circle around the core, then absorbed by it one by one.
						for (int i = 0; i < 8; i++)
						{
							HypostasisGeoCube cube = Cubes[i];
							cube.RotationTarget += 0.01f + Main.rand.NextFloat(0.0025f * i);
							cube.FrontDraw = true;

							if (Timer < 120)
								cube.PositionTarget = NPC.Center + new Vector2(0f, 90f).RotatedBy(MathHelper.TwoPi / 8f * i);

							if (Timer == 120 + i * 10)
                            {
								cube.PositionTarget = NPC.Center;
								cube.ScaleTarget = 0f;
                            }
						}
						break;
					case 4: // flat circle around the core, then starts spinning.
						for (int i = 0; i < 8; i++)
						{
							HypostasisGeoCube cube = Cubes[i];
							cube.RotationTarget += 0.03f + Main.rand.NextFloat(0.0025f * i);
							cube.FrontDraw = true;
							cube.PositionTarget = NPC.Center + new Vector2(0f, 90f).RotatedBy(MathHelper.TwoPi / 8f * i + Timer * ((Timer < 220) ? 0.02f : 0.075f));
						}
						break;
					case 5: // Disappear
						for (int i = 0; i < 8; i++)
						{
							HypostasisGeoCube cube = Cubes[i];
							cube.ScaleTarget = 0f;
							if (Timer > 120)
								cube.PositionTarget = NPC.Center;
						}
						break;
					case 6: // flat circle around the core, then disappear and reappear.
						for (int i = 0; i < 8; i++)
						{
							HypostasisGeoCube cube = Cubes[i];
							cube.RotationTarget += 0.01f + Main.rand.NextFloat(0.0025f * i);
							cube.FrontDraw = true;

							if (Timer < 120)
								cube.PositionTarget = NPC.Center + new Vector2(0f, 90f).RotatedBy(MathHelper.TwoPi / 8f * i);

							if (Timer == 120)
								cube.ScaleTarget = 0f;

							if (Timer == 390)
								cube.ScaleTarget = 1f;
						}
						break;
					default:
						break;
				}

				switch (StateCombat)
				{
					case 1: // Combat beginning
						if (CheckChange())
						{
							ScaleCoreTarget = 0.8f;
							for (int i = 0; i < 8; i++) Cubes[i].GlowMultTarget = 1f;
							ChangeCubeState(0);
						}

						if (Timer == 60)
							ChangeCubeState(1);

						if (Timer == 180)
							ChangeCombatState(2);
						break;
					case 2: // Pillars spawn
						if (CheckChange())
						{
							ChangeCubeState(2);
						}

						if (Timer > 180 && Timer < 300)
							PositionTarget.Y -= 0.35f;

						if (Timer == 300)
						{ // spawn pillars
							Vector2 spawnGroundPosition = FindGround(SpawnCenter);
							int type = ModContent.NPCType<HypostasisGeoPillar>();
							for (int i = -3; i < 4; i += 2)
							{
								Vector2 pillarPosition = spawnGroundPosition;
								pillarPosition.X += i * 224f;
								NPC pillar = Main.npc[NPC.NewNPC(NPC.GetSource_FromAI(), (int)pillarPosition.X, (int)pillarPosition.Y, type)];
								Pillars.Add(pillar);
							}
						}


						if (Timer == 420)
							ChangeCombatState(3);

						if (Timer == 540)
							ChangeCombatState(3);
						break;
					case 3: // Selects random pillar then randomly selects an attack
						if (CheckChange())
							ChangeCubeState(1);

						if (Timer == 120 && Pillars.Count > 0)
						{
							PillarSelected = Pillars[Main.rand.Next(Pillars.Count)];
							PositionTarget = PillarSelected.Center - new Vector2(NPC.width * 0.5f, 192f);
						}
						else if (Pillars.Count == 0)
						{
							ChangeCombatState(2);
						}

						if (Timer == 300)
						{ // randomly selects an attack
							switch (Main.rand.Next(3))
							{
								case 1:
									ChangeCombatState(8);
									break;
								case 2:
									ChangeCombatState(9);
									break;
								default:
									ChangeCombatState(7);
									break;
							}
						}
						break;
					case 4: // Falls to the ground, can be hit
						if (CheckChange()) {
							Vector2 groundPosition = FindGround(NPC.position);
							groundPosition.Y -= 64f;
							PositionTarget = groundPosition;
							ScaleCoreTarget = 1.2f;
							ChangeCubeState(5);
							Exposed = true;

							foreach (Projectile projectile in Main.projectile)
                            {
								if (OwnedProjectileTypes.Contains(projectile.type))
									projectile.Kill();
                            }
						}

						if (Timer == 600)
						{
							ChangeCombatState(3);
						}
						break;
					case 5: // same as 4, shorter recovery
						if (CheckChange())
						{
							Vector2 groundPosition = FindGround(NPC.position);
							groundPosition.Y -= 64f;
							PositionTarget = groundPosition;
							ScaleCoreTarget = 1.2f;
							ChangeCubeState(5);
							Exposed = true;
						}

						if (Timer == 300)
						{
							ChangeCombatState(3);
						}
						break;
					case 6: // Low life remaining : spawns 3 pillars. The player has 15 seconds to destroy them
						if (CheckChange())
						{
							ChangeCubeState(5);

							foreach (NPC pillar in Pillars)
							{
								pillar.ai[1] = 1f; //despawns pillars
								pillar.netUpdate = true;
							}
							Pillars.Clear();
							PillarSelected = null;

							GlowTarget = 0.5f;
							SymbolGlowTarget = 0.25f;

							Vector2 spawnGroundPosition = FindGround(SpawnCenter);
							spawnGroundPosition.Y -= 256f;
							PositionTarget = spawnGroundPosition;
						}

						if (Timer == 120)
						{
							Vector2 spawnGroundPosition = FindGround(SpawnCenter);
							int type = ModContent.NPCType<HypostasisGeoPillar>();
							for (int i = -1; i < RemainingPillars - 1; i ++)
							{
								Vector2 pillarPosition = spawnGroundPosition;
								pillarPosition.X += i * 300f;
								NPC pillar = Main.npc[NPC.NewNPC(NPC.GetSource_FromAI(), (int)pillarPosition.X, (int)pillarPosition.Y, type)];
								Pillars.Add(pillar);
							}
						}

						if (Timer > 120)
                        {
							RemainingPillars = Pillars.Count;
							if (RemainingPillars == 0)
								NPC.life = 0;
						}

						if (Timer == 1200)
						{
							RemainingPillars = Pillars.Count;
							foreach (NPC pillar in Pillars)
							{
								pillar.ai[1] = 1f; //despawns pillars
								pillar.netUpdate = true;
							}
							Pillars.Clear();
							ChangeCombatState(2);

							if (RemainingPillars > 0)
								NPC.life = (int)(NPC.lifeMax / 3 * RemainingPillars);
						}
						break;
					case 7: // Spawns delayed explosion where player will be attack
						if (CheckChange())
                        {
							ChangeCubeState(4);
							SymbolGlowTarget = 0.7f;
						}

						if (Timer == 60)
						{
							int type = OwnedProjectileTypes[1];
							foreach (Player player in Main.player)
                            {
								if (player.active && player.Distance(NPC.Center) < 3200f)
                                {
									Vector2 groundPosition = FindGround(player.Center);
									groundPosition.Y -= 196;
									Projectile.NewProjectile(NPC.GetSource_FromAI(), groundPosition, Vector2.Zero, type, (int)(NPC.damage * 2.5f), 0f, 255, player.whoAmI);
								}
                            }
                        }

						if (Timer == 480)
							ChangeCombatState(3);
						break;
					case 8: // Circle shoot attack
						if (CheckChange())
						{
							ChangeCubeState(3);
							SymbolGlowTarget = 0.7f;
						}

						if (Timer >= 260 && Timer < 440 && Timer % 15 == 0)
                        {
							int type = OwnedProjectileTypes[0];
							Vector2 velocity = TargetPosition - NPC.Center;
							velocity.Normalize();
							velocity *= 12f;
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, type, NPC.damage, 0f);
                        }

						if (Timer == 560)
							ChangeCombatState(3);
						break;
					case 9: // Lingering cubes attack
						if (CheckChange())
						{
							ChangeCubeState(6);
							SymbolGlowTarget = 0.7f;
						}

						if (Timer == 180)
						{
							int type = OwnedProjectileTypes[2];
							foreach (Projectile projectile in Main.projectile)
                            {
								if (projectile.type == type)
									projectile.timeLeft = 45;
                            }

							for (int i = 0; i < 10; i ++)
							{
								Vector2 groundPosition = FindGround(new Vector2(SpawnCenter.X - Main.rand.NextFloat(1800) + 800f, NPC.position.Y));
								groundPosition.Y -= 250f;
								Projectile.NewProjectile(NPC.GetSource_FromAI(), groundPosition, Vector2.Zero, type, NPC.damage, 0f);
							}
                        }

						if (Timer == 560)
							ChangeCombatState(3);
						break;
					default:
						break;
                }
				SpawnDust<HypostasisGeoDust>(0f, 1f, 0, 1, 80);
				SpawnDust<HypostasisGeoDustSmall>(0f, 1f, 16, 1, 25);
			}
			else
			{
				SpawnDust<HypostasisGeoDust>(0f, 1f, 0, 1, 120);
				SpawnDust<HypostasisGeoDustSmall>(0f, 1f, 16, 1, 40);
			}
		}

		/*
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
			if (NPC.life - damage < NPC.lifeMax * 0.1f)
				damage = (int)(damage - NPC.lifeMax * 0.1f) + 1;
		}

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
		{
		}

        public override bool SpecialOnKill()
		{
			if (RemainingPillars > 0)
			{
				NPC.life = (int)(NPC.lifeMax * 0.1f);
				return false;
			}
			return true;
		}
		*/

        public override void HitEffect(int hitDirection, double damage)
		{
			if (!InCombat)
				ChangeCombatState(1);

			if (NPC.life < NPC.lifeMax * 0.1f)
			{
				ChangeCombatState(6);
				NPC.life = (int)(NPC.lifeMax * 0.1f);
			}
		}

        public override bool SafePreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Vector2 drawPosition = Vector2.Transform(NPC.position + new Vector2(NPC.width * 0.5f, NPC.height * 0.5f + NPC.gfxOffY) - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
			float scaleMult = ((float)Math.Sin(TimeAlive * 0.05f)) * 0.05f + 1.1f;
			float rotationCore = ((float)Math.Sin(TimeAlive * 0.025f) * 0.15f);
			float rotationCoreCenter = ((float)Math.Sin(TimeAlive * 0.07f) * 0.33f);

			// Draw Back Cubes
			foreach (HypostasisGeoCube cube in Cubes)
				if (!cube.FrontDraw) cube.DrawCube(TimeAlive, this, spriteBatch);

			// Draw Symbol
			if (SymbolGlow > 0.025f)
            {
				spriteBatch.Draw(TextureSymbol, drawPosition, null, ColorGeo * SymbolGlow, 0f, TextureSymbol.Size() * 0.5f, ScaleCore, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureSymbol, drawPosition, null, ColorGeo * SymbolGlow * 0.3f, 0f, TextureSymbol.Size() * 0.5f, ScaleCore * scaleMult, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureSymbol, drawPosition, null, ColorGeo * SymbolGlow * 0.1f, 0f, TextureSymbol.Size() * 0.5f, ScaleCore * scaleMult * 1.3f, SpriteEffects.None, 0f);

				int nbDots = 50;
				float segment = (MathHelper.TwoPi / nbDots);
				float rotationBonus = 0.005f * TimeAlive;
				for (int i = 0; i < nbDots; i++)
				{
					Vector2 direction = (Vector2.UnitY * 48f).RotatedBy(segment * i + rotationBonus);
					Vector2 position = NPC.position + direction;
					float rotation = direction.ToRotation();
					Vector2 drawPosition2 = Vector2.Transform(position + new Vector2(NPC.width * 0.5f, NPC.height * 0.5f + NPC.gfxOffY) - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
					spriteBatch.Draw(TexturePixel, drawPosition2, null, ColorGeo * SymbolGlow, rotation, TexturePixel.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
					spriteBatch.Draw(TexturePixel, drawPosition2, null, ColorGeo * SymbolGlow * 0.3f, rotation, TexturePixel.Size() * 0.5f, 1f * scaleMult * 1.5f, SpriteEffects.None, 0f);
				}

				nbDots = 65;
				segment = (MathHelper.TwoPi / nbDots);
				for (int i = 0; i < nbDots; i++)
				{
					Vector2 direction = (Vector2.UnitY * 64f).RotatedBy(segment * i - rotationBonus);
					Vector2 position = NPC.position + direction;
					float rotation = direction.ToRotation();
					Vector2 drawPosition2 = Vector2.Transform(position + new Vector2(NPC.width * 0.5f, NPC.height * 0.5f + NPC.gfxOffY) - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
					spriteBatch.Draw(TexturePixel, drawPosition2, null, ColorGeo * SymbolGlow * 0.75f, rotation, TexturePixel.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
					spriteBatch.Draw(TexturePixel, drawPosition2, null, ColorGeo * SymbolGlow * 0.225f, rotation, TexturePixel.Size() * 0.5f, 1f * scaleMult * 1.5f, SpriteEffects.None, 0f);
				}
			}

			// Draw Core
			spriteBatch.Draw(TextureCoreCenter, drawPosition, null, ColorGeo, NPC.rotation + MathHelper.Pi * 0.25f + rotationCoreCenter, TextureCoreCenter.Size() * 0.5f, ScaleCore, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureCore, drawPosition, null, ColorGeo * 0.7f * Glow, NPC.rotation + rotationCore, TextureCore.Size() * 0.5f, ScaleCore, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureCore, drawPosition, null, ColorGeo * 0.2f * Glow, NPC.rotation + rotationCore, TextureCore.Size() * 0.5f, ScaleCore * scaleMult, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureCore, drawPosition, null, ColorGeo * 0.2f * Glow, NPC.rotation + rotationCore, TextureCore.Size() * 0.5f, ScaleCore * scaleMult * 1.1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureCore, drawPosition, null, ColorGeo * 0.03f * Glow, NPC.rotation + rotationCore, TextureCore.Size() * 0.5f, ScaleCore * scaleMult * 1.3f, SpriteEffects.None, 0f);

			// Draw Front Cubes
			foreach (HypostasisGeoCube cube in Cubes)
				if (cube.FrontDraw) cube.DrawCube(TimeAlive, this, spriteBatch);

			return false;
		}

		public override void ResetEffects()
		{
			GenshinGlobalNPC.TimerElementGeo = 600;
			ScaleCore += (ScaleCoreTarget - ScaleCore) * 0.025f;
			SymbolGlow += (SymbolGlowTarget - SymbolGlow) * 0.05f;
			Glow += (GlowTarget - Glow) * 0.05f;
			foreach (HypostasisGeoCube cube in Cubes)
				cube.ResetEffects();

			for (int i = Pillars.Count - 1; i >= 0; i--)
				if (!Pillars[i].active)
					Pillars.RemoveAt(i);

			Vector2 velocity = PositionTarget - NPC.position;
			NPC.position += velocity * 0.025f;

			if (Exposed)
			{
				GenshinGlobalNPC.ResistanceAnemo = 0.1f;
				GenshinGlobalNPC.ResistanceCryo = 0.1f;
				GenshinGlobalNPC.ResistanceDendro = 0.1f;
				GenshinGlobalNPC.ResistanceElectro = 0.1f;
				GenshinGlobalNPC.ResistanceHydro = 0.1f;
				GenshinGlobalNPC.ResistancePyro = 0.1f;
				GenshinGlobalNPC.ResistancePhysical = 0.1f;
			}
			else
			{
				GenshinGlobalNPC.ResistanceAnemo = 1f;
				GenshinGlobalNPC.ResistanceCryo = 1f;
				GenshinGlobalNPC.ResistanceDendro = 1f;
				GenshinGlobalNPC.ResistanceElectro = 1f;
				GenshinGlobalNPC.ResistanceHydro = 1f;
				GenshinGlobalNPC.ResistancePyro = 1f;
				GenshinGlobalNPC.ResistancePhysical = 1f;
			}
		}

		public void ChangeCombatState(int state)
        {
			Timer = 0;
			StateCombat = state;
			StateChanged = true;
			GlowTarget = 1f;
			ScaleCoreTarget = 1f;
			SymbolGlowTarget = 0f;
			Exposed = false;
		}

		public void ChangeCubeState(int state)
		{
			for (int i = 0; i < 8; i++)
			{
				HypostasisGeoCube cube = Cubes[i];
				cube.ScaleTarget = 1f;
				cube.ShadeIntensityTarget = 0f;
			}
			StateCubes = state;
		}

		public bool CheckChange()
        {
			if (!StateChanged) return false;
			else
            {
				StateChanged = false;
				return true;
			}
        }

		public Vector2 FindGround(Vector2 position)
        {
			Vector2 offSetGround = new Vector2(0f, 15f);
			for (int i = 0; i < 50; i++) // checks for ground position under the npc spawnpos
			{
				offSetGround = Collision.TileCollision(position, offSetGround, 2, 2, true);
				position += offSetGround;
				if (offSetGround.Y < 15f)
				{
					break;
				}
			}
			return position;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
    }

	public class HypostasisGeoCube
    {
		public Vector2 Position = Vector2.Zero;
		public Vector2 PositionTarget = Vector2.Zero;
		public float Rotation = 0f;
		public float RotationTarget = 0f;
		public float RotationOffSet = 0f;
		public float RotationOffSet2 = 0f;
		public float Scale = 1f;
		public float ScaleTarget = 1;
		public float GlowMult = 0.5f;
		public float GlowMultTarget = 0.5f;
		public float ShadeIntensity = 0.5f;
		public float ShadeIntensityTarget = 0.5f;
		public int GlowIndex;
		public bool FrontDraw;

		Texture2D TextureCube => HypostasisGeo.TextureCube;
		Texture2D TextureGlow => HypostasisGeo.TextureGlow;
		public Color ColorGeo => HypostasisGeo.ColorGeo;
		public Color ColorBrown => HypostasisGeo.ColorBrown;

		public void ResetEffects()
		{
			Rotation += (RotationTarget - Rotation) * 0.05f;
			GlowMult += (GlowMultTarget - GlowMult) * 0.05f;
			Scale += (ScaleTarget - Scale) * 0.05f;
			ShadeIntensity += (ShadeIntensityTarget - ShadeIntensity) * 0.15f;
			Vector2 velocity = PositionTarget - Position;
			Position += velocity * 0.06f;
			if (Scale < 0.1f && ScaleTarget < 0.1f) Scale = 0f;
		}

		public float RotationCube(float timeAlive) => (float)Math.Sin(timeAlive * 0.02f + RotationOffSet) * 0.1f;
		public Vector2 DrawPositionCube(NPC npc) => Vector2.Transform((Position + new Vector2(0f, npc.gfxOffY)) - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);

		public void DrawCube(float timeAlive, HypostasisGeo npcH, SpriteBatch spriteBatch)
		{
			float rotationCube = npcH.NPC.rotation + Rotation + RotationCube(timeAlive) + RotationOffSet2;
			Vector2 drawPositionCube = DrawPositionCube(npcH.NPC);
			float scaleMultGlow = ((float)Math.Sin(timeAlive * 0.05f)) * 0.05f + 1.1f;
			Rectangle rectangle = HypostasisGeo.TextureGlow.Bounds;
			rectangle.Height /= 4;
			rectangle.Y += rectangle.Height * GlowIndex;
			float Glow = GlowMult - (npcH.CombatTime == 0 ? (float)Math.Sin(timeAlive * 0.02f) * 0.15f + 0.15f : 0f);

			spriteBatch.Draw(TextureCube, drawPositionCube, null, ColorBrown, rotationCube, TextureCube.Size() * 0.5f, Scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureGlow, drawPositionCube, rectangle, ColorGeo * Glow, rotationCube, rectangle.Size() * 0.5f, Scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureGlow, drawPositionCube, rectangle, ColorGeo * 0.2f * Glow, rotationCube, rectangle.Size() * 0.5f, Scale * scaleMultGlow, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureCube, drawPositionCube, null, Color.Black * ShadeIntensity, rotationCube, TextureCube.Size() * 0.5f, Scale, SpriteEffects.None, 0f);
		}
	}
}
