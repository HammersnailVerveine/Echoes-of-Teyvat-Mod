using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Amber.Projectiles
{
    public class AmberProjectileBurstArrow : GenshinProjectile
    {
        public static Texture2D TrailTexture;
        public Texture2D ArrowTexture;

        public List<Vector2> OldPosition = new();
        public List<float> OldRotation = new();

        public bool Disappearing;
        public float ColorMult = 0f;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 30;
            Projectile.scale = 1f;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            PostDrawAdditive = true;
        }
        
        public override void OnFirstFrame()
        {
            GenshinPlayer ownerPlayer = Owner.GetModPlayer<GenshinPlayer>();
            TrailTexture ??= GetTexture();
            ArrowTexture = ModContent.Request<Texture2D>(OwnerCharacter.Weapon.Texture + "_Arrow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Disappearing = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 10;
            Projectile.penetrate = -1;
            return false;
        }

        public override void SafeAI()
        {
            Projectile.scale = 0.75f;
            if (Projectile.timeLeft <= 10) Disappearing = true;

            Projectile.tileCollide = TimeSpent > 15;

            if (!Disappearing)
            {
                // Gravity
                Projectile.rotation = Projectile.velocity.ToRotation();

                // Afterimages
                OldPosition.Add(Projectile.Center);
                OldRotation.Add(Projectile.rotation);
            }

            if (OldPosition.Count > 10 || (Disappearing && OldPosition.Count > 0))
            {
                OldPosition.RemoveAt(0);
                OldRotation.RemoveAt(0);
            }

            if (ColorMult < 1f) ColorMult += 0.1f;
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            if (!Disappearing && ArrowTexture != null)
            {
                Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(ArrowTexture, drawPosition, null, lightColor * 1.5f * ColorMult, Projectile.rotation, ArrowTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            }
        }

        public override void SafePostDrawAdditive(Color lightColor, SpriteBatch spriteBatch)
        {
            if (ArrowTexture == null) return;
            float rotation = Projectile.rotation;

            if (!Disappearing)
            {
                Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(ArrowTexture, drawPosition, null, GenshinElementUtils.GetColor(Element) * 0.75f * ColorMult, rotation, ArrowTexture.Size() * 0.5f, Projectile.scale * 1.15f, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < OldPosition.Count; i++)
            {
                Vector2 drawPosition2 = Vector2.Transform(OldPosition[i] - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(TrailTexture, drawPosition2, null, GenshinElementUtils.GetColor(Element) * 0.08f * i * ColorMult, Projectile.rotation, TrailTexture.Size() * 0.5f, Projectile.scale * 1.1f, SpriteEffects.None, 0f);
            }
        }
    }
}
