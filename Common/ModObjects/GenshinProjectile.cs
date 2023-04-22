using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.GlobalObjets;
using GenshinMod.Content.Projectiles;
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
        public bool ProjectileTrail = false; // Will the projectile leave a trail of afterimages ?
        public float ProjectileTrailOffset = 0f; // Offcenters the afterimages a bit. useless without projectileTrail activated. Looks terrible on most projectiles.
        public int ElementalParticles = 0; // Number of particles (value : 1) spawned on first hit
        public int ElementalParticleChance = 100; // % chance of ElementalParticles spawning on first hit
        public GenshinElement Element = GenshinElement.NONE; // Projectile element
        public AbilityType AbilityType = AbilityType.NONE; // Bonus damage type for multipliers
        public int ElementApplication = ElementApplicationWeak; // Elemental application duration
        public bool IgnoreICD = false; // Will the projectile always apply its element no matter what?
        public bool CanReact = true; // Can the projectile trigger elemental reactions?
        public bool CanDealDamage = true; // Can the projectile deal damage?
        public bool FirstFrameDamage; // The projectile resets immunity and deals damage on its first frame only
        public GenshinCharacter OwnerCharacter; // Reference to the owner character
        public bool FirstHit = false; // Has the projectile hit a target yet ?
        public int TimeSpent = 0; // Time the projectile has spent alive
        public float DefenseIgnore = 0f; // % of enemy defense ignored.
        public bool PostDrawAdditive = false; // Should the SafePostDrawAdditive() methos be called?
        public AttackWeight AttackWeight = AttackWeight.MEDIUM;

        public virtual void SafeAI() { }
        public virtual void SafePostAI() { }
        public virtual void SafeOnHitNPC(NPC target, int damage, float knockback, bool crit) { }
        public virtual void OnFirstHitNPC(NPC target, int damage, float knockback, bool crit) { }
        public virtual void SafeModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) { }
        public virtual bool SafePreDraw(SpriteBatch spriteBatch, Color lightcolor) => true;
        public virtual void SafePostDraw(Color lightColor, SpriteBatch spriteBatch) { } // Replaces the normal PostDraw
        public virtual void SafePostDrawAdditive(Color lightColor, SpriteBatch spriteBatch) { } // Called after SafePostDraw, the spritebatch in parameter uses additive blending
        public virtual void SafeSecondPostDraw(Color lightColor, SpriteBatch spriteBatch) { } // Called after SafePostDrawAdditive so a normal blend can be drawn above the additive one
        public bool IsLocalOwner => Projectile.owner == Main.myPlayer;
        public Player Owner => Main.player[Projectile.owner];
        public GenshinPlayer OwnerGenshinPlayer => Owner.GetModPlayer<GenshinPlayer>();
        public bool FirstFrame => TimeSpent == 1;
        public Texture2D GetTexture() => ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public Texture2D GetTexture(String loc) => ModContent.Request<Texture2D>(loc, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public Texture2D GetWeaponTexture() => ModContent.Request<Texture2D>(OwnerGenshinPlayer.CharacterCurrent.Weapon.Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public static bool CanHomeInto(NPC npc) => npc.active && !npc.dontTakeDamage && !npc.friendly && npc.lifeMax > 5;
        public static int ElementApplicationWeak => 570; // 9.5 sec
        public static int ElementApplicationMedium => 900; // 15 sec
        public static int ElementApplicationStrong => 1200; // 20 sec
        public static float SwordRotation => MathHelper.ToRadians(45f);
        public static float TileLength => 16f;

        public Vector2 PlayerCenterRelative => Owner.Center - Projectile.Size * 0.5f;

        public Vector2 VelocityImmobile => Projectile.velocity * 0.0000001f; // Returns an almost immobile velocity, so projectiles spawned from this have the corrent kb direction

        // OVERRIDES

        public sealed override void AI()
        {
            TimeSpent++;
            if (FirstFrameDamage)
            {
                if (FirstFrame)
                {
                    Projectile.friendly = true;
                    ResetImmunity();
                }
                else
                    Projectile.friendly = false;
            }
            SafeAI();
        }

        public sealed override void PostAI()
        {
            SafePostAI();
            if (ProjectileTrail)
            {
                PostAITrail();
            }
        }

        public sealed override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!FirstHit && damage > 0)
            {
                FirstHit = true;

                if (ElementalParticles > 0 && Main.rand.Next(100) < ElementalParticleChance && target.GetGlobalNPC<GenshinGlobalNPC>().GiveEnergyParticlesHit)
                {
                    SpawnElementalParticle(Element, 1f, ElementalParticles);
                }

                OnFirstHitNPC(target, damage, knockback, crit);
            }
            SafeOnHitNPC(target, damage, knockback, crit);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!CanDealDamage)
            {
                damage = 0;
                target.life++;
            }
            SafeModifyHitNPC(target, ref damage, ref knockback, ref crit, ref hitDirection);
        }

        /*
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            InflictElement(target, Element, ElementApplication);
        }
        */

        // UTILITY

        public static int SpawnProjectile(GenshinProjectile projectile, Vector2 position, Vector2 velocity, int type, int damage, float knockback, GenshinElement element, AbilityType damageType, float ai0 = 0, float ai1 = 0)
        {
            return projectile.SpawnProjectile(position, velocity, type, damage, knockback, element, damageType, ai0, ai1);
        }

        public int SpawnProjectile(Vector2 position, Vector2 velocity, int type, int damage, float knockback, GenshinElement element, AbilityType damageType, float ai0 = 0, float ai1 = 0)
        {
            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity, type, damage, knockback, Projectile.owner, ai0, ai1);
            Projectile projectile = Main.projectile[proj];
            if (projectile.ModProjectile is GenshinProjectile genshinProjectile) {
                genshinProjectile.OwnerCharacter = OwnerCharacter;
                genshinProjectile.Element = element;
                genshinProjectile.AbilityType = damageType;
            }
            return proj;
        }

        public int SpawnProjectile(Vector2 position, Vector2 velocity, int type, int damage, float knockback, float ai0 = 0, float ai1 = 0)
        {
            return SpawnProjectile(position, velocity, type, damage, knockback, Element, AbilityType, ai0, ai1);
        }

        public void SpawnDust<T>(float velocity = 0f, float scale = 1f, int offSet = 10, int quantity = 1, int chanceDenominator = 1) where T : ModDust => SpawnDust(ModContent.DustType<T>(), velocity, scale, offSet, quantity, chanceDenominator);

        public void SpawnDust(int type, float velocity = 0f, float scale = 1f, int offSet = 10, int quantity = 1, int chanceDenominator = 1)
        {
            if (!Main.rand.NextBool(chanceDenominator)) return;
            for (int i = 0; i < quantity; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.position - new Vector2(offSet, offSet), Projectile.width + offSet * 2, Projectile.height + offSet * 2, type)];
                dust.velocity = new Vector2(Main.rand.NextFloat(-velocity, velocity), Main.rand.NextFloat(-velocity, velocity));
                dust.scale = scale;
            }
        }

        public static void SpawnDust<T>(Vector2 position, Vector2 direction, float velocity = 0f, float scale = 1f, int offSet = 10, int quantity = 1, int chanceDenominator = 1) where T : ModDust => SpawnDust(ModContent.DustType<T>(), position, direction, velocity, scale, offSet, quantity, chanceDenominator);

        public static void SpawnDust(int type, Vector2 position, Vector2 direction, float velocity = 0f, float scale = 1f, int offSet = 10, int quantity = 1, int chanceDenominator = 1)
        {
            if (!Main.rand.NextBool(chanceDenominator)) return;
            for (int i = 0; i < quantity; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(position - new Vector2(offSet, offSet), offSet * 2, offSet * 2, type)];
                dust.velocity = new Vector2(Main.rand.NextFloat(-velocity, velocity), Main.rand.NextFloat(-velocity, velocity)) + direction;
                dust.scale = scale;
            }
        }

        public void SpawnElementalParticle(GenshinElement element, float value, int number = 1)
        {
            int type = ModContent.ProjectileType<ProjectileElementalParticle>();
            for (int i = 0; i < number; i ++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, type, 0, 0f, Projectile.owner, (float)element, value);
            }
        }

        /*
        public void InflictElement(NPC npc, GenshinElement element, int duration)
        {
            if (IgnoreICD) npc.GetGlobalNPC<GenshinGlobalNPC>().InflictElement(element, duration);
            else if (OwnerCharacter.TryApplyElement(npc)) npc.GetGlobalNPC<GenshinGlobalNPC>().InflictElement(element, duration);
        }
        */

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

        // DRAW

        public sealed override bool PreDraw(ref Color lightColor)
        {
            if (ProjectileTrail)
            {
                PreDrawTrail(Main.spriteBatch, lightColor);
            }
            return SafePreDraw(Main.spriteBatch, lightColor);
        }

        public void PreDrawTrail(SpriteBatch spriteBatch, Color lightColor)
        {
            float offSet = ProjectileTrailOffset + 0.5f;
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * offSet, Projectile.height * offSet);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0.3f);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            SafePostDraw(lightColor, spriteBatch);
            if (PostDrawAdditive)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

                SafePostDrawAdditive(lightColor, spriteBatch);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
            SafeSecondPostDraw(lightColor, spriteBatch);
        }

        public void PostAITrail()
        {
            for (int length = Projectile.oldPos.Length - 1; length > 0; length--)
            {
                Projectile.oldPos[length] = Projectile.oldPos[length - 1];
            }
            Projectile.oldPos[0] = Projectile.position;
        }

        public void FirstFrameHit()
        {
            if (FirstFrame)
            {
                ResetImmunity();
                Projectile.friendly = true;
            }
            else Projectile.friendly = false;
        }
    }
}
