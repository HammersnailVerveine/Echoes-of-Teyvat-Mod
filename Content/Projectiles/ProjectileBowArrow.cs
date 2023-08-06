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
    public class ProjectileBowArrow : GenshinProjectile
    {
        public static Texture2D TrailTexture;
        public Texture2D ArrowTexture;

        public List<Vector2> OldPosition;
        public List<float> OldRotation;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 300;
            Projectile.scale = 1f;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            PostDrawAdditive = true;
            AttackWeight = AttackWeight.LIGHT;
        }

        public override void OnSpawn(IEntitySource source)
        {
            GenshinPlayer ownerPlayer = Owner.GetModPlayer<GenshinPlayer>();
            TrailTexture ??= GetTexture();
            ArrowTexture = ModContent.Request<Texture2D>(ownerPlayer.CharacterCurrent.Weapon.Texture + "_Arrow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Projectile.width = (int)(ArrowTexture.Width * ownerPlayer.CharacterCurrent.WeaponSize);
            Projectile.height = (int)(ArrowTexture.Height * ownerPlayer.CharacterCurrent.WeaponSize);
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
        }

        public override void SafeAI()
        {
            // Gravity
            Projectile.velocity.Y += 0.25f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Afterimages
            OldPosition.Add(Projectile.Center);
            OldRotation.Add(Projectile.rotation);
            if (OldPosition.Count > 15)
            {
                OldPosition.RemoveAt(0);
                OldRotation.RemoveAt(0);
            }
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
                Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(ArrowTexture, drawPosition, null, lightColor * 1.5f, Projectile.rotation, ArrowTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
        }

        public override void SafePostDrawAdditive(Color lightColor, SpriteBatch spriteBatch)
        {
            SpriteEffects effect = (Projectile.ai[1] < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation;

            if (Element != GenshinElement.NONE)
            {
                Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(ArrowTexture, drawPosition, null, GenshinElementUtils.GetColor(Element) * 0.75f, rotation, ArrowTexture.Size() * 0.5f, Projectile.scale * 1.15f, effect, 0f);
            }

            for (int i = 0; i < OldPosition.Count; i++)
            {
                Vector2 drawPosition2 = Vector2.Transform(OldPosition[i] - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                float rotation2 = Projectile.rotation;
                if (Element == GenshinElement.NONE)
                    spriteBatch.Draw(TrailTexture, drawPosition2, null, lightColor * 0.03f * i, rotation2, TrailTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
                else
                    spriteBatch.Draw(TrailTexture, drawPosition2, null, GenshinElementUtils.GetColor(Element) * 0.05f * i, rotation2, TrailTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
            }
        }
    }
}
