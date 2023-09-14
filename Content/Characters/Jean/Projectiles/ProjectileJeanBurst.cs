using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.ModObjects.Weapons;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Jean.Projectiles
{
    public class ProjectileJeanBurst : GenshinProjectile
    {
        public Texture2D TextureProjectile;
        public Texture2D TextureOutline;
        public Texture2D TextureBird;
        public Texture2D TextureSwirl;
        float alpha = 1f;

        public List<NPC> insideNPCs;

        public override void SetDefaults()
        {
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 660;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Main.projFrames[Projectile.type] = 9;
            PostDrawAdditive = true;
            IgnoreICD = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            TextureProjectile ??= GetTexture();
            Projectile.scale = 0.5f;
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            TextureOutline ??= GetTexture("GenshinMod/Content/Characters/Jean/Projectiles/ProjectileJeanBurst_Outline");
            TextureBird ??= GetTexture("GenshinMod/Content/Characters/Jean/Projectiles/ProjectileJeanBurst_Eagle");
            TextureSwirl ??= GetTexture("GenshinMod/Content/Characters/Jean/Projectiles/ProjectileJeanSkillSwirlEffect");
            insideNPCs = new List<NPC>();
        }

        public override void SafeAI()
        {
            Projectile.friendly = FirstFrame;
            Projectile.rotation = (float)Math.Sin(TimeSpent * 0.01f);
            if (Projectile.scale < 1f)
                Projectile.scale *= 1.075f;
            if (TimeSpent > 600) alpha *= 0.95f;
            else alpha = (float)Math.Sin(TimeSpent * 0.05f) * 0.15f + 0.85f;

            if (OwnerCharacter is CharacterJean jean)
            {
                if (Projectile.timeLeft % 60 == 0 && Main.LocalPlayer.Center.Distance(Projectile.Center) < 130)
                {
                    GenshinCharacter genshinCharacter = Main.LocalPlayer.GetModPlayer<GenshinPlayer>().CharacterCurrent;
                    genshinCharacter.Heal(jean.AbilityBurst.GetScaling3() / 10f);
                    genshinCharacter.ApplyElement(GenshinElement.ANEMO, friendly: true);
                    SpawnDust<JeanDust>(genshinCharacter.Player.Center, Vector2.Zero, 1f, 1f, genshinCharacter.Player.height, 6);
                }

                foreach (NPC npc in Main.npc)
                {
                    if (IsValidTarget(npc))
                    {
                        if (npc.Center.Distance(Projectile.Center) < 130)
                        {
                            if (!insideNPCs.Contains(npc))
                            {
                                int type = ModContent.ProjectileType<JeanProjectileBurstHit>();
                                SpawnProjectile(npc.Center, Vector2.Zero, type, jean.AbilityBurst.GetScaling2(), 0f);
                                insideNPCs.Add(npc);
                            }
                        }
                        else
                        {
                            if (insideNPCs.Contains(npc))
                            {
                                int type = ModContent.ProjectileType<JeanProjectileBurstHit>();
                                SpawnProjectile(npc.Center, Vector2.Zero, type, jean.AbilityBurst.GetScaling2(), 0f);
                                insideNPCs.Remove(npc);
                            }
                        }
                    }
                }
            }

            SpawnDust<JeanDust>(Projectile.Center, Projectile.velocity, 1f, 1f, 120, 1, 10);

            Vector2 dustpoint = new Vector2(125, 0).RotatedByRandom(Math.PI * 2f);
            SpawnDust<JeanDust>(Projectile.Center + dustpoint, Projectile.velocity, 1f, 1f, 1, 1, 3);
        }

        public override void SafePostDrawAdditive(Color lightColor, SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
            Color color = GenshinElementUtils.GetColor(Element) * alpha;
            if (TimeSpent <= 5) color *= TimeSpent * 0.2f;

            spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * 2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.25f, Projectile.rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.25f, Projectile.rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * 0.8f, SpriteEffects.None, 0f);

            spriteBatch.Draw(TextureOutline, drawPosition, null, color * 0.9f, Projectile.rotation * 2.5f, TextureProjectile.Size() * 0.5f, Projectile.scale * 2.05f, SpriteEffects.None, 0f);

            spriteBatch.Draw(TextureSwirl, drawPosition, null, color * 0.35f, Projectile.rotation * 4f + 7f, TextureSwirl.Size() * 0.5f, Projectile.scale * 2.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSwirl, drawPosition, null, color * 0.35f, Projectile.rotation * -2f, TextureSwirl.Size() * 0.5f, Projectile.scale * 2.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSwirl, drawPosition, null, color * 0.3f, Projectile.rotation * 5f - 2f, TextureSwirl.Size() * 0.5f, Projectile.scale * 1.3f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSwirl, drawPosition, null, color * 0.4f, Projectile.rotation * 7f - 1f, TextureSwirl.Size() * 0.5f, Projectile.scale * 3.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSwirl, drawPosition, null, color * 0.3f, Projectile.rotation * -4f, TextureSwirl.Size() * 0.5f, Projectile.scale * 3.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSwirl, drawPosition, null, color * 0.5f, Projectile.rotation * 3f + 0.5f, TextureSwirl.Size() * 0.5f, Projectile.scale * 3.5f, SpriteEffects.None, 0f);

            spriteBatch.Draw(TextureBird, drawPosition, null, Color.White * alpha, 0f, TextureBird.Size() * 0.5f, Projectile.scale * 2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureBird, drawPosition, null, Color.White * 0.5f * alpha, 0f, TextureBird.Size() * 0.5f, Projectile.scale * 2.05f, SpriteEffects.None, 0f);
        }
    }
}
