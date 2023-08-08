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
    public class ProjectileBowNormal : GenshinProjectile
    {
        public Texture2D WeaponTexture;
        public Texture2D ArrowTexture;
        public float ArrowOffsetMult = 1f;

        public List<Vector2> OldPosition;
        public List<float> OldRotation;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 30;
            Projectile.scale = 1f;
            ProjectileTrail = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            PostDrawAdditive = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

        public override void OnSpawn(IEntitySource source)
        {
            GenshinPlayer ownerPlayer = Owner.GetModPlayer<GenshinPlayer>();
            WeaponTexture = ModContent.Request<Texture2D>(ownerPlayer.CharacterCurrent.Weapon.Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            ArrowTexture = ModContent.Request<Texture2D>(ownerPlayer.CharacterCurrent.Weapon.Texture + "_Arrow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Projectile.scale = ownerPlayer.CharacterCurrent.WeaponSize * 0.8f;
            Projectile.width = (int)(WeaponTexture.Width * Projectile.scale);
            Projectile.height = (int)(WeaponTexture.Height * Projectile.scale);
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void SafeAI()
        {
            // position & rotation

            Vector2 velNormalized = Projectile.velocity;
            velNormalized.Normalize();
            Projectile.position = Owner.Center.Floor() + (velNormalized * TileLength * 1.225f * Projectile.scale) - Projectile.Size * 0.5f;

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

            ArrowOffsetMult *= 0.5f;
            Owner.direction = Projectile.Center.X > Owner.Center.X ? 1 : -1;
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            // Draw background composite arm

            Vector2 toOwner = Owner.Center - Projectile.Center;
            toOwner.Normalize();
            if (Math.Abs(toOwner.X) > toOwner.Y) OwnerGenshinPlayer.DrawCompositeArm(spriteBatch, false, true, -toOwner.X * 8f, -toOwner.Y * 8f, toOwner.ToRotation() + MathHelper.Pi);

            // Draw the Bow
            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
            spriteBatch.Draw(WeaponTexture, drawPosition, null, lightColor * 1.5f, Projectile.rotation, WeaponTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

            // Draw the string
            Vector2 direction = toOwner;
            Vector2 arrowDrawPosition = drawPosition;
            arrowDrawPosition -= direction * (ArrowTexture.Width * 0.25f + 3f);
            direction *= ArrowOffsetMult * (ArrowTexture.Width * 0.5f + 2f);
            arrowDrawPosition += direction;

            if (OwnerCharacter.Weapon is WeaponBow bow)
            {
                Texture2D PixelTexture = GenshinSystemUI.PixelTexture;

                Vector2 texturepos = (- new Vector2(WeaponTexture.Width, WeaponTexture.Height) * 0.5f + bow.StringOffSet) * Projectile.scale;
                texturepos = texturepos.RotatedBy(Projectile.rotation);
                Vector2 stringDirection = drawPosition - toOwner * (ArrowTexture.Width * 0.5f - 2f) + texturepos - arrowDrawPosition;
                stringDirection = stringDirection.RotatedBy(MathHelper.ToRadians(180f));
                Vector2 directionNormalized = stringDirection;
                directionNormalized.Normalize();
                directionNormalized *= 1.5f;
                Color stringColor = bow.StringColor.MultiplyRGB(lightColor);

                while (stringDirection.Length() > 1)
                {
                    spriteBatch.Draw(PixelTexture, drawPosition + texturepos + stringDirection, null, stringColor, Projectile.rotation, PixelTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(PixelTexture, drawPosition - Vector2.Reflect(texturepos + stringDirection, toOwner) , null, stringColor, Projectile.rotation, PixelTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
                    stringDirection -= directionNormalized;
                }
            }

            // Composite arm stuff

            OwnerGenshinPlayer.CompositeArmOffset = toOwner * 12f - direction * 0.1f;
            OwnerGenshinPlayer.CompositeArmAngle = toOwner.ToRotation() + MathHelper.Pi;
        }

        public override void SafePostDrawAdditive(Color lightColor, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < OldPosition.Count; i++)
            {
                Vector2 drawPosition2 = Vector2.Transform(OldPosition[i] - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                float rotation2 = OldRotation[i];
                spriteBatch.Draw(WeaponTexture, drawPosition2, null, lightColor * 0.075f * i, rotation2, WeaponTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            }
        }

        public override void SafeSecondPostDraw(Color lightColor, SpriteBatch spriteBatch) => OwnerGenshinPlayer.DrawCompositeArm(spriteBatch, false, true);
    }
}
