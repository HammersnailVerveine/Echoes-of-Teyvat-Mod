using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
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
            ElementApplication = ElementApplicationMedium;
        }

        public override void OnFirstFrame()
        {
            GenshinPlayer ownerPlayer = Owner.GetModPlayer<GenshinPlayer>();
            TrailTexture ??= GetTexture();
            ArrowTexture = ModContent.Request<Texture2D>(ownerPlayer.CharacterCurrent.Weapon.Texture + "_Arrow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            if (Main.netMode != NetmodeID.Server)
            {
                Projectile.width = (int)(ArrowTexture.Width * 0.25f);
            }

            Projectile.height = 4;
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
        }

        public override void SafeOnHitNPC(NPC target)
        {
            Projectile.ai[0] = 1f;
            Projectile.friendly = false;
            Projectile.timeLeft = 30;
            Projectile.penetrate = -1;
            Projectile.netUpdate = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] = 1f;
            Projectile.friendly = false;
            Projectile.timeLeft = 30;
            Projectile.penetrate = -1;
            Projectile.netUpdate = true;
            return false;
        }

        public override void SafeAI()
        {
            if (Projectile.ai[0] != 1f)
            {
                // Gravity
                Projectile.velocity.Y += 0.25f;
                Projectile.rotation = Projectile.velocity.ToRotation();

                // Afterimages
                OldPosition.Add(Projectile.Center);
                OldRotation.Add(Projectile.rotation);

                ResetImmunity();
            }
            else
            {
                Projectile.velocity *= 0f;
            }

            if (OldPosition.Count > 15 || (Projectile.ai[0] == 1f && OldPosition.Count > 0))
            {
                OldPosition.RemoveAt(0);
                OldRotation.RemoveAt(0);
            }
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            if (Projectile.ai[0] != 1f)
            {
                Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(ArrowTexture, drawPosition, null, lightColor * 1.5f, Projectile.rotation, ArrowTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            }
        }

        public override void SafePostDrawAdditive(Color lightColor, SpriteBatch spriteBatch)
        {
            float rotation = Projectile.rotation;

            if (Projectile.ai[0] != 1f)
            {
                if (Element != GenshinElement.NONE)
                {
                    Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                    spriteBatch.Draw(ArrowTexture, drawPosition, null, GenshinElementUtils.GetColor(Element) * 0.75f, rotation, ArrowTexture.Size() * 0.5f, Projectile.scale * 1.15f, SpriteEffects.None, 0f);
                }
            }

            for (int i = 0; i < OldPosition.Count; i++)
            {
                Vector2 drawPosition2 = Vector2.Transform(OldPosition[i] - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                if (Element == GenshinElement.NONE)
                    spriteBatch.Draw(TrailTexture, drawPosition2, null, lightColor * 0.03f * i, Projectile.rotation, TrailTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
                else
                    spriteBatch.Draw(TrailTexture, drawPosition2, null, GenshinElementUtils.GetColor(Element) * 0.05f * i, Projectile.rotation, TrailTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            }
        }
    }
}
