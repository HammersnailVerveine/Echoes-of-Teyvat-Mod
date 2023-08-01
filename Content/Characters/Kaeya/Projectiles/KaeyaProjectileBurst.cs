using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Kaeya.Projectiles
{
    public class KaeyaProjectileBurst : GenshinProjectile
    {
        public static Texture2D TextureProjectile;
        public List<Vector2> OldPosition;
        public List<float> OldRotation;
        public List<int> HitNPC;
        public List<int> HitNPCTimer;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Kaeya Icicle");
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 60 * 8;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override void OnSpawn(IEntitySource source)
        {
            TextureProjectile ??= ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
            HitNPC = new List<int>();
            HitNPCTimer = new List<int>();

            for (int i = 0; i < 8; i++)
            {
                Vector2 direction = Vector2.UnitY.RotatedByRandom(180f) * Main.rand.NextFloat(3f, 6f);
                SpawnDust<KaeyaDustFrost>(Owner.Center, direction, 1f, Main.rand.NextFloat(1f, 2.5f), 10);
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 direction = Vector2.UnitY.RotatedByRandom(180f) * Main.rand.NextFloat(2f, 5f);
                SpawnDust<KaeyaDustFrostBig>(Owner.Center, direction, 1f, Main.rand.NextFloat(1f, 2.5f), 10);
            }
        }

        public override void SafeAI()
        {
            Vector2 position = Owner.Center + (Vector2.UnitY * TileLength * 5f).RotatedBy(MathHelper.ToRadians(Projectile.ai[0])) - Projectile.Size * 0.5f;
            Projectile.position = position;
            Vector2 direction = Projectile.Center - Owner.Center;
            Projectile.rotation = direction.ToRotation();

            Projectile.ai[0] += 3f;

            SpawnDust<KaeyaDustFrost>(1f, 1f, 10, 1, 8);
            SpawnDust<KaeyaDustFrostBig>(0.75f, 1f, 5, 1, 25);

            // Afterimages
            OldPosition.Add(Projectile.Center);
            OldRotation.Add(Projectile.rotation);
            if (OldPosition.Count > 15)
                if (OldPosition.Count > 15)
                {
                    OldPosition.RemoveAt(0);
                    OldRotation.RemoveAt(0);
                }

            // Hit limit
            for (int i = HitNPCTimer.Count - 1; i >= 0; i--)
            {
                HitNPCTimer[i]--;
                if (HitNPCTimer[i] <= 0)
                {
                    HitNPCTimer.RemoveAt(0);
                    HitNPC.RemoveAt(0);
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            SpawnDust<KaeyaDustFrost>(1f, 1f, 10, 8);
            SpawnDust<KaeyaDustFrostBig>(0.75f, 1f, 5, 3);
        }
        public override void SafeOnHitNPC(NPC target)
        {
            HitNPC.Add(target.whoAmI);
            HitNPCTimer.Add(60);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (HitNPC.Contains(target.whoAmI)) return false;
            return base.CanHitNPC(target);
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
            spriteBatch.Draw(TextureProjectile, drawPosition, null, Color.White * 0.5f, Projectile.rotation, TextureProjectile.Size() * 0.5f, Projectile.scale * 1f, SpriteEffects.None, 0f);

            for (int i = 0; i < OldPosition.Count; i++)
            {
                Vector2 drawPosition2 = Vector2.Transform(OldPosition[i] - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(TextureProjectile, drawPosition2, null, Color.White * 0.02f * i, Projectile.rotation, TextureProjectile.Size() * 0.5f, 0.2f + 0.0425f * i, SpriteEffects.None, 0f);
            }
        }
    }
}
