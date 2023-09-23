using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.ModObjects.Weapons;
using GenshinMod.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Amber.Projectiles
{
    public class AmberProjectileSkillDoll : GenshinProjectile
    {
        public Texture2D ProjectileTexture;

        public List<Vector2> OldPosition;
        public List<float> OldRotation;

        bool HitGround = false;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 480;
            Projectile.scale = 1f;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            PostDrawAdditive = true;
            AttackWeight = AttackWeight.LIGHT;
        }
        
        public override void OnFirstFrame()
        {
            ProjectileTexture ??= GetTexture();
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.Y *= 0f;
            HitGround = true;
            return false;
        }

        public override void Kill(int timeLeft)
        {
            int type = ModContent.ProjectileType<AmberProjectileExplosion>();
            SpawnProjectile(Projectile.Center, VelocityImmobile, type, Projectile.damage, Projectile.knockBack);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }

        public override void SafeAI()
        {
            Projectile.velocity.Y += 0.25f;
            Projectile.velocity.X *= 0.99f;
            if (HitGround) Projectile.velocity.X *= 0.95f;
            Projectile.rotation = Projectile.velocity.X / 10f;

            if (Projectile.velocity.X > 0.5f && TimeSpent < 60) Projectile.timeLeft++;

            if (TimeSpent % 2 == 0)
            {
                if (Projectile.velocity.X > 1f)
                {
                    OldPosition.Add(Projectile.Center);
                    OldRotation.Add(Projectile.rotation);

                    if (OldPosition.Count > 5)
                    {
                        OldPosition.RemoveAt(0);
                        OldRotation.RemoveAt(0);
                    }
                }
                else if (OldPosition.Count > 0)
                {
                    OldPosition.RemoveAt(0);
                    OldRotation.RemoveAt(0);
                }
            }

            // destruction by shooting on the doll

            if (IsLocalOwner)
            {
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.ModProjectile is Content.Projectiles.ProjectileBowArrow arrow)
                    {
                        if (arrow.Projectile.Center.Distance(Projectile.Center) < 32f && arrow.OwnerCharacter is CharacterAmber && arrow.Element == GenshinElement.PYRO && arrow.Projectile.ai[0] != 1f)
                        {
                            Projectile.damage *= 3;
                            Projectile.Kill();
                            arrow.Projectile.ai[0] = 1f;
                            arrow.Projectile.netUpdate = true;
                        }
                    }
                }
            }

            // Taunt

            foreach (NPC npc in Main.npc)
            {
                if (npc.ModNPC is GenshinNPC genshinNPC)
                {
                    if (IsValidTarget(npc) && npc.Center.Distance(Projectile.Center) < 160f)
                        genshinNPC.TargetPositionForced = Projectile.Center;
                }
            }
        }

        public override void SafePostDrawAdditive(Color lightColor, SpriteBatch spriteBatch)
        {
            if (Projectile.velocity.X < 0.5f && TimeSpent > 60)
            {
                float scaleMult = (((float)Math.Cos((Projectile.timeLeft - 480) * 0.05f)) * 0.115f + 1.12f);
                Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(ProjectileTexture, drawPosition, null, GenshinElementUtils.GetColor(GenshinElement.PYRO) * 0.75f, Projectile.rotation, ProjectileTexture.Size() * 0.5f, Projectile.scale * scaleMult, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < OldPosition.Count; i++)
            {
                Vector2 drawPosition = Vector2.Transform(OldPosition[i] - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(ProjectileTexture, drawPosition, null, lightColor * 0.1f * i, Projectile.rotation, ProjectileTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            }
        }

        public override void SafeSecondPostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
            spriteBatch.Draw(ProjectileTexture, drawPosition, null, lightColor * 1.5f, Projectile.rotation, ProjectileTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
        }
    }
}
