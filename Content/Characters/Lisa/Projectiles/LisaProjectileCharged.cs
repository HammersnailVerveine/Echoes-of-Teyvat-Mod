using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace GenshinMod.Content.Characters.Lisa.Projectiles
{
    public class LisaProjectileCharged : GenshinProjectile
    {
        public Texture2D TextureProjectile;
        public List<Vector2> Positions;
        public float lightmult = 1f;
        public float scale = 0f;
        public Vector2 basePosition;
        public Vector2 basePositionPlayer;
        int oldDir = -1;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lizap");
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 60;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            IgnoreICD = true;
            AttackWeight = AttackWeight.LIGHT;
        }

        public override void OnFirstFrame()
        {
            TextureProjectile ??= GetTexture();
            basePosition = Projectile.position;
            basePositionPlayer = Owner.position;
            CalculateNodes();

            Vector2 direction = Projectile.Center - Owner.Center;
            if (direction.Length() > 30f)
            {
                direction.Normalize();
                Vector2 position = Owner.Center + direction;

                SpawnDust<LisaDustRound>(position + direction * 10f, direction, 1f, 1f, 10, 4);
            }
        }

        public override void SafeAI()
        {
            if (TimeSpent < 30) scale += 1 / 25f;
            else if (TimeSpent % 5 == 0) CalculateNodes();
            if (TimeSpent > 50) lightmult *= 0.9f;

            if (TimeSpent == 30)
            {
                Projectile.friendly = true;
                ResetImmunity();

                Vector2 direction = Projectile.Center - Owner.Center;
                int width = (int)Math.Max(Math.Abs(direction.X), Math.Abs(direction.Y));
                direction *= 0.5f;
                Projectile.position = Owner.Center + direction - new Vector2(width * 0.5f, width * 0.5f);
                Projectile.width = width;
                Projectile.height = width;
            }
            else Projectile.friendly = false;
        }

        public void CalculateNodes()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Positions = new List<Vector2>();
            Vector2 direction = basePositionPlayer - basePosition;
            float length = direction.Length();
            float segmentLength = Main.rand.NextFloat(15f, 17f);

            int dir = -oldDir;
            oldDir = -oldDir;
            for (int i = 0; i < length; i += (int)segmentLength)
            {
                dir = -dir;
                Vector2 newPosition = direction.RotatedBy(MathHelper.ToRadians(i / segmentLength * 5f * dir));
                newPosition.Normalize();
                newPosition *= length - i;
                newPosition = basePositionPlayer - newPosition;
                Positions.Add(newPosition);
                SpawnDust<LisaDustRound>(newPosition, Vector2.Zero, 1f, 1f, 10, 1, 2);
            }
        }
        public override void SafeOnHitNPC(NPC target)
        {
            if (OwnerCharacter is CharacterLisa lisa) lisa.TryApplyStackLisa(target);
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            Vector2 lastPosition = Owner.Center;
            Color color = new Color(155, 155, 255) * lightmult;
            foreach (Vector2 position in Positions)
            {
                Vector2 drawPosition = Vector2.Transform(position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(TextureProjectile, drawPosition, null, Color.White * 0.4f * lightmult, Main.rand.NextFloat(3.14f), TextureProjectile.Size() * 0.5f, Projectile.scale * scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.2f, Main.rand.NextFloat(3.14f), TextureProjectile.Size() * 0.5f, Projectile.scale * 1.5f * scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureProjectile, drawPosition, null, color * 0.05f, 0f, TextureProjectile.Size() * 0.5f, Projectile.scale * 2f * scale, SpriteEffects.None, 0f);

                if (TimeSpent >= 30)
                {
                    if (lastPosition != Owner.Center)
                    {
                        Vector2 direction = position - lastPosition;
                        float rotation = Main.rand.NextFloat(3.14f);
                        float length = direction.Length();
                        float factor = (length / (TextureProjectile.Width));
                        direction.Normalize();
                        for (float i = 0; i < length; i += factor / 2f)
                        {
                            Vector2 drawPosition2 = Vector2.Transform(position - direction * i - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                            spriteBatch.Draw(TextureProjectile, drawPosition2, null, Color.White * 0.5f * lightmult, rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * 0.66f, SpriteEffects.None, 0f);
                            spriteBatch.Draw(TextureProjectile, drawPosition2, null, color * 0.2f, rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * 1.4f, SpriteEffects.None, 0f);
                            spriteBatch.Draw(TextureProjectile, drawPosition2, null, color * 0.05f, direction.ToRotation(), TextureProjectile.Size() * 0.5f, Projectile.scale * 1.9f, SpriteEffects.None, 0f);
                            SpawnDust<LisaDustRound>(position - direction * i, Vector2.Zero, 1f, 1f, 10, 1, 90);
                        }
                    }
                    lastPosition = position;
                }
            }
        }
    }
}
