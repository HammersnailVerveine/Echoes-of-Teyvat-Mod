using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Albedo.Projectiles
{
    public class AlbedoBurstMain : GenshinProjectile
    {
        private static Texture2D TextureSelf;
        private float MultAlpha = 0.1f;
        private float MultScale = 1f;
        private List<int> HitNPC;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Solar Isotoma");
        }

        public override void SetDefaults()
        {
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 180;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            IgnoreICD = true;
            FirstFrameDamage = true;
            AttackWeight = AttackWeight.BLUNT;
        }

        public override void OnSpawn(IEntitySource source)
        {
            TextureSelf ??= GetTexture();
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 direction = Projectile.Center - Owner.Center;
            direction.Normalize();
            direction *= 15f;
            Vector2 position = Owner.Center + direction;
            HitNPC = new List<int>();
        }

        public override void SafeAI()
        {
            Lighting.AddLight(Projectile.Center, 0.4f, 0.4f, 0.2f);

            if (TimeSpent < 15)
            {
                MultAlpha *= 1.4f;
                if (MultAlpha > 1f) MultAlpha = 1f;


                MultScale *= 1.2f;
                if (MultScale > 2f) MultScale = 2f;
            }
            else
            {
                MultScale *= 0.9975f;
                MultAlpha *= 0.95f;
            }

            if (Projectile.timeLeft < 60 && Projectile.timeLeft % 8 == 0)
            {
                if (OwnerCharacter is CharacterAlbedo albedo)
                {
                    if (albedo.skillActive)
                    {
                        foreach (Projectile proj in Main.projectile)
                        {
                            if (proj.active && proj.owner == Owner.whoAmI && proj.ModProjectile is AlbedoProjectileSkillMain)
                            {
                                bool hit = false;
                                foreach (NPC npc in Main.npc)
                                {
                                    if (CanHomeInto(npc) && !HitNPC.Contains(npc.whoAmI) && npc.Center.Distance(proj.Center) < AlbedoProjectileSkillMain.Range)
                                    {
                                        SpawnProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<AlbedoProjectileBlast>(), OwnerCharacter.AbilitySkill.GetScaling2(), 1f, 1f);
                                        HitNPC.Add(npc.whoAmI);
                                        hit = true;
                                        break;
                                    }
                                }

                                if (!hit)
                                {
                                    Vector2 position = proj.Center + new Vector2(0f, Main.rand.Next((int)AlbedoProjectileSkillMain.Range - 150) + 100f).RotatedByRandom(MathHelper.ToRadians(360));
                                    SpawnProjectile(position, Vector2.Zero, ModContent.ProjectileType<AlbedoProjectileBlast>(), OwnerCharacter.AbilitySkill.GetScaling2(), 1f, 1f);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
            Color color = GenshinElementUtils.GetColor(GenshinElement.GEO);
            float scale = Projectile.scale * MultScale;
            Vector2 offsetVector = Projectile.velocity;
            offsetVector.Normalize();
            offsetVector *= 156f;
            drawPosition -= offsetVector * 0.7f;
            spriteBatch.Draw(TextureSelf, drawPosition + offsetVector * 0.5f, null, color * MultAlpha * 0.8f, Projectile.rotation, TextureSelf.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(TextureSelf, drawPosition + offsetVector * 1.25f, null, color * MultAlpha, Projectile.rotation, TextureSelf.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSelf, drawPosition + offsetVector.RotatedBy(MathHelper.ToRadians(-20)), null, color * MultAlpha * 0.9f, Projectile.rotation - ((float)Math.PI / 10f), TextureSelf.Size() * 0.5f, scale * 0.75f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSelf, drawPosition + offsetVector.RotatedBy(MathHelper.ToRadians(20)), null, color * MultAlpha * 0.9f, Projectile.rotation + ((float)Math.PI / 10f), TextureSelf.Size() * 0.5f, scale * 0.75f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSelf, drawPosition + offsetVector.RotatedBy(MathHelper.ToRadians(-40)) * 0.8f, null, color * MultAlpha * 0.8f, Projectile.rotation - ((float)Math.PI / 5f), TextureSelf.Size() * 0.5f, scale * 0.4f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSelf, drawPosition + offsetVector.RotatedBy(MathHelper.ToRadians(40)) * 0.8f, null, color * MultAlpha * 0.8f, Projectile.rotation + ((float)Math.PI / 5f), TextureSelf.Size() * 0.5f, scale * 0.4f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSelf, drawPosition + offsetVector.RotatedBy(MathHelper.ToRadians(-80)) * 0.7f, null, color * MultAlpha * 0.8f, Projectile.rotation - ((float)Math.PI / 5f), TextureSelf.Size() * 0.5f, scale * 0.2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSelf, drawPosition + offsetVector.RotatedBy(MathHelper.ToRadians(80)) * 0.7f, null, color * MultAlpha * 0.8f, Projectile.rotation + ((float)Math.PI / 5f), TextureSelf.Size() * 0.5f, scale * 0.2f, SpriteEffects.None, 0f);

            spriteBatch.Draw(TextureSelf, drawPosition + offsetVector, null, color * MultAlpha * 0.3f, Projectile.rotation, TextureSelf.Size() * 0.5f, scale * 1.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSelf, drawPosition + offsetVector.RotatedBy(MathHelper.ToRadians(-20)) * 0.8f, null, color * MultAlpha * 0.27f, Projectile.rotation - ((float)Math.PI / 10f), TextureSelf.Size() * 0.5f, scale * 1.125f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSelf, drawPosition + offsetVector.RotatedBy(MathHelper.ToRadians(20)) * 0.8f, null, color * MultAlpha * 0.27f, Projectile.rotation + ((float)Math.PI / 10f), TextureSelf.Size() * 0.5f, scale * 1.125f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSelf, drawPosition + offsetVector.RotatedBy(MathHelper.ToRadians(-40)) * 0.64f, null, color * MultAlpha * 0.24f, Projectile.rotation - ((float)Math.PI / 5f), TextureSelf.Size() * 0.5f, scale * 0.6f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSelf, drawPosition + offsetVector.RotatedBy(MathHelper.ToRadians(40)) * 0.64f, null, color * MultAlpha * 0.24f, Projectile.rotation + ((float)Math.PI / 5f), TextureSelf.Size() * 0.5f, scale * 0.6f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSelf, drawPosition + offsetVector.RotatedBy(MathHelper.ToRadians(-80)) * 0.56f, null, color * MultAlpha * 0.24f, Projectile.rotation - ((float)Math.PI / 5f), TextureSelf.Size() * 0.5f, scale * 0.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSelf, drawPosition + offsetVector.RotatedBy(MathHelper.ToRadians(80)) * 0.56f, null, color * MultAlpha * 0.24f, Projectile.rotation + ((float)Math.PI / 5f), TextureSelf.Size() * 0.5f, scale * 0.5f, SpriteEffects.None, 0f);

        }
    }
}
