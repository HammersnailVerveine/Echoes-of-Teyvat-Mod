using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.ModObjects.Weapons;
using GenshinMod.Content.Dusts;
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
        public GenshinAbility AbilityHeld;

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

        public float BaseHealthMax = 1000f; // Max health no modifiers
        public float BaseDefenseMax = 1000f; // Max defense no modifiers
        public float BaseAttackMax = 1000f; // Max attack no modifiers
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
        public int TimerWeaponInfusion = 0;
        public GenshinElement WeaponInfusion = GenshinElement.NONE;
        public bool WeaponInfusionOverridable = true;

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
        public float StatHealingBonus = 0f; // Bonus Healing Done% (base = 0%)
        public float StatHealingReceived = 0f; // Bonus Healind Received% (base = 0%)
        public float StatShieldStrength = 0f; // Bonus Shield Strength% (base = 0%)


        public float StatDamageReaction = 0f; // Bonus Reaction Damage (base = 0%)
        public float StatDamageReactionVaporize = 0f; // Bonus Vaporize Reaction Damage (base = 0%)
        public float StatDamageReactionOverloaded = 0f; // Bonus Overloaded Reaction Damage (base = 0%)
        public float StatDamageReactionMelt = 0f; // Bonus Melt Reaction Damage (base = 0%)
        public float StatDamageReactionElectrocharged = 0f; // Bonus Electrocharged Reaction Damage (base = 0%)
        public float StatDamageReactionSuperconduct = 0f; // Bonus Superconduct Reaction Damage (base = 0%)
        public float StatDamageReactionSwirl = 0f; // Bonus Swirl Reaction Damage (base = 0%)
        public float StatDamageReactionBurning = 0f; // Bonus Burning Reaction Damage (base = 0%)
        public float StatDamageReactionBloom = 0f; // Bonus Bloom Reaction Damage (base = 0%)
        public float StatDamageReactionHyperbloom = 0f; // Bonus Hyperbloom Reaction Damage (base = 0%)
        public float StatDamageReactionBurgeon = 0f; // Bonus Burgeon Reaction Damage (base = 0%)
        public float StatDamageReactionQuicken = 0f; // Bonus Quicken Reaction Damage (base = 0%)
        public float StatDamageReactionAggravate = 0f; // Bonus Aggravate Reaction Damage (base = 0%)
        public float StatDamageReactionSpread = 0f; // Bonus Spread Reaction Damage (base = 0%)
        public float StatDamageReactionShatter = 0f; // Bonus Shatter Reaction Damage (base = 0%)
        public float StatDamageReactionFrozen = 0f; // Bonus Frozen Duration (base = 0%)
        public float StatDamageReactionCrystallize = 0f; // Bonus Crystallize Reaction Shield Value (base = 0%)

        public float WeaponSize = 1f;

        public List<ICDTracker> ICDTrackers;

        public float BaseHealth => (BaseHealthMax / 10f) * Level;
        public float BaseDefense => (BaseDefenseMax / 10f) * Level;
        public float BaseAttack => (BaseAttackMax / 10f) * Level;
        public int EffectiveHealth => (int)(BaseHealth * (1f + StatHealth)) + StatHealthFlat;
        public float EffectiveDefense => (float)(((float)BaseDefense * (1f + StatDefense)) + StatDefenseFlat);
        public float EffectiveAttack => (float)(((float)(BaseAttack + Weapon.EffectiveAttack) * (1f + StatAttack)) + StatAttackFlat);
        public float EffectiveHealing => (float)(1f + StatHealingBonus);
        public float ReactionTransformativeDamage => 16.05f * Level * 10f;

        public bool IsAlive => Health > 0; 

        public abstract void SetDefaults();
        public virtual void SafeUpdate() { }
        public virtual void SafePreUpdate() { }
        public virtual void SafePostUpdate() { }
        public virtual void SafeResetEffects() { }
        public virtual void DrawEffects(SpriteBatch spriteBatch, Color lightColor) { }
        public virtual void OnSwapIn() { }
        public virtual bool OnSwapOut() => true; // Return false to prevent swap out
        public virtual bool OnHeal(int value) => true; // Return false to prevent heal
        public virtual bool OnDamage(int value) => true; // Return false to prevent damage
        public virtual bool OnDeath() => true; // Return false to prevent death
        public virtual bool OnPartyAdd(); // Called when the character is added to the party
        public virtual bool OnPartyRemove(); // Called when the character is removed from the party
		
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

                if (!Main.playerInventory && IsAlive) // cannot use abilities with inventory open or while dead
                {
                    if (Main.mouseLeft && !GenshinPlayer.IsUsing) // Try use NA if LMB used
                    {
                        if (AbilityNormal.HoldTimeMax > 0)
                            TryHoldAbility(AbilityNormal, Main.mouseLeftRelease);
                        else if (Main.mouseLeftRelease || Autoswing)
                            TryUseAbility(AbilityNormal);
                    }

                    if (Main.mouseRight && !GenshinPlayer.IsUsing) // Try use CA if LMB used
                    {
                        if (AbilityCharged.HoldTimeMax > 0)
                            TryHoldAbility(AbilityCharged, Main.mouseRightRelease);
                        else if (Main.mouseRightRelease || Autoswing)
                            TryUseAbility(AbilityCharged);
                    }

                    if (GenshinPlayer.KeySkill && !GenshinPlayer.IsUsing) // Try use Skill if Skill key used
                    {
                        if (AbilitySkill.HoldTimeMax > 0)
                            TryHoldAbility(AbilitySkill, GenshinPlayer.KeySkillRelease);
                        else if (GenshinPlayer.KeySkillRelease)
                            TryUseAbility(AbilitySkill);
                    }

                    if (GenshinPlayer.KeyBurst && !GenshinPlayer.IsUsing) // Try use Burst if Burst key used
                    {
                        if (AbilityBurst.HoldTimeMax > 0)
                            TryHoldAbility(AbilityBurst, GenshinPlayer.KeyBurstRelease);
                        else if (GenshinPlayer.KeyBurstRelease)
                            TryUseAbility(AbilityBurst);
                    }
                }
            }

            if (IsHoldingAbility)
            {
                if ((AbilityHeld == AbilityNormal && !Main.mouseLeft)
                    || (AbilityHeld == AbilityCharged && !Main.mouseRight)
                    || (AbilityHeld == AbilitySkill && !GenshinPlayer.KeySkill)
                    || (AbilityHeld == AbilityBurst && !GenshinPlayer.KeyBurst))
                    StopHoldAbility();
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
            TimerWeaponInfusion--;

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
            StatHealingBonus = 0f;
            StatHealingReceived = 0f;
            StatShieldStrength = 0f;
            StatDamageReaction = 0f;
            StatDamageReactionVaporize = 0f;
            StatDamageReactionOverloaded = 0f;
            StatDamageReactionMelt = 0f;
            StatDamageReactionElectrocharged = 0f;
            StatDamageReactionSuperconduct = 0f;
            StatDamageReactionSwirl = 0f;
            StatDamageReactionBurning = 0f;
            StatDamageReactionBloom = 0f;
            StatDamageReactionHyperbloom = 0f;
            StatDamageReactionBurgeon = 0f;
            StatDamageReactionQuicken = 0f;
            StatDamageReactionAggravate = 0f;
            StatDamageReactionSpread = 0f;
            StatDamageReactionShatter = 0f;
            StatDamageReactionFrozen = 0f;
            StatDamageReactionCrystallize = 0f;
            WeaponSize = 1f;

            if (TimerWeaponInfusion < 1)
            {
                WeaponInfusion = GenshinElement.NONE;
                WeaponInfusionOverridable = true;
            }

            for (int i = ICDTrackers.Count - 1; i >= 0; i--)
            {
                ICDTrackers[i].Timer--;
                if (ICDTrackers[i].Timer <= 0) ICDTrackers.RemoveAt(i);
            }
        }

        public void TryUseAbility(GenshinAbility ability)
        {
            if (!ability.IsUsed() && ability.CanUse() && CanUseAbility && Energy >= ability.Energy && GenshinPlayer.TryUseStamina(ability.Stamina) && !IsHoldingAbility)
            {
                Energy -= ability.Energy;
                AbilityCurrent = ability;
                ability.Use();
            }
        }

        public void TryHoldAbility(GenshinAbility ability, bool keyReleased)
        {
            if (!ability.IsUsed() && ability.CanUse() && CanUseAbility && Energy >= ability.Energy && (keyReleased || AbilityHeld != null))
            {
                ability.Hold();
                GenshinPlayer.IsHolding = true;
                AbilityHeld = ability;
                if (ability.HoldMax)
                {
                    if (GenshinPlayer.TryUseStamina(ability.Stamina))
                    {
                        Energy -= ability.Energy;
                        AbilityCurrent = ability;
                        AbilityHeld = null;
                        ability.Use();
                    }
                    ability.HoldReset();
                }
            }
        }

        public void StopHoldAbility()
        {
            if (!AbilityHeld.IsUsed() && AbilityHeld.CanUse() && CanUseAbility && Energy >= AbilityHeld.Energy)
            {
                if (GenshinPlayer.TryUseStamina(AbilityHeld.Stamina))
                {
                    Energy -= AbilityHeld.Energy;
                    AbilityCurrent = AbilityHeld;
                    AbilityCurrent.Use();
                }
            }
            AbilityHeld.HoldReset();
            AbilityHeld = null;
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
            TimerVanityWeapon = 30;

            int type = ModContent.DustType<SwapDust>();
            for (int i = 0; i < 10; i++)
                Dust.NewDust(Player.position, Player.width, Player.height, type, 0f, 0f);
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
            if (IsAlive)
            {
                value = (int)(value * (1f + StatHealingReceived));
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
        }

        public void Revive(float maxHealth = 0.1f, int value = 0, bool combatText = false)
        {
            Health = (int)(EffectiveHealth * maxHealth) + value;
            if (Health > EffectiveHealth) Health = EffectiveHealth;
            CombatText.NewText(Player.Hitbox, new Color(188, 255, 55), value);
        }

        public void Infuse(GenshinElement element, int duration, bool overridable = true)
        {
            if (WeaponInfusionOverridable || !overridable) // Only a non overridable infusion can replace a non overridable infusion.
            {
                WeaponInfusion = element;
                TimerWeaponInfusion = duration;
                WeaponInfusionOverridable = overridable;
            }
        }

        public void Damage(int value, GenshinElement element, bool crit = false, bool combatText = true)
        {
            if (OnDamage(value))
            {
                value = ApplyDefense(value);
                if (IsCurrentCharacter && combatText) CombatText.NewText(Player.Hitbox, new Color(255, 80, 80), value, crit);
                if (GenshinPlayer.Shields.Count > 0)
                { // Shield damage calculations
                    int highestRemainingHealth = int.MinValue;
                    foreach (GenshinShield shield in GenshinPlayer.Shields)
                    {
                        int remainingValue = value;
                        if (shield.Element == GenshinElement.GEO) remainingValue = (int)(remainingValue / 1.5f / (1f + StatShieldStrength));
                        else if (shield.Element == element) remainingValue = (int)(remainingValue / 2.5f / (1f + StatShieldStrength));
                        else remainingValue = (int)(remainingValue / (1f + StatShieldStrength));

                        shield.Health -= remainingValue;
                        if (shield.Health > highestRemainingHealth)
                            highestRemainingHealth = shield.Health;
                    }
                    value = -highestRemainingHealth;
                }

                if (value > 0) 
                {
                    Health -= value;
                    if (Health <= 0)
                    { // Death
                        for (int i = 0; i < 20; i++)
                        {
                            Dust dust = Main.dust[Dust.NewDust(Player.position, Player.width, Player.height, DustID.Smoke, 0f, 0f)];
                            dust.scale *= 1.75f + Main.rand.NextFloat(0.5f);
                        }

                        for (int i = 0; i < 5; i++)
                        {
                            Dust dust = Main.dust[Dust.NewDust(Player.position, Player.width, Player.height, DustID.Smoke, 0f, 0f)];
                            dust.scale *= 2.5f + Main.rand.NextFloat(0.5f);
                        }

                        if (OnDeath())
                        {
                            GenshinPlayer.OnCharacterDeath();
                            Health = 0;
                            RemoveVanityWeapon(300);
                        }
                        else
                        {
                            if (Health < 1)
                                Health = 1;
                        }
                    }
                }
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
        public bool IsHoldingAbility => AbilityHeld != null;
        public bool IsCurrentCharacter => GenshinPlayer.CharacterCurrent == this;

        public void TryEquipWeapon(GenshinWeapon weapon)
        {
            if (weapon.WeaponType == WeaponType) ForceEquipWeapon(weapon);
        }

        public void RemoveVanityWeapon(int time = 150)
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

        public float GetReactionBonus(GenshinReaction reaction)
        {
            float bonus = 0;
            bonus += StatDamageReaction;


            switch (reaction)
            {
                case GenshinReaction.VAPORIZE:
                    bonus += StatDamageReactionVaporize;
                    break;
                case GenshinReaction.OVERLOADED:
                    bonus += StatDamageReactionOverloaded;
                    break;
                case GenshinReaction.MELT:
                    bonus += StatDamageReactionMelt;
                    break;
                case GenshinReaction.ELECTROCHARGED:
                    bonus += StatDamageReactionElectrocharged;
                    break;
                case GenshinReaction.FROZEN:
                    bonus += StatDamageReactionFrozen;
                    break;
                case GenshinReaction.SUPERCONDUCT:
                    bonus += StatDamageReactionSuperconduct;
                    break;
                case GenshinReaction.SWIRL:
                    bonus += StatDamageReactionSwirl;
                    break;
                case GenshinReaction.CRYSTALLIZE:
                    bonus += StatDamageReactionCrystallize;
                    break;
                case GenshinReaction.BURNING:
                    bonus += StatDamageReactionBurning;
                    break;
                case GenshinReaction.BLOOM:
                    bonus += StatDamageReactionBloom;
                    break;
                case GenshinReaction.HYPERBLOOM:
                    bonus += StatDamageReactionHyperbloom;
                    break;
                case GenshinReaction.BURGEON:
                    bonus += StatDamageReactionBurgeon;
                    break;
                case GenshinReaction.QUICKEN:
                    bonus += StatDamageReactionQuicken;
                    break;
                case GenshinReaction.AGGRAVATE:
                    bonus += StatDamageReactionAggravate;
                    break;
                case GenshinReaction.SPREAD:
                    bonus += StatDamageReactionSpread;
                    break;
                case GenshinReaction.SHATTER:
                    bonus += StatDamageReactionShatter;
                    break;
                default: // NONE
                    break;
            }

            return bonus;
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
