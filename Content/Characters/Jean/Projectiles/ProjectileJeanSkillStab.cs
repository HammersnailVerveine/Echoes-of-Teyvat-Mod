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
    public class ProjectileJeanSkillStab : GenshinProjectile
    {
        public Texture2D WeaponTexture;

        public List<Vector2> OldPosition;
        public List<int> HitNPC;
        public List<int> HitNPCPending;
        Vector2 pushDirection = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jean Slash");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 300;
            Projectile.scale = 1f;
            ProjectileTrail = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            PostDrawAdditive = true;
            ElementalParticles = 2;
            ElementalParticleBonusChance = 67;
            AttackWeight = AttackWeight.MEDIUM;
        }

        public override void OnSpawn(IEntitySource source)
        {
            GenshinPlayer ownerPlayer = Owner.GetModPlayer<GenshinPlayer>();
            WeaponTexture = ModContent.Request<Texture2D>(ownerPlayer.CharacterCurrent.Weapon.Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Projectile.width = (int)(WeaponTexture.Width * 1.4f * ownerPlayer.CharacterCurrent.WeaponSize);
            Projectile.height = (int)(WeaponTexture.Height * 1.4f * ownerPlayer.CharacterCurrent.WeaponSize);
            OldPosition = new List<Vector2>();
            HitNPC = new List<int>();
            HitNPCPending = new List<int>();
        }

        public override void SafeOnHitNPC(NPC target)
        {
            HitNPC.Add(target.whoAmI);

            if (Collision.TileCollision(target.position, Vector2.UnitY * 16f, target.width, target.height, false, false, (int)Owner.gravDir).Length() == 16f)
                HitNPCPending.Add(target.whoAmI); // NPC is not grounded, can take fall damage
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (HitNPC.Contains(target.whoAmI)) return false;
            return base.CanHitNPC(target);
        }

        public override void SafeAI()
        {
            // position & rotation
            Projectile.velocity *= 0.825f;
            Projectile.scale = OwnerCharacter.WeaponSize * 1.05f;

            if (FirstFrame)
            {
                pushDirection = Projectile.Center - Owner.Center;
                pushDirection.Normalize();
                Projectile.rotation = pushDirection.ToRotation() + MathHelper.ToRadians(45f);
            }

            // Afterimages
            if (TimeSpent < 30)
            {
                OldPosition.Add(Projectile.Center);
                if (OldPosition.Count > 8)
                {
                    OldPosition.RemoveAt(0);
                }

                SpawnDust<JeanDust>(Projectile.Center, Vector2.UnitY.RotatedBy(Projectile.rotation + MathHelper.ToRadians(45)), 1f, 1f, 10, 1, 3);
            }
            else if (OldPosition.Count > 1)
            {
                OldPosition.RemoveAt(0);
                Projectile.friendly = false;
            }

            if (TimeSpent > 40) PostDrawAdditive = false;

            // pushes enemies away

            for (int i = HitNPCPending.Count - 1; i >= 0; i--)
            {
                NPC npc = Main.npc[HitNPCPending[i]];
                if (IsValidTarget(npc, true) && npc.knockBackResist > 0f && TimeSpent < 30f)
                {
                    npc.velocity *= 0.5f;
                    npc.velocity += pushDirection * 5f * npc.knockBackResist;
                }

                if (Collision.TileCollision(npc.position, npc.velocity, npc.width, npc.height, false, false, (int)Owner.gravDir).Length() < npc.velocity.Length())
                {
                    if (OwnerCharacter is CharacterJean jean)
                    {
                        int targetDamage = npc.GetGlobalNPC<Common.GlobalObjets.GenshinGlobalNPC>().ApplyResistance((int)(jean.AbilitySkill.GetScaling2() * npc.knockBackResist), GenshinElement.NONE);
                        OwnerGenshinPlayer.TryApplyDamageToNPC(npc, targetDamage, 0f, -npc.direction, combatText: true);
                        HitNPCPending.RemoveAt(i);
                    }
                }
            }
        }

        public override void SafePostDraw(Color lightColor, SpriteBatch spriteBatch)
        {
            if (TimeSpent < 30)
            {
                Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition + new Vector2(0f, Owner.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                SpriteEffects effect = (Projectile.ai[1] < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                float rotation = Projectile.rotation + (effect == SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));
                spriteBatch.Draw(WeaponTexture, drawPosition, null, lightColor * 1.5f, rotation, WeaponTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
            }
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
                float rotation2 = Projectile.rotation + (effect == SpriteEffects.None ? 0f : MathHelper.ToRadians(90f));
                if (Element == GenshinElement.NONE)
                    spriteBatch.Draw(WeaponTexture, drawPosition2, null, lightColor * 0.075f * i, rotation2, WeaponTexture.Size() * 0.5f, Projectile.scale, effect, 0f);
                else
                    spriteBatch.Draw(WeaponTexture, drawPosition2, null, GenshinElementUtils.GetColor(Element) * 0.125f * i, rotation2, WeaponTexture.Size() * 0.5f, Projectile.scale * 1.15f, effect, 0f);
            }
        }
    }
}
