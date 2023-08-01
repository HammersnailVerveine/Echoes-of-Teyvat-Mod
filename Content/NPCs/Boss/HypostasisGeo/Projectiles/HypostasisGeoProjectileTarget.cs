using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.NPCs.Boss.HypostasisGeo.Projectiles
{
    public class HypostasisGeoProjectileTarget : GenshinProjectile
    {
        public static Texture2D TextureProj;
        public static Texture2D TextureProjGlow;
        public Color ColorGeo;
        public static Color ColorBrown;

        private Vector2 position;
        private int RotationQuarter = 0;

        private Player PlayerTarget => Main.player[(int)Projectile.ai[0]];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hypostasis Blast");
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 480;
            Projectile.scale = 1f;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Element = GenshinElement.GEO;
        }

        public override void OnSpawn(IEntitySource source)
        {
            TextureProj ??= GetTexture();
            TextureProjGlow ??= ModContent.Request<Texture2D>("GenshinMod/Content/NPCs/Boss/HypostasisGeo/Projectiles/HypostasisGeoProjectileTargetGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            ColorGeo = GenshinElementUtils.GetColor(GenshinElement.GEO);
            ColorBrown = new Color(115, 75, 45);
            Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2f);
            position = new Vector2(0f, Projectile.position.Y);
        }

        public override void SafeAI()
        {
            if (Projectile.timeLeft > 150)
            {
                Projectile.position.Y = position.Y;
                Projectile.position.X = PlayerTarget.Center.X + position.X - Projectile.width / 2f;

                if (Math.Abs(position.X) < 56f || Math.Sign(position.X) != Math.Sign(PlayerTarget.velocity.X))
                    position.X += PlayerTarget.velocity.X * 1.5f;
                if (PlayerTarget.velocity.X == 0) position.X *= 0.975f;
            }
            else
            {
                if (Projectile.tileCollide)
                {  // Didn't hit the ground yet
                    Projectile.hostile = true;
                    Projectile.velocity.Y += 0.5f;
                    Projectile.tileCollide = true;
                }
            }

            if (Projectile.timeLeft > 180)
            {
                Projectile.rotation += 0.016f;
                if (Projectile.rotation > MathHelper.TwoPi / 4f)
                {
                    RotationQuarter++;
                    Projectile.rotation %= MathHelper.TwoPi / 4f;
                }
            }
            else
            {
                Projectile.rotation = Projectile.tileCollide ? Projectile.rotation * 0.9f : 0f;
            }

            for (int length = Projectile.oldPos.Length - 1; length > 0; length--)
            {
                Projectile.oldPos[length] = Projectile.oldPos[length - 1];
            }
            Projectile.oldPos[0] = Projectile.position;

            SpawnDust<HypostasisGeoDust>(0f, 1f, 0, 1, 80);
            SpawnDust<HypostasisGeoDustSmall>(0f, 1f, 16, 1, 25);
        }

        public override void Kill(int timeLeft)
        {
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
            float Glow = 1f - ((float)Math.Sin(TimeSpent * 0.03f) * 0.15f + 0.15f);
            float scaleMultGlow = ((float)Math.Sin(Projectile.timeLeft * 0.05f)) * 0.025f + 1.05f;
            float scaleMult = TimeSpent < 60 ? TimeSpent / 60f : 1f * (Projectile.timeLeft > 30 ? 1f : (Projectile.timeLeft + 1) / 31f);
            float rotation = Projectile.rotation + (MathHelper.TwoPi / 4f) * RotationQuarter;

            spriteBatch.Draw(TextureProj, drawPosition, null, ColorGeo * 0.1f * Glow, rotation, TextureProj.Size() * 0.5f, Projectile.scale * scaleMult * 1.3f * scaleMultGlow, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProj, drawPosition, null, ColorGeo * 0.05f * Glow, -rotation, TextureProj.Size() * 0.5f, Projectile.scale * scaleMult * 1.5f * scaleMultGlow, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProj, drawPosition, null, ColorBrown, rotation, TextureProj.Size() * 0.5f, Projectile.scale * scaleMult, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjGlow, drawPosition, null, ColorGeo * 0.5f * Glow, rotation, TextureProjGlow.Size() * 0.5f, Projectile.scale * scaleMult, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjGlow, drawPosition, null, ColorGeo * 0.3f * Glow, rotation, TextureProjGlow.Size() * 0.5f, Projectile.scale * scaleMultGlow * scaleMult, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureProjGlow, drawPosition, null, ColorGeo * 0.15f * Glow, rotation, TextureProjGlow.Size() * 0.5f, Projectile.scale * scaleMultGlow * scaleMult * 1.1f, SpriteEffects.None, 0f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0f;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 90;
            Projectile.hostile = false;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            SpawnDust<HypostasisGeoDust>(Projectile.Center, Projectile.velocity, Main.rand.NextFloat(2.5f) + 2.5f, 1f, 56, 4);
            SpawnDust<HypostasisGeoDustSmall>(Projectile.Center, Projectile.velocity, Main.rand.NextFloat(2.5f) + 2.5f, 1f, 72, 12);
            return false;
        }
    }
}
