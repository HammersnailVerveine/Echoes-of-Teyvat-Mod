using GenshinMod.Common.GameObjects;
using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;

namespace GenshinMod.Content.Characters.Barbara.Projectiles
{
    public class BarbaraProjectileBurstCircle : GenshinProjectile
    {
        public static Texture2D texture;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Barbara Burst Circle");
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 70;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0.25f;
            Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2f);
            texture ??= GetTexture();
        }

        public override void SafeAI()
        {
            Projectile.position = Owner.Center - new Vector2(Projectile.width / 2, Projectile.height / 2);
            Projectile.rotation -= 0.01f * (Projectile.ai[0] == 0.2f ? -1 : 1);
            Projectile.scale *= 1.04f;

            SpawnDust<BarbaraDustBubble>(0.5f, Main.rand.NextFloat(1f, 2f), (int)(10 * Projectile.scale), 1, 3);
            SpawnDust<BarbaraDustStar>(0.2f, Main.rand.NextFloat(1f, 2f), (int)(10 * Projectile.scale), 1, 10);
            SpawnDust<BarbaraDustStarBig>(0.2f, Main.rand.NextFloat(1f, 2f), (int)(10 * Projectile.scale), 1, 20);
            if (Projectile.scale > 1f)
            {
                Vector2 position = new Vector2(0f, Main.rand.NextFloat(Projectile.height / 2f - 5f, Projectile.height / 2f + 5f) * Projectile.scale * 0.75f).RotatedByRandom(MathHelper.ToRadians(360));
                SpawnDust<BarbaraDustStar>(Projectile.Center + position, Vector2.Zero, 0.25f, 1.2f, 0, 1);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SpawnDust<BarbaraDustBubble>(0.5f, Main.rand.NextFloat(1f, 2f), (int)(10 * Projectile.scale), 15);
            for (int i = 0; i < 15; i++)
            {
                Vector2 position = new Vector2(0f, Main.rand.NextFloat(Projectile.height / 2f - 5f, Projectile.height / 2f + 5f) * Projectile.scale * 0.75f).RotatedByRandom(MathHelper.ToRadians(360));
                SpawnDust<BarbaraDustStar>(Projectile.Center + position, Vector2.Zero, 0.25f, 1.2f, 15, 1);
                Vector2 position2 = new Vector2(0f, Main.rand.NextFloat(Projectile.height / 2f - 5f, Projectile.height / 2f + 5f) * Projectile.scale * 0.75f).RotatedByRandom(MathHelper.ToRadians(360));
                SpawnDust<BarbaraDustStarBig>(Projectile.Center + position2, Vector2.Zero, 0.25f, 1.2f, 15, 1);
            }

            foreach (GenshinCharacter character in OwnerGenshinPlayer.CharacterTeam)
                character.Heal(Projectile.damage); // TEMP
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
            Color color = lightColor * 1.1f * (1 - Projectile.scale / 3);
            float randomRotation = -Projectile.rotation + 0.3f * Projectile.scale;
            spriteBatch.Draw(texture, drawPosition, null, color, randomRotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition, null, color, -randomRotation + 0.5f, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition, null, color * 0.7f, randomRotation * 0.8f + 0.3f, texture.Size() * 0.5f, Projectile.scale * 1.25f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition, null, color * 0.7f, -randomRotation * 0.8f + 1.5f, texture.Size() * 0.5f, Projectile.scale * 1.25f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition, null, color * 0.4f, randomRotation * 0.6f + 1.8f, texture.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition, null, color * 0.4f, -randomRotation * 0.6f + 2.5f, texture.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition, null, color * 0.4f, randomRotation * 0.6f + 1.8f, texture.Size() * 0.5f, Projectile.scale * 0.75f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition, null, color * 0.4f, -randomRotation * 0.6f + 2.5f, texture.Size() * 0.5f, Projectile.scale * 0.75f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition, null, color * 0.4f, randomRotation * 0.6f + 1.8f, texture.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition, null, color * 0.4f, -randomRotation * 0.6f + 2.5f, texture.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
        }
    }
}
