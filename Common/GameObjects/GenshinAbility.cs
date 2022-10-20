using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Common.GameObjects
{
    public abstract class GenshinAbility
    {
        public string Name; // Ability Name
        //public int Damage = 0; // Ability base Damage
        public int UseTime = 1; // Ability base UseTime
        public float KnockBack = 0f; // Ability base Knockback
        public float Velocity = 0f; // Ability base Projectile Veloctiy
        public float Stamina = 0; // Ability base Stamina Cost
        public int Cooldown = 0; // Ability base Cooldown
        public int Energy = 0; // Ability Particle generation (nb of particles)
        public int ChargesMax = 1; // Ability Maximum number of charges

        public int UseTimeCurrent = 0; // Current usetime (>0 = current being used)
        public int CooldownCurrent = 0; // Current Cooldown (0 = ready)
        public int ChargesCurrent = 0; // Current number of available charges
        public GenshinCharacter Character; // Owner character (setup in Initialize())

        public AbilityType AbilityType = AbilityType.NONE; // Bonus damage multiplier depending on ability type

        public static float AlmostImmobile = 0.001f;
        public static float Immobile = 0f;

        public abstract void SetDefaults();
        public abstract void OnUse();
        public virtual void OnUseUpdate() { }
        public virtual void OnUseEnd() { }
        public virtual void SafeResetEffects() { }
        public int Level => Character.GetAbilityLevel(this);
        public virtual bool CanUse() => ChargesCurrent > 0;
        public virtual IEntitySource GetSource() => Player.GetSource_Misc("GenshinMod Attack");
        public bool IsUsed() => UseTimeCurrent > 0;
        public GenshinElement Element => Character.Element;
        public Player Player => Character.Player;

        public GenshinAbility Initialize(GenshinCharacter character)
        {
            Character = character;
            SetDefaults();
            ChargesCurrent = ChargesMax;
            return this;
        }

        public void ResetEffects()
        {
            if (CooldownCurrent > 0) CooldownCurrent--;
            if (CooldownCurrent <= 0 && ChargesCurrent < ChargesMax)
            {
                ChargesCurrent++;
                if (ChargesCurrent < ChargesMax) CooldownCurrent = Cooldown;
            }

            SafeResetEffects();
        }

        public void Use()
        {
            ChargesCurrent--;
            UseTimeCurrent = UseTime;
            Character.GenshinPlayer.TimerUse = UseTime;
            Character.GenshinPlayer.TimerUseRef = UseTime;
            Character.GenshinPlayer.LastUseDirection = Main.MouseWorld.X - Player.Center.X > 0 ? 1 : -1;
            if (CooldownCurrent <= 0) CooldownCurrent = Cooldown;
            OnUse();
        }

        public int SpawnProjectileSpecific(IEntitySource source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, int owner, GenshinElement element, AbilityType damageType, float ai0 = 0, float ai1 = 0)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, owner, ai0, ai1);
            Projectile projectile = Main.projectile[proj];
            projectile.GetGlobalProjectile<GenshinGlobalProjectile>().OwnerCharacter = this.Character;
            if (projectile.ModProjectile is GenshinProjectile genshinProjectile)
            {
                genshinProjectile.Element = element;
                genshinProjectile.AbilityType = damageType;
            }
            return proj;
        }

        public int SpawnProjectileSpecific(Vector2 velocity, int type, GenshinElement element, AbilityType damageType, float ai0 = 0, float ai1 = 0)
        {
            return SpawnProjectileSpecific(GetSource(), Character.Player.Center, velocity, type, GetScaling(), KnockBack, Character.Player.whoAmI, element, damageType, ai0, ai1);
        }

        public int SpawnProjectileSpecific(Vector2 position, Vector2 velocity, int type, GenshinElement element, AbilityType damageType, float ai0 = 0, float ai1 = 0)
        {
            return SpawnProjectileSpecific(GetSource(), position, velocity, type, GetScaling(), KnockBack, Character.Player.whoAmI, element, damageType, ai0, ai1);
        }

        public int SpawnProjectile(Vector2 position, Vector2 velocity, int type, float ai0 = 0, float ai1 = 0)
        {
            return SpawnProjectileSpecific(GetSource(), position, velocity, type, GetScaling(), KnockBack, Character.Player.whoAmI, Element, AbilityType, ai0, ai1);
        }

        public int SpawnProjectile(Vector2 velocity, int type, float ai0 = 0, float ai1 = 0)
        {
            return SpawnProjectileSpecific(GetSource(), Character.Player.Center, velocity, type, GetScaling(), KnockBack, Character.Player.whoAmI, Element, AbilityType, ai0, ai1);
        }

        public Vector2 VelocityToCursor() => VelocityToTarget(Main.MouseWorld);

        public Vector2 VelocityToTarget(Vector2 target)
        {
            Vector2 velocity = target - Player.Center;
            velocity.Normalize();
            velocity *= Velocity;
            return velocity;
        }

        public abstract int GetScaling();
        public virtual int GetScaling2() => 1;
        public virtual int GetScaling3() => 1;
        public virtual int GetScaling4() => 1;
    }
    public enum AbilityType : int
    {
        NONE = 0,
        NORMAL = 1,
        CHARGED = 2,
        SKILL = 3,
        BURST = 4
    }
}
