using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.ModObjects.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Jean.Projectiles
{
    public class ProjectileJeanSkill : GenshinProjectile
    {
        public Texture2D WeaponTexture;

        public List<Vector2> OldPosition;
        public List<float> OldRotation;

        private const float Reach = 120f; // 7.5 tiles 

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jean Slash");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 300;
            Projectile.scale = 1f;
            ProjectileTrail = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            PostDrawAdditive = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

        public override void OnSpawn(IEntitySource source)
        {
            GenshinPlayer ownerPlayer = Owner.GetModPlayer<GenshinPlayer>();
            WeaponTexture = ModContent.Request<Texture2D>(ownerPlayer.CharacterCurrent.Weapon.Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Projectile.width = (int)(WeaponTexture.Width * ownerPlayer.CharacterCurrent.WeaponSize);
            Projectile.height = (int)(WeaponTexture.Height * ownerPlayer.CharacterCurrent.WeaponSize);
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
        }

        public override void SafeAI()
        {
            // position & rotation
            Vector2 direction = Vector2.Zero;
            Projectile.scale = OwnerCharacter.WeaponSize * 0.8f;
            if (IsLocalOwner)
            {
                Vector2 target = Main.MouseWorld;
                direction = target - Owner.Center;
                direction.Normalize();
                Projectile.position = Owner.Center + GetOwnerArmOffset() + (direction * TileLength * 3.25f * Projectile.scale) - Projectile.Size * 0.5f;
                Projectile.rotation = direction.ToRotation() + MathHelper.ToRadians(45f);
            }
            else
            {
                // mp sync : todo
            }

            // Afterimages
            if (TimeSpent % 2 == 0)
            {
                OldPosition.Add(Projectile.Center);
                OldRotation.Add(Projectile.rotation);
                if (OldPosition.Count > 5)
                {
                    OldPosition.RemoveAt(0);
                    OldRotation.RemoveAt(0);
                }
            }

            // Enemy attraction

            Vector2 attactionPos = Projectile.Center + direction * TileLength * 4f;
            if (TimeSpent % 6 == 0)
            {
                int type = ModContent.ProjectileType<ProjectileJeanSkillSwirlEffect>();
                SpawnProjectile(attactionPos, Vector2.Zero, type, 0, 0f, GenshinElement.ANEMO, Common.GameObjects.AbilityType.SKILL);
            }

            foreach (NPC npc in Main.npc)
            {
                if (IsValidTarget(npc, true) && npc.knockBackResist > 0f)
                {
                    float distance = npc.Center.Distance(attactionPos);
                    bool veryClose = false;

                    if (distance < 1f)
                    {
                        distance = 1f;
                        veryClose = true;
                    }

                    if (distance < Reach)
                    { // todo : implement npc kb resist in the equation 
                        npc.velocity *= 0.5f / (Reach / distance);
                        if (!veryClose)
                        {
                            Vector2 attractionDirection = attactionPos - npc.Center;
                            attractionDirection.Normalize();
                            float attractionMult = (Reach / distance);
                            if (attractionMult > 5f) attractionMult = 5f;
                            npc.velocity += attractionDirection * attractionMult;
                        }
                    }
                }
            }

            OwnerGenshinPlayer.CompositeArmOffset = direction * 8f;
            OwnerGenshinPlayer.CompositeArmAngle = direction.ToRotation();
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
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

        public override void SafeSecondPostDraw(Color lightColor, SpriteBatch spriteBatch) => OwnerGenshinPlayer.DrawCompositeArm(spriteBatch);
    }
}
