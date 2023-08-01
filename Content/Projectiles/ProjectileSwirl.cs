using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GenshinMod.Content.Projectiles
{
    public class ProjectileSwirl : GenshinProjectile
    {
        public Texture2D TextureProjectile;
        float alpha = 1f;
        float rotationRand = 0f;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Swirl");
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 20;
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
            rotationRand = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void SafeAI()
        {
            Projectile.friendly = FirstFrame;
            alpha -= (1 / 20f);
            Projectile.rotation += 0.1f;
            Projectile.scale *= 1.1f;
            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast);
        }

        public override void SafePostDrawAdditive(Color lightColor, SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
            Color color = GenshinElementUtils.GetColor(Element) * alpha;

            spriteBatch.Draw(TextureProjectile, drawPosition, null, color, Projectile.rotation, TextureProjectile.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * 1.1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation + MathHelper.Pi, TextureProjectile.Size() * 0.5f, Projectile.scale * 1.1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation + MathHelper.Pi / 2f, TextureProjectile.Size() * 0.5f, Projectile.scale * 1.1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation - MathHelper.Pi / 2f, TextureProjectile.Size() * 0.5f, Projectile.scale * 1.1f, SpriteEffects.None, 0f);

            spriteBatch.Draw(TextureProjectile, drawPosition, null, color, Projectile.rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * 0.75f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * 0.85f, SpriteEffects.None, 0f);

            spriteBatch.Draw(TextureProjectile, drawPosition, null, color, Projectile.rotation + rotationRand, TextureProjectile.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation + rotationRand, TextureProjectile.Size() * 0.5f, Projectile.scale * 1.1f * 0.5f, SpriteEffects.None, 0f);

            spriteBatch.Draw(TextureProjectile, drawPosition, null, color, Projectile.rotation + rotationRand, TextureProjectile.Size() * 0.5f, Projectile.scale * 0.75f * 0.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.5f, Projectile.rotation + rotationRand, TextureProjectile.Size() * 0.5f, Projectile.scale * 0.85f * 0.5f, SpriteEffects.None, 0f);
        }
    }
}
