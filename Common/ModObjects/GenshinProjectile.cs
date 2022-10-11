using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Common.ModObjects
{
    public abstract class GenshinProjectile : ModProjectile
    {
        public bool projectileTrail = false; // Will the projectile leave a trail of afterimages ?
        public float projectileTrailOffset = 0f; // Offcenters the afterimages a bit. useless without projectileTrail activated. Looks terrible on most projectiles.

        public virtual void SafeAI() { }
        public virtual void SafePostAI() { }
        public virtual bool SafePreDraw(SpriteBatch spriteBatch, Color lightcolor) => true;

        public int timeSpent = 0;
        public bool IsLocalOwner => Projectile.owner == Main.myPlayer;
        public Player Owner => Main.player[Projectile.owner];
        public bool FirstFrame => timeSpent == 1;

        public Vector2 VelocityImmobile => Projectile.velocity * 0.0000001f; // Returns an almost immobile velocity, so projectiles spawned from this have the corrent kb direction

        public void Bounce(Vector2 oldVelocity, float speedMult = 1f, bool reducePenetrate = false)
        {
            if (reducePenetrate)
            {
                Projectile.penetrate--;
                if (Projectile.penetrate < 0) Projectile.Kill();
            }
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X * speedMult;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y * speedMult;
        }

        public void ResetImmunity()
        {
            for (int l = 0; l < Main.npc.Length; l++)
            {
                NPC target = Main.npc[l];
                if (Projectile.Hitbox.Intersects(target.Hitbox))
                {
                    target.immune[Projectile.owner] = 0;
                }
            }
        }

        public sealed override void AI()
        {
            timeSpent++;
            SafeAI();
        }

        public sealed override void PostAI()
        {
            SafePostAI();
            if (projectileTrail)
            {
                PostAITrail();
            }
        }

        public sealed override bool PreDraw(ref Color lightColor)
        {
            if (projectileTrail)
            {
                PreDrawTrail(Main.spriteBatch, lightColor);
            }
            return SafePreDraw(Main.spriteBatch, lightColor);
        }

        public int SpawnProjectile(Vector2 position, Vector2 velocity, int type, int damage, float knockback, float ai0 = 0, float ai1 = 0)
        {
            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity, type, damage, knockback, Projectile.owner, ai0, ai1);
            return proj;
        }

        public void PreDrawTrail(SpriteBatch spriteBatch, Color lightColor)
        {
            float offSet = projectileTrailOffset + 0.5f;
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * offSet, Projectile.height * offSet);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0.3f);
            }
        }

        public void PostAITrail()
        {
            for (int length = Projectile.oldPos.Length - 1; length > 0; length--)
            {
                Projectile.oldPos[length] = Projectile.oldPos[length - 1];
            }
            Projectile.oldPos[0] = Projectile.position;
        }

        public void spawnExplosionGore()
        {
            for (int g = 0; g < 2; g++)
            {
                int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1f + Main.rand.NextFloat();
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                Main.gore[goreIndex].rotation = Main.rand.NextFloat();
                goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1f + Main.rand.NextFloat();
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                Main.gore[goreIndex].rotation = Main.rand.NextFloat();
                goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1f + Main.rand.NextFloat();
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
                Main.gore[goreIndex].rotation = Main.rand.NextFloat();
                goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1f + Main.rand.NextFloat();
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
                Main.gore[goreIndex].rotation = Main.rand.NextFloat();
            }
        }

        public void SpawnDust<T>(float velocity = 0f, float scale = 1f, int offSet = 10, int quantity = 1, int chanceDenominator = 1) where T : ModDust => SpawnDust(ModContent.DustType<T>(), velocity, scale, offSet, quantity, chanceDenominator);

        public void SpawnDust(int type, float velocity = 0f, float scale = 1f, int offSet = 10, int quantity = 1, int chanceDenominator = 1)
        {
            if (!Main.rand.NextBool(chanceDenominator)) return;
            for (int i = 0; i < quantity; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.position - new Vector2(offSet, offSet), Projectile.width + offSet * 2, Projectile.height + offSet * 2, type)];
                dust.velocity = new Vector2(Main.rand.NextFloat(-velocity, velocity), Main.rand.NextFloat(-velocity, velocity));
            }
        }

        public void SpawnDust<T>(Vector2 position, Vector2 direction, float velocity = 0f, float scale = 1f, int offSet = 10, int quantity = 1, int chanceDenominator = 1) where T : ModDust => SpawnDust(ModContent.DustType<T>(), position, direction, velocity, scale, offSet, quantity, chanceDenominator);

        public void SpawnDust(int type, Vector2 position, Vector2 direction, float velocity = 0f, float scale = 1f, int offSet = 10, int quantity = 1, int chanceDenominator = 1)
        {
            if (!Main.rand.NextBool(chanceDenominator)) return;
            for (int i = 0; i < quantity; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(position - new Vector2(offSet, offSet), offSet * 2, offSet * 2, type)];
                dust.velocity = new Vector2(Main.rand.NextFloat(-velocity, velocity), Main.rand.NextFloat(-velocity, velocity)) + direction;
            }
        }
    }
}
