using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.ModObjects.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GenshinMod.Content.Projectiles
{
    public class ProjectileClaymoreNormal : GenshinProjectile
    {
        public Texture2D WeaponTexture;
        public GenshinWeapon Weapon;
        public float acceleration = 0.8f;

        public List<Vector2> OldPosition;
        public List<float> OldRotation;
        public List<int> HitNPC;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Claymore Slash");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 45;
            Projectile.scale = 1f;
            ProjectileTrail = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            AttackWeight = AttackWeight.BLUNT;
            PostDrawAdditive = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            GenshinPlayer ownerPlayer = Owner.GetModPlayer<GenshinPlayer>();
            Weapon = ownerPlayer.CharacterCurrent.Weapon;
            WeaponTexture = ModContent.Request<Texture2D>(Weapon.Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Projectile.width = (int)(WeaponTexture.Width * ownerPlayer.CharacterCurrent.WeaponSize);
            Projectile.height = (int)(WeaponTexture.Height * ownerPlayer.CharacterCurrent.WeaponSize);
            Projectile.scale = ownerPlayer.CharacterCurrent.WeaponSize;
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
            HitNPC = new List<int>();
        }

        public override void SafeAI()
        {
            Vector2 position = Owner.Center + (Vector2.UnitY * TileLength * 4.5f * Projectile.scale).RotatedBy(MathHelper.ToRadians(Projectile.ai[0])) - Projectile.Size * 0.5f;
            Projectile.position = position;

            Vector2 direction = Projectile.Center - Owner.Center;
            Projectile.rotation = direction.ToRotation() + MathHelper.ToRadians(45f);

            Projectile.ai[0] += Projectile.ai[1] * acceleration;
            if (TimeSpent > 17) acceleration *= 0.775f;
            if (TimeSpent < 4) acceleration *= 2.3f;

            // Afterimages
            if (TimeSpent < 30 || Element != GenshinElement.NONE)
            {
                OldPosition.Add(Projectile.Center);
                OldRotation.Add(Projectile.rotation);
                if (OldPosition.Count > 8)
                {
                    OldPosition.RemoveAt(0);
                    OldRotation.RemoveAt(0);
                }
            }
            else if (OldPosition.Count > 1)
            {
                OldPosition.RemoveAt(0);
                OldRotation.RemoveAt(0);
            }
        }

        public override void SafeOnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            HitNPC.Add(target.whoAmI);
        }


        public override bool? CanHitNPC(NPC target)
        {
            if (HitNPC.Contains(target.whoAmI)) return false;
            return base.CanHitNPC(target);
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            GenshinElement infusion = OwnerCharacter.WeaponInfusion;
            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
            SpriteEffects effect = (Projectile.ai[1] < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float rotation = Projectile.rotation + (effect == SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));
            spriteBatch.Draw(WeaponTexture, drawPosition, null, lightColor * 1.5f, rotation, WeaponTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
        }

        public override void SafePostDrawAdditive(Color lightColor, SpriteBatch spriteBatch)
        {
            SpriteEffects effect = (Projectile.ai[1] < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation + (effect == SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));

            if (Element != GenshinElement.NONE)
            {
                Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(WeaponTexture, drawPosition, null, GenshinElementUtils.GetColor(Element) * 0.75f, rotation, WeaponTexture.Size() * 0.5f, Projectile.scale * 1.15f, effect, 0f);
            }

            for (int i = 0; i < OldPosition.Count; i++)
            {
                Vector2 drawPosition2 = Vector2.Transform(OldPosition[i] - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                float rotation2 = OldRotation[i] + (effect == SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));
                if (Element == GenshinElement.NONE)
                    spriteBatch.Draw(WeaponTexture, drawPosition2, null, lightColor * 0.075f * i, rotation2, WeaponTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
                else
                    spriteBatch.Draw(WeaponTexture, drawPosition2, null, GenshinElementUtils.GetColor(Element) * 0.125f * i, rotation2, WeaponTexture.Size() * 0.5f, Projectile.scale * 1.15f, effect, 0f);
            }
        }
    }
}
