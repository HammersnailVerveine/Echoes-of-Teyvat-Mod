using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.ModObjects.Weapons;
using GenshinMod.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GenshinMod.Content.Projectiles
{
    public class ProjectileBowCharged : GenshinProjectile
    {
        public Texture2D WeaponTexture;
        public Texture2D ArrowTexture;
        public float ArrowOffsetMult = 0f;

        public List<Vector2> OldPosition;
        public List<float> OldRotation;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jean Slash");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 3600;
            Projectile.scale = 1f;
            ProjectileTrail = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            PostDrawAdditive = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            GenshinPlayer ownerPlayer = Owner.GetModPlayer<GenshinPlayer>();
            WeaponTexture = ModContent.Request<Texture2D>(ownerPlayer.CharacterCurrent.Weapon.Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            ArrowTexture = ModContent.Request<Texture2D>(ownerPlayer.CharacterCurrent.Weapon.Texture + "_Arrow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Projectile.width = (int)(WeaponTexture.Width * ownerPlayer.CharacterCurrent.WeaponSize);
            Projectile.height = (int)(WeaponTexture.Height * ownerPlayer.CharacterCurrent.WeaponSize);
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
        }

        public override void SafeAI()
        {
            // position & rotation
            Vector2 direction = Vector2.Zero;
            Projectile.scale = OwnerCharacter.WeaponSize * 0.8f;
            if (IsLocalOwner)
            {
                Vector2 target = Main.MouseWorld;
                direction = target - Owner.Center;
                direction.Normalize();
            }
            else
            {
                // mp sync : todo
            }
            Projectile.position = Owner.Center + (direction * TileLength * 3.5f * Projectile.scale) - Projectile.Size * 0.5f;
            Projectile.rotation = direction.ToRotation();

            // Afterimages
            if (TimeSpent % 2 == 0)
            {
                OldPosition.Add(Projectile.Center);
                OldRotation.Add(Projectile.rotation);
                if (OldPosition.Count > 5)
                {
                    OldPosition.RemoveAt(0);
                    OldRotation.RemoveAt(0);
                }
            }

            if (OwnerCharacter.AbilityCharged.HoldTime >= 30)
            {
                ArrowOffsetMult = (float)(OwnerCharacter.AbilityCharged.HoldTime - 30) / (float)(OwnerCharacter.AbilityCharged.HoldTimeFull - 90);
                if (ArrowOffsetMult > 1f) ArrowOffsetMult = 1f;
            }

            Owner.direction = Projectile.Center.X > Owner.Center.X ? 1 : -1;
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            // Draw the Bow
            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
            SpriteEffects effect = (Projectile.ai[1] < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color elementColor = GenshinElementUtils.GetColor(OwnerCharacter.Element);
            if (OwnerCharacter.AbilityCharged.HoldTime < OwnerCharacter.AbilityCharged.HoldTimeFull) elementColor *= (float)(OwnerCharacter.AbilityCharged.HoldTime - (OwnerCharacter.AbilityCharged.HoldTimeFull - 15)) / (OwnerCharacter.AbilityCharged.HoldTimeFull - (OwnerCharacter.AbilityCharged.HoldTimeFull - 15));
            float scaleMult = (((float)Math.Sin(TimeSpent * 0.05f)) * 0.115f + 1.115f);
            float rotation = Projectile.rotation + (effect == SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));

            if (OwnerCharacter.AbilityCharged.HoldTime >= OwnerCharacter.AbilityCharged.HoldTimeFull - 15)
            {
                spriteBatch.Draw(WeaponTexture, drawPosition, null, elementColor * 0.2f, rotation, WeaponTexture.Size() * 0.5f, Projectile.scale * scaleMult, effect, 0f);
                spriteBatch.Draw(WeaponTexture, drawPosition, null, elementColor * 0.15f, rotation, WeaponTexture.Size() * 0.5f, Projectile.scale * scaleMult * 1.05f, effect, 0f);
            }
            spriteBatch.Draw(WeaponTexture, drawPosition, null, lightColor * 1.5f, rotation, WeaponTexture.Size() * 0.5f, Projectile.scale, effect, 0f);

            // Draw the Arrow

            Vector2 toOwner = Owner.Center - Projectile.Center;
            toOwner.Normalize();

            Vector2 direction = toOwner;
            Vector2 arrowDrawPosition = drawPosition;
            arrowDrawPosition -= direction * (ArrowTexture.Width * 0.25f - 1f);
            direction *= ArrowOffsetMult * (ArrowTexture.Width * 0.5f + 2f);
            arrowDrawPosition += direction;
            spriteBatch.Draw(ArrowTexture, arrowDrawPosition, null, lightColor * 1.5f, rotation, ArrowTexture.Size() * 0.5f, Projectile.scale, effect, 0f);

            if (OwnerCharacter.AbilityCharged.HoldTime >= OwnerCharacter.AbilityCharged.HoldTimeFull - 15)
            {
                spriteBatch.Draw(ArrowTexture, arrowDrawPosition, null, elementColor * 0.4f, rotation, ArrowTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
                spriteBatch.Draw(ArrowTexture, arrowDrawPosition, null, elementColor * 0.35f, rotation, ArrowTexture.Size() * 0.5f, Projectile.scale * scaleMult, effect, 0f);
            }

            // Draw the string

            if (OwnerCharacter.Weapon is WeaponBow bow)
            {
                Texture2D PixelTexture = GenshinSystemUI.PixelTexture;

                Vector2 texturepos = - new Vector2(WeaponTexture.Width, WeaponTexture.Height) * 0.5f + bow.StringOffSet;
                texturepos = texturepos.RotatedBy(rotation);
                Vector2 stringDirection = drawPosition - toOwner * (ArrowTexture.Width * 0.5f - 2f) + texturepos - arrowDrawPosition;
                stringDirection = stringDirection.RotatedBy(MathHelper.ToRadians(180f));
                Vector2 directionNormalized = stringDirection;
                directionNormalized.Normalize();
                directionNormalized *= 1.5f;
                Color stringColor = bow.StringColor.MultiplyRGB(lightColor);
                while (stringDirection.Length() > 1)
                {
                    spriteBatch.Draw(PixelTexture, drawPosition + texturepos + stringDirection, null, stringColor, rotation, PixelTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
                    spriteBatch.Draw(PixelTexture, drawPosition - Vector2.Reflect(texturepos + stringDirection, toOwner) , null, stringColor, rotation, PixelTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
                    stringDirection -= directionNormalized;
                }

                if (OwnerCharacter.AbilityCharged.HoldTime >= OwnerCharacter.AbilityCharged.HoldTimeFull - 15)
                {
                    stringDirection = drawPosition - toOwner * (ArrowTexture.Width * 0.5f - 2f) + texturepos - arrowDrawPosition;
                    stringDirection = stringDirection.RotatedBy(MathHelper.ToRadians(180f));
                    while (stringDirection.Length() > 1)
                    {
                        spriteBatch.Draw(PixelTexture, drawPosition + texturepos + stringDirection, null, elementColor * 0.5f, rotation, PixelTexture.Size() * 0.5f, Projectile.scale * 1.25f * scaleMult, effect, 0f);
                        spriteBatch.Draw(PixelTexture, drawPosition - Vector2.Reflect(texturepos + stringDirection, toOwner), null, elementColor * 0.5f, rotation, PixelTexture.Size() * 0.5f, Projectile.scale * 1.25f * scaleMult, effect, 0f);
                        stringDirection -= directionNormalized;
                    }
                }
            }
        }

        public override void SafePostDrawAdditive(Color lightColor, SpriteBatch spriteBatch)
        {
            SpriteEffects effect = (Projectile.ai[1] < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation + (effect == SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));
            float colormult = 1f;
            if (OwnerCharacter.AbilityCharged.HoldTime < OwnerCharacter.AbilityCharged.HoldTimeFull) colormult *= (float)(OwnerCharacter.AbilityCharged.HoldTime - (OwnerCharacter.AbilityCharged.HoldTimeFull - 15)) / (OwnerCharacter.AbilityCharged.HoldTimeFull - (OwnerCharacter.AbilityCharged.HoldTimeFull - 15));

            for (int i = 0; i < OldPosition.Count; i++)
            {
                Vector2 drawPosition2 = Vector2.Transform(OldPosition[i] - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                float rotation2 = OldRotation[i] + (effect == SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));
                GenshinElement element = OwnerCharacter.AbilityCharged.HoldTime >= OwnerCharacter.AbilityCharged.HoldTimeFull - 15 ? OwnerCharacter.Element : GenshinElement.NONE;
                if (element == GenshinElement.NONE)
                    spriteBatch.Draw(WeaponTexture, drawPosition2, null, lightColor * 0.075f * i, rotation2, WeaponTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
                else
                spriteBatch.Draw(WeaponTexture, drawPosition2, null, GenshinElementUtils.GetColor(element) * 0.125f * i * colormult, rotation2, WeaponTexture.Size() * 0.5f, Projectile.scale * 1.15f, effect, 0f);
            }
        }
    }
}
