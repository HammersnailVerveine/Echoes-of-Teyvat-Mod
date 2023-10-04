using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Noelle.Projectiles
{
    public class ProjectileNoelleNormal : ProjectileClaymoreNormal
    {
        public static Texture2D TextureGlow;

        public override void OnFirstFrame()
        {
            GenshinPlayer ownerPlayer = Owner.GetModPlayer<GenshinPlayer>();
            Weapon = ownerPlayer.CharacterCurrent.Weapon;
            WeaponTexture = ModContent.Request<Texture2D>(Weapon.Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Projectile.scale = ownerPlayer.CharacterCurrent.WeaponSize;
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
            HitNPC = new List<int>();
            LoadTextures();

            if (IsLocalOwner)
            {
                Projectile.width = (int)(WeaponTexture.Width * 1.4f * ownerPlayer.CharacterCurrent.WeaponSize);
                Projectile.height = (int)(WeaponTexture.Height * 1.4f * ownerPlayer.CharacterCurrent.WeaponSize);
            }
        }

        public static void LoadTextures()
        {
            TextureGlow ??= ModContent.Request<Texture2D>("GenshinMod/Content/Characters/Noelle/Projectiles/ProjectileNoelleNormalGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SafeAI()
        {
            Vector2 position = Owner.Center + (Vector2.UnitY * TileLength * 3.625f * Projectile.scale).RotatedBy(MathHelper.ToRadians(Projectile.ai[0])) - Projectile.Size * 0.5f;
            Projectile.position = position;

            Vector2 direction = Projectile.Center - Owner.Center;
            direction.Normalize();
            Projectile.rotation = direction.ToRotation() + MathHelper.ToRadians(45f);

            Projectile.ai[0] += Projectile.ai[1] * acceleration;
            if (TimeSpent > 17) acceleration *= 0.775f;
            if (TimeSpent < 4) acceleration *= 2.3f;

            int count = 8;
            if (OwnerCharacter is CharacterNoelle noelle)
            { // Burst visuals
                if (noelle.BurstTimer > 0)
                    count = 16;
            }

            // Afterimages
            if (TimeSpent < 30)
            {
                OldPosition.Add(Projectile.Center);
                OldRotation.Add(Projectile.rotation);
                if (OldPosition.Count > count)
                {
                    OldPosition.RemoveAt(0);
                    OldRotation.RemoveAt(0);
                }
            }
            else if (OldPosition.Count > 1 && TimeSpent % 2 == 0)
            {
                OldPosition.RemoveAt(0);
                OldRotation.RemoveAt(0);
            }

            OwnerGenshinPlayer.CompositeArmOffset = direction * 8f;
            OwnerGenshinPlayer.CompositeArmAngle = direction.ToRotation();
        }

        public override void SafeOnHitNPC(NPC target)
        {
            HitNPC.Add(target.whoAmI);
        }


        public override bool? CanHitNPC(NPC target)
        {
            if (HitNPC.Contains(target.whoAmI)) return false;
            return base.CanHitNPC(target);
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            Vector2 drawPosition;
            SpriteEffects effect;
            float rotation;

            if (OwnerCharacter is CharacterNoelle noelle)
            { // Burst visuals
                if (noelle.BurstTimer > 0)
                {
                    drawPosition = Vector2.Transform(Owner.Center + GetOwnerArmOffset() + (Vector2.UnitY * TileLength * 4.5f).RotatedBy(MathHelper.ToRadians(Projectile.ai[0])) + new Vector2(Projectile.width, Projectile.height) * 0.5f - Projectile.Size * 0.5f - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                    effect = (Projectile.ai[1] < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    rotation = Projectile.rotation + (effect == SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));
                    spriteBatch.Draw(WeaponTexture, drawPosition, null, lightColor * 1.5f, rotation, WeaponTexture.Size() * 0.5f, 1f, effect, 0f);
                    return;
                }
            }

            // Other visuals
            drawPosition = Vector2.Transform(Projectile.Center + GetOwnerArmOffset() - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
            effect = (Projectile.ai[1] < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            rotation = Projectile.rotation + (effect == SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));
            spriteBatch.Draw(WeaponTexture, drawPosition, null, lightColor * 1.5f, rotation, WeaponTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
        }

        public override void SafePostDrawAdditive(Color lightColor, SpriteBatch spriteBatch)
        {
            SpriteEffects effect = (Projectile.ai[1] < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation + (effect == SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));

            if (OwnerCharacter is CharacterNoelle noelle)
            { // Burst infusion visuals
                if (noelle.BurstTimer > 0)
                {
                    if (Element != GenshinElement.NONE)
                    {
                        Vector2 drawPosition = Vector2.Transform(Projectile.Center + GetOwnerArmOffset() - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                        spriteBatch.Draw(WeaponTexture, drawPosition, null, GenshinElementUtils.GetColor(Element) * 0.75f, rotation, WeaponTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
                        spriteBatch.Draw(WeaponTexture, drawPosition, null, GenshinElementUtils.GetColor(Element) * 0.25f, rotation, WeaponTexture.Size() * 0.5f, Projectile.scale * 1.2f, effect, 0f);
                    }

                    for (int i = 0; i < OldPosition.Count; i++)
                    {
                        Vector2 drawPosition2 = Vector2.Transform(OldPosition[i] + GetOwnerArmOffset() - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                        float rotation2 = OldRotation[i] + (effect == SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));
                        if (Element == GenshinElement.NONE)
                            spriteBatch.Draw(WeaponTexture, drawPosition2, null, lightColor * 0.075f * i, rotation2, WeaponTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
                        else
                        {
                            spriteBatch.Draw(TextureGlow, drawPosition2, null, GenshinElementUtils.GetColor(Element) * 0.06f * i, rotation2, TextureGlow.Size() * 0.5f, Projectile.scale * 1.1f, effect, 0f);
                            spriteBatch.Draw(TextureGlow, drawPosition2, null, GenshinElementUtils.GetColor(Element) * 0.015f * i, rotation2, TextureGlow.Size() * 0.5f, Projectile.scale * 1.3f, effect, 0f);
                            spriteBatch.Draw(WeaponTexture, drawPosition2, null, GenshinElementUtils.GetColor(Element) * 0.025f * i, rotation, WeaponTexture.Size() * 0.5f, Projectile.scale * 1.15f, effect, 0f);
                        }
                    }
                    return;
                }
            }
            // Other infusion visuals
            if (Element != GenshinElement.NONE)
            {
                Vector2 drawPosition = Vector2.Transform(Projectile.Center + GetOwnerArmOffset() - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(WeaponTexture, drawPosition, null, GenshinElementUtils.GetColor(Element) * 0.75f, rotation, WeaponTexture.Size() * 0.5f, Projectile.scale * 1.15f, effect, 0f);
            }

            for (int i = 0; i < OldPosition.Count; i++)
            {
                Vector2 drawPosition2 = Vector2.Transform(OldPosition[i] + GetOwnerArmOffset() - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                float rotation2 = OldRotation[i] + (effect == SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));
                if (Element == GenshinElement.NONE)
                    spriteBatch.Draw(WeaponTexture, drawPosition2, null, lightColor * 0.075f * i, rotation2, WeaponTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
                else
                    spriteBatch.Draw(WeaponTexture, drawPosition2, null, GenshinElementUtils.GetColor(Element) * 0.125f * i, rotation2, WeaponTexture.Size() * 0.5f, Projectile.scale * 1.15f, effect, 0f);
            }
        }

        public override void SafeSecondPostDraw(Color lightColor, SpriteBatch spriteBatch) => OwnerGenshinPlayer.DrawCompositeArm(spriteBatch);
    }
}
