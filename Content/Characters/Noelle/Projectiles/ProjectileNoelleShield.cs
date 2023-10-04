using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Noelle.Projectiles
{
    public class ProjectileNoelleShield : GenshinProjectile
    {
        public static Texture2D TextureSelf;

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 20;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            AttackWeight = AttackWeight.BLUNT;
        }

        public override void SafeAI()
        {
            Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.1f);
            Projectile.scale *= 1.1f;
            if (FirstFrame)
            {
                Projectile.friendly = true;
                ResetImmunity();
                SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
            }
            else
                Projectile.friendly = false;
        }

        public override void OnFirstFrame()
        {
            TextureSelf ??= ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Main.GameViewMatrix.EffectMatrix);
            Color color = GenshinElementUtils.GetColor(GenshinElement.GEO);
            spriteBatch.Draw(TextureSelf, drawPosition, null, color * (1.2f - Projectile.scale * 0.75f), Projectile.rotation, TextureSelf.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSelf, drawPosition, null, color * (1.2f - Projectile.scale * 0.75f) * 0.15f, Projectile.rotation, TextureSelf.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureSelf, drawPosition, null, color * (1.2f - Projectile.scale * 0.75f) * 0.5f, Projectile.rotation + ((float)Math.PI / 2f), TextureSelf.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
        }
    }
}
