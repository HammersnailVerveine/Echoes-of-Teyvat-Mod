using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.ModObjects.Weapons;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Jean.Projectiles
{
    public class ProjectileJeanCharged : GenshinProjectile
    {
        public Texture2D WeaponTexture;
        public GenshinWeapon Weapon;
        public float acceleration = 0.8f;
        public float DistanceToCharacter = 0.25f;
        public float UpwardsStab = 0f;

        public List<Vector2> OldPosition;
        public List<float> OldRotation;
        public List<int> HitNPC;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sword Slash");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 180;
            Projectile.scale = 1f;
            ProjectileTrail = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            PostDrawAdditive = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

        public override void OnFirstFrame()
        {
            GenshinPlayer ownerPlayer = Owner.GetModPlayer<GenshinPlayer>();
            Weapon = ownerPlayer.CharacterCurrent.Weapon;
            WeaponTexture = ModContent.Request<Texture2D>(Weapon.Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Projectile.scale = ownerPlayer.CharacterCurrent.WeaponSize;
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
            HitNPC = new List<int>();

            if (IsLocalOwner)
            {
                Projectile.width = (int)(WeaponTexture.Width * 1.4f * ownerPlayer.CharacterCurrent.WeaponSize);
                Projectile.height = (int)(WeaponTexture.Height * 1.4f * ownerPlayer.CharacterCurrent.WeaponSize);
            }
        }

        public override void SafeAI()
        {
            Vector2 position = Owner.Center + (Vector2.UnitY * TileLength * 3.5f * Projectile.scale * DistanceToCharacter).RotatedBy(MathHelper.ToRadians(Projectile.ai[0])) - Projectile.Size * 0.5f;
            Projectile.position = position;

            Vector2 direction = Projectile.Center - Owner.Center;
            Projectile.rotation = direction.ToRotation() + MathHelper.ToRadians(45f);

            Projectile.position += Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(45)) * UpwardsStab;

            if (TimeSpent < 10)
            {
                DistanceToCharacter *= 1.125f;
                Projectile.ai[0] += Projectile.ai[1] * acceleration;
                if (TimeSpent < 4) acceleration *= 2.35f;
            }
            else if (TimeSpent < 18)
            {
                UpwardsStab += (10f - (TimeSpent - 9)) * 1.5f * Projectile.ai[1];
            } 

            AttackWeight = Element == GenshinElement.GEO ? AttackWeight.BLUNT : AttackWeight.MEDIUM;

            // Afterimages
            if (TimeSpent < 18)
            {
                OldPosition.Add(Projectile.Center);
                OldRotation.Add(Projectile.rotation);
                if (OldPosition.Count > 8)
                {
                    OldPosition.RemoveAt(0);
                    OldRotation.RemoveAt(0);
                }

                SpawnDust<JeanDust>(Projectile.Center, Vector2.UnitY.RotatedBy(Projectile.rotation + MathHelper.ToRadians(45)), 1f, 1f, 10);
            }
            else if (OldPosition.Count > 1 && TimeSpent % 2 == 0)
            {
                OldPosition.RemoveAt(0);
                OldRotation.RemoveAt(0);
            }

            if (TimeSpent > 30) PostDrawAdditive = false;

            // floats enemies upwards

            for (int i = HitNPC.Count - 1; i >= 0; i--)
            {
                NPC npc = Main.npc[HitNPC[i]];
                if (IsValidTarget(npc, true) && npc.knockBackResist > 0f)
                {
                    npc.velocity *= 0.25f;
                    if (npc.velocity.Y > 0f && !npc.noGravity)
                        npc.velocity.Y *= 0.1f;

                    if (Collision.TileCollision(npc.position, npc.velocity, npc.width, npc.height, false, false, (int)Owner.gravDir).Length() < npc.velocity.Length())
                        HitNPC.RemoveAt(i);

                    SpawnDust<JeanDust>(npc.Center, Vector2.Zero, 1f, 1f, npc.height, 1, 5);
                }
            }

        }

        public override void SafeOnHitNPC(NPC target)
        {
            HitNPC.Add(target.whoAmI);
            if (target.knockBackResist > 0f)
            {
                target.velocity *= 0.5f;
                target.velocity.Y -= 30f;
            }

            foreach (Projectile projectile in Main.projectile)
            { // removes NPC awaiting skill fall damage to avoid weird behaviours
                if (projectile.ModProjectile is ProjectileJeanSkillStab stab)
                {
                    for (int i = stab.HitNPCPending.Count - 1; i >= 0; i--)
                    {
                        if (stab.HitNPCPending[i] == target.whoAmI)
                            stab.HitNPCPending.RemoveAt(i);
                    }
                }
            }
        }


        public override bool? CanHitNPC(NPC target)
        {
            if (HitNPC.Contains(target.whoAmI)) return false;
            return base.CanHitNPC(target);
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            if (TimeSpent < 30)
            {
                Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                SpriteEffects effect = (Projectile.ai[1] < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                float rotation = Projectile.rotation + (effect != SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));
                spriteBatch.Draw(WeaponTexture, drawPosition, null, lightColor * 1.5f, rotation, WeaponTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
            }
        }

        public override void SafePostDrawAdditive(Color lightColor, SpriteBatch spriteBatch)
        {
            SpriteEffects effect = (Projectile.ai[1] < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation + (effect != SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));

            if (Element != GenshinElement.NONE)
            {
                Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(WeaponTexture, drawPosition, null, GenshinElementUtils.GetColor(Element) * 0.75f, rotation, WeaponTexture.Size() * 0.5f, Projectile.scale * 1.15f, effect, 0f);
            }

            for (int i = 0; i < OldPosition.Count; i++)
            {
                Vector2 drawPosition2 = Vector2.Transform(OldPosition[i] - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                float rotation2 = OldRotation[i] + (effect != SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));
                if (Element == GenshinElement.NONE)
                    spriteBatch.Draw(WeaponTexture, drawPosition2, null, lightColor * 0.075f * i, rotation2, WeaponTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
                else
                    spriteBatch.Draw(WeaponTexture, drawPosition2, null, GenshinElementUtils.GetColor(Element) * 0.125f * i, rotation2, WeaponTexture.Size() * 0.5f, Projectile.scale * 1.15f, effect, 0f);
            }
        }
    }
}
