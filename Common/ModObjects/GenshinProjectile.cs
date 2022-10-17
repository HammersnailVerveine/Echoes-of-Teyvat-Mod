using GenshinMod.Common.GameObjects;
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
        public GenshinElement Element = GenshinElement.NONE; // Projectile element
        public int ElementApplication = ElementApplicationWeak;
        public bool IgnoreICD = false;
        public bool CanReact = true;

        public bool FirstHit = false; // Has the projectile hit a target yet ?

        public virtual void SafeAI() { }
        public virtual void SafePostAI() { }
        public virtual void SafeOnHitNPC(NPC target, int damage, float knockback, bool crit) { }
        public virtual void OnFirstHitNPC(NPC target, int damage, float knockback, bool crit) { }
        public virtual bool SafePreDraw(SpriteBatch spriteBatch, Color lightcolor) => true;

        public static int ElementApplicationWeak => 570; // 9.5 sec
        public static int ElementApplicationMedium => 900; // 15 sec
        public static int ElementApplicationStrong => 1200; // 20 sec

        public int timeSpent = 0;
        public bool IsLocalOwner => Projectile.owner == Main.myPlayer;
        public Player Owner => Main.player[Projectile.owner];
        public GenshinCharacter OwnerCharacter => Projectile.GetGlobalProjectile<GenshinGlobalProjectile>().OwnerCharacter;
        public bool FirstFrame => timeSpent == 1;
        public Texture2D GetTexture() => ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public static bool CanHomeInto(NPC npc) => npc.active && !npc.dontTakeDamage && !npc.friendly && npc.lifeMax > 5;

        public Vector2 VelocityImmobile => Projectile.velocity * 0.0000001f; // Returns an almost immobile velocity, so projectiles spawned from this have the corrent kb direction

        // OVERRIDES

        public sealed override void AI()
        {
            timeSpent++;
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
            if (!FirstHit)
            {
                FirstHit = true;

                if (ElementalParticles > 0)
                {
                    SpawnElementalParticle(Element, 1f, ElementalParticles);
                }

                OnFirstHitNPC(target, damage, knockback, crit);
            }
            SafeOnHitNPC(target, damage, knockback, crit);
        }

        /*
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            InflictElement(target, Element, ElementApplication);
        }
        */

        // UTILITY

        public int SpawnProjectile(Vector2 position, Vector2 velocity, int type, int damage, float knockback, float ai0 = 0, float ai1 = 0)
        {
            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity, type, damage, knockback, Projectile.owner, ai0, ai1);
            Main.projectile[proj].GetGlobalProjectile<GenshinGlobalProjectile>().OwnerCharacter = OwnerCharacter;
            return proj;
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

        public void SpawnElementalParticle(GenshinElement element, float value, int number = 1)
        {
            int type = ModContent.ProjectileType<ProjectileElementalParticle>();
            for (int i = 0; i < number; i ++)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, type, 0, 0f, Projectile.owner, (float)element, value);
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

        public void PostAITrail()
        {
            for (int length = Projectile.oldPos.Length - 1; length > 0; length--)
            {
                Projectile.oldPos[length] = Projectile.oldPos[length - 1];
            }
            Projectile.oldPos[0] = Projectile.position;
        }
    }
}
