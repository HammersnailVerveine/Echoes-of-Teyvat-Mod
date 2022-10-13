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
        public int Damage = 0; // Ability base Damage
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

        public abstract void SetDefaults();
        public abstract void OnUse();
        public virtual void OnUseUpdate() { }
        public virtual void OnUseEnd() { }
        public virtual void SafeResetEffects() { }

        public virtual bool CanUse() => ChargesCurrent > 0;

        public virtual IEntitySource GetSource() => Player.GetSource_Misc("GenshinMod Attack");

        public bool IsUsed() => UseTimeCurrent > 0;
        public CharacterElement Element => Character.Element;
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
            CooldownCurrent = Cooldown;
            Character.GenshinPlayer.TimerUse = UseTime;
            Character.GenshinPlayer.TimerUseRef = UseTime;
            Character.GenshinPlayer.LastUseDirection = Main.MouseWorld.X - Player.Center.X > 0 ? 1 : -1;
            OnUse();
        }

        public int SpawnProjectile(IEntitySource source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, int owner, float ai0 = 0, float ai1 = 0)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, owner, ai0, ai1);
            return proj;
        }

        public int SpawnProjectile(Vector2 velocity, int type, float ai0 = 0, float ai1 = 0)
        {
            return SpawnProjectile(GetSource(), Character.Player.Center, velocity, type, Damage, KnockBack, Character.Player.whoAmI, ai0, ai1);
        }

        public int SpawnProjectile(Vector2 position, Vector2 velocity, int type, float ai0 = 0, float ai1 = 0)
        {
            return SpawnProjectile(GetSource(), position, velocity, type, Damage, KnockBack, Character.Player.whoAmI, ai0, ai1);
        }

        public Vector2 VelocityToCursor() => VelocityToTarget(Main.MouseWorld);

        public Vector2 VelocityToTarget(Vector2 target)
        {
            Vector2 velocity = target - Player.Center;
            velocity.Normalize();
            velocity *= Velocity;
            return velocity;
        }
    }
}
