using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;

namespace GenshinMod.Content.Characters.Amber.Projectiles
{
    public class AmberProjectileExplosion : GenshinProjectile
    {
        public Texture2D TextureProjectile;
        float alpha = 1f;

        public override void SetDefaults()
        {
            Projectile.width = 240;
            Projectile.height = 240;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 20;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Main.projFrames[Projectile.type] = 9;
            ElementApplication = ElementApplicationMedium;
            IgnoreICD = true;
            AttackWeight = AttackWeight.BLUNT;
            PostDrawAdditive = true;
            ElementalParticles = 4;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2f);
            TextureProjectile ??= GetTexture();
            ResetImmunity();
        }

        public override void SafeAI()
        {
            Projectile.friendly = FirstFrame;
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.5f);
            if (TimeSpent % 2 == 0)
            {
                Projectile.frame++;
                if (Projectile.frame > 9)
                    Projectile.Kill();
            }

            if (TimeSpent > 6) alpha -= (1 / 20f) * 2;
        }

        public override void SafePostDrawAdditive(Color lightColor, SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
            float rotation = Projectile.rotation + (OwnerGenshinPlayer.LastUseDirection == 1 ? 0f : MathHelper.ToRadians(90f));
            Rectangle rectangle = TextureProjectile.Bounds;
            rectangle.Height /= Main.projFrames[Projectile.type];
            rectangle.Y += Projectile.frame * rectangle.Height;

            spriteBatch.Draw(TextureProjectile, drawPosition, rectangle, Color.White * alpha * 0.6f, rotation, rectangle.Size() * 0.5f, Projectile.scale * 2.5f * (TimeSpent / 10f), SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjectile, drawPosition, rectangle, Color.White * alpha * 0.7f, rotation, rectangle.Size() * 0.5f, Projectile.scale * 2.1f * (TimeSpent / 10f), SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjectile, drawPosition, rectangle, Color.White * alpha * 0.8f, rotation + (float)Math.PI * 0.25f, rectangle.Size() * 0.5f, Projectile.scale * 1.3f * (TimeSpent / 10f), SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjectile, drawPosition, rectangle, Color.White * alpha, rotation + (float)Math.PI * 0.25f, rectangle.Size() * 0.5f, Projectile.scale * (TimeSpent / 10f), SpriteEffects.None, 0f);
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
            float rotation = Projectile.rotation + (OwnerGenshinPlayer.LastUseDirection == 1 ? 0f : MathHelper.ToRadians(90f));
            Rectangle rectangle = TextureProjectile.Bounds;
            rectangle.Height /= Main.projFrames[Projectile.type];
            rectangle.Y += Projectile.frame * rectangle.Height;

            spriteBatch.Draw(TextureProjectile, drawPosition, rectangle, Color.White * alpha * 0.8f, rotation + (float)Math.PI * 1.25f, rectangle.Size() * 0.5f, Projectile.scale * 1.3f * (TimeSpent / 10f), SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjectile, drawPosition, rectangle, Color.White * alpha, rotation + (float)Math.PI * 0.25f, rectangle.Size() * 0.5f, Projectile.scale * (TimeSpent / 10f) * 0.8f, SpriteEffects.None, 0f);
        }
    }
}
