﻿using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.ModObjects.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Common.GameObjects
{
    public abstract class GenshinCharacter
    {
        // Automatic variables
        public Player Player;
        public GenshinPlayer GenshinPlayer;
        public GenshinAbility AbilityCurrent;

        public Texture2D TextureHead;
        public Texture2D TextureBody;
        public Texture2D TextureLegs;
        public Texture2D TextureArms;
        public Texture2D TextureAbilitySkill;
        public Texture2D TextureAbilityBurst;
        public Texture2D TextureIcon;

        // Default variables

        public GenshinAbility AbilityNormal;
        public GenshinAbility AbilityCharged;
        public GenshinAbility AbilitySkill;
        public GenshinAbility AbilityBurst;

        public string Name;
        public GenshinElement Element;

        public int BaseHealth = 100; // Max health no modifiers
        public int BaseDefense = 100; // Max defense no modifiers
        public int BaseAttack = 100; // Max attack no modifiers
        public WeaponType WeaponType; // Character Weapon Type
        public bool Autoswing = false; // NA autoswing

        // Equipment Variables

        public GenshinWeapon Weapon;

        // Levels (1 - 10)

        public int Level = 1; // Character level
        public int LevelAbilityNormal = 1;
        public int LevelAbilitySkill = 1;
        public int LevelAbilityBurst = 1;

        // Dynamic variables

        public int Health = 100; // Current health
        public float Energy = 0f; // Current energy
        public int TimerCanUse = 0;
        public int TimerVanityWeapon = 0; // Vanity (floating) weapon can only appear if <= 0

        // Reset variables

        public float StatEnergyRecharge = 1f; // Energy Recharge % (base = 100%)
        public float StatAttack = 0f; // Bonus Attack% (base = 0%)
        public float StatHealth = 0f; // Health bonus % (of FlatHealth, base = 0%)
        public float StatDefense = 0f; // Defense bonus % (of FlatDefense, base = 0%)
        public float StatElementalMastery = 0f; // Elemental mastery (base = 0)
        public int StatAttackFlat = 0; // Bonus flat damage (base = 0)
        public int StatHealthFlat = 0; // Bonus flat health (base = 0)
        public int StatDefenseFlat = 0; // Bonus flat defense (base = 0)

        public float StatDamage = 0f; // Bonus Damage% (base = 0%)
        public float StatDamagePhysical = 0f; // Bonus Physical Damage% (base = 0%)
        public float StatDamageGeo = 0f; // Bonus Geo Damage% (base = 0%)
        public float StatDamageAnemo = 0f; // Bonus Anemo Damage% (base = 0%)
        public float StatDamageCryo = 0f; // Bonus Cryo Damage% (base = 0%)
        public float StatDamageElectro = 0f; // Bonus Electro Damage% (base = 0%)
        public float StatDamageDendro = 0f; // Bonus Dencro Damage% (base = 0%)
        public float StatDamageHydro = 0f; // Bonus Hydro Damage% (base = 0%)
        public float StatDamagePyro = 0f; // Bonus Pyro Damage% (base = 0%)
        public float StatDamageNA = 0f; // Bonus NA Damage% (base = 0%)
        public float StatDamageCA = 0f; // Bonus CA Damage% (base = 0%)
        public float StatDamageSkill = 0f; // Bonus Skill Damage% (base = 0%)
        public float StatDamageBurst = 0f; // Bonus Burst Damage% (base = 0%)

        public List<ICDTracker> ICDTrackers;

        public int EffectiveHealth => (int)(BaseHealth * (1f + StatHealth)) + StatHealthFlat;
        public float EffectiveDefense => (float)(((float)BaseDefense * (1f + StatDefense)) + StatDefenseFlat);
        public float EffectiveAttack => (float)(((float)(BaseAttack + Weapon.EffectiveAttack) * (1f + StatAttack)) + StatAttackFlat);

        public bool IsAlive => Health > 0; 

        public abstract void SetDefaults();
        public virtual void SafeUpdate() { }
        public virtual void SafePreUpdate() { }
        public virtual void SafePostUpdate() { }
        public virtual void SafeResetEffects() { }
        public virtual void OnSwapIn() { }
        public virtual bool OnSwapOut() => true; // Return false to prevent swap out
        public virtual bool OnHeal(int value) => true; // Return false to prevent heal
        public virtual bool OnDamage(int value) => true; // Return false to prevent damage

        public GenshinCharacter Initialize(GenshinPlayer modPlayer)
        {
            string className = GetType().Name;
            className = className.Remove(0, 9);
            string location = "GenshinMod/Content/Characters/" + className + "/Textures/" + className;
            TextureHead ??= ModContent.Request<Texture2D>(location + "_Head", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureBody ??= ModContent.Request<Texture2D>(location + "_Body", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureLegs ??= ModContent.Request<Texture2D>(location + "_Legs", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureArms ??= ModContent.Request<Texture2D>(location + "_Arms", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureIcon ??= ModContent.Request<Texture2D>(location + "_Icon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureAbilitySkill ??= ModContent.Request<Texture2D>(location + "_Ability_Skill", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureAbilityBurst ??= ModContent.Request<Texture2D>(location + "_Ability_Burst", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            GenshinPlayer = modPlayer;
            Player = modPlayer.Player;
            ICDTrackers = new List<ICDTracker>();
            
            SetDefaults();

            if (Weapon == null) TryEquipWeapon(GenshinWeapon.GetWeakestWeapon(WeaponType));
            Health = EffectiveHealth;
            return this;
        }

        public void PreUpdate()
        {
            SafePreUpdate();
            Weapon.WeaponUpdate();
        }

        public void Update() // Use to setup character stats. No active effects
        {
            SafePreUpdate();
            Weapon.WeaponUpdate();

            if (IsCurrentCharacter)
            {
                Weapon.WeaponUpdateActive();
            }
        }

        public void PostUpdate() // Active effects here. Stats are setup every frame in PreUpdate
        {
            SafePostUpdate();
            Weapon.WeaponPostUpdate();

            if (AbilityCurrent != null)
            {
                AbilityCurrent.OnUseUpdate();
                AbilityCurrent.UseTimeCurrent--;
                if (AbilityCurrent.UseTimeCurrent <= 0)
                {
                    AbilityCurrent.OnUseEnd();
                    AbilityCurrent = null;
                }
            }

            else if (IsCurrentCharacter)
            {
                Weapon.WeaponPostUpdateActive();
                if (TimerVanityWeapon <= 0) Weapon.SpawnVanityWeapon();

                if (Main.mouseLeft && (Main.mouseLeftRelease || Autoswing) && !GenshinPlayer.IsUsing())
                {
                    TryUseAbility(AbilityNormal);
                }
                if (Main.mouseRight && (Main.mouseRightRelease || Autoswing) && !GenshinPlayer.IsUsing())
                {
                    TryUseAbility(AbilityCharged);
                }
            }
        }

        public void ResetEffects()
        {
            SafeResetEffects();
            AbilityNormal.ResetEffects();
            AbilityCharged.ResetEffects();
            AbilitySkill.ResetEffects();
            AbilityBurst.ResetEffects();
            Weapon.WeaponResetEffects();
            TimerCanUse --;
            TimerVanityWeapon--;

            StatEnergyRecharge = 1f;
            StatAttack = 0f;
            StatHealth = 0f;
            StatDefense = 0f;
            StatElementalMastery = 0f;
            StatAttackFlat = 0;
            StatHealthFlat = 0;
            StatDefenseFlat = 0;
            StatDamage = 0f;
            StatDamagePhysical = 0f;
            StatDamageGeo = 0f;
            StatDamageAnemo = 0f;
            StatDamageCryo = 0f;
            StatDamageElectro = 0f;
            StatDamageDendro = 0f;
            StatDamageHydro = 0f;
            StatDamagePyro = 0f;
            StatDamageNA = 0f;
            StatDamageCA = 0f;
            StatDamageSkill = 0f;
            StatDamageBurst = 0f;

            for (int i = ICDTrackers.Count - 1; i >= 0; i--)
            {
                ICDTrackers[i].Timer--;
                if (ICDTrackers[i].Timer <= 0) ICDTrackers.RemoveAt(i);
            }
        }

        public void TryUseAbility(GenshinAbility ability)
        {
            if (!ability.IsUsed() && ability.CanUse() && CanUseAbility && Energy >= ability.Energy && GenshinPlayer.TryUseStamina(ability.Stamina))
            {
                Energy -= ability.Energy;
                AbilityCurrent = ability;
                ability.Use();
            }
        }

        public void GainEnergy(GenshinElement element, float value)
        {
            if (element == GenshinElement.NONE) value *= 2;
            else if (element == Element) value *= 3;

            GainEnergyFlat(value);
        }

        public void GainEnergyFlat(float value)
        {
            Energy += value * StatEnergyRecharge;
            if (Energy > AbilityBurst.Energy) Energy = AbilityBurst.Energy;
        }

        public bool TryApplyElement(NPC npc)
        {
            foreach (ICDTracker tracker in ICDTrackers)
            {
                if (tracker.npc == npc)
                {
                    return tracker.TryApplyElement();
                }
            }

            ICDTrackers.Add(new ICDTracker(npc));
            return true;
        }

        public void OnSwapInGlobal()
        {
            OnSwapIn();
            Weapon.WeaponOnSwapIn();
            TimerCanUse = 10;

            for (int i = 0; i < 10; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(Player.position, Player.width, Player.height, DustID.YellowTorch, 0f, 0f)];
                dust.noGravity = true;
                dust.scale *= 2f;
            }
        }

        public bool OnSwapOutGlobal()
        {
            if (OnSwapOut())
            {
                Weapon.WeaponOnSwapOut();
                Weapon.KillProjectile();
                return true;
            }
            return false;
        }

        public void Heal(int value, bool combatText = true)
        {
            if (OnHeal(value))
            {
                if (value > EffectiveHealth - Health) value = EffectiveHealth - Health;
                if (value > 0)
                {
                    Health += value;
                    if (Health > EffectiveHealth) Health = EffectiveHealth;
                    if (IsCurrentCharacter && combatText) CombatText.NewText(Player.Hitbox, new Color(188, 255, 55), value);
                }
            }
        }

        public void Damage(int value, bool crit = false, bool combatText = true)
        {
            if (OnDamage(value))
            {
                value = ApplyDefense(value);
                Health -= value;
                if (Health < 0) Health = 0;
                if (IsCurrentCharacter && combatText) CombatText.NewText(Player.Hitbox, new Color(255, 80, 80), value, crit);
            }
        }

        public int GetAbilityLevel(GenshinAbility ability)
        {
            if (ability == AbilityNormal || ability == AbilityCharged) return LevelAbilityNormal;
            if (ability == AbilitySkill) return LevelAbilitySkill;
            if (ability == AbilityBurst) return LevelAbilityBurst;
            return 1;
        }

        public int ApplyDefense(int value, int sourceLevel = 10) => (int)(value * (1f - (EffectiveDefense / (EffectiveDefense + 500f + sourceLevel * 50f))));

        public bool CanUseAbility => AbilityCurrent == null && TimerCanUse <= 0;
        public bool IsCurrentCharacter => GenshinPlayer.CharacterCurrent == this;

        public void TryEquipWeapon(GenshinWeapon weapon)
        {
            if (weapon.WeaponType == WeaponType) ForceEquipWeapon(weapon);
        }

        public void RemoveVanityWeapon(int time)
        {
            TimerVanityWeapon = time;
            Weapon.KillProjectile();
        }

        public void ForceEquipWeapon(GenshinWeapon weapon) => weapon.Equip(this);

        public int ApplyDamageMult(int damage, GenshinElement element, AbilityType abilityType)
        {
            float mult = 1f;

            mult += StatDamage;

            switch (element)
            {
                case GenshinElement.NONE:
                    mult += StatDamagePhysical;
                    break;
                case GenshinElement.GEO:
                    mult += StatDamageGeo;
                    break;
                case GenshinElement.ANEMO:
                    mult += StatDamageAnemo;
                    break;
                case GenshinElement.CRYO:
                    mult += StatDamageCryo;
                    break;
                case GenshinElement.ELECTRO:
                    mult += StatDamageElectro;
                    break;
                case GenshinElement.DENDRO:
                    mult += StatDamageDendro;
                    break;
                case GenshinElement.HYDRO:
                    mult += StatDamageHydro;
                    break;
                case GenshinElement.PYRO:
                    mult += StatDamagePyro;
                    break;
                default:
                    break;
            }

            switch(abilityType)
            {
                case AbilityType.NORMAL:
                    mult += StatDamageNA;
                    break;
                case AbilityType.CHARGED:
                    mult += StatDamageCA;
                    break;
                case AbilityType.SKILL:
                    mult += StatDamageSkill;
                    break;
                case AbilityType.BURST:
                    mult += StatDamageBurst;
                    break;
                default:
                    break;
            }

            damage = (int)(damage * mult);

            return damage;
        }
    }

    public class ICDTracker
    {
        public NPC npc;
        public int Timer;
        public int HitCount;

        public ICDTracker(NPC npc) {
            this.npc = npc;
            Timer = 150;
            HitCount = 0;
        }

        public bool TryApplyElement()
        {
            HitCount++;
            if (HitCount >= 3)
            {
                HitCount = 0;
                return true;
            }

            return false;
        }
    }
}
