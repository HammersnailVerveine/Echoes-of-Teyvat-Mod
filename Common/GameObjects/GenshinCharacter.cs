using GenshinMod.Common.Configs;
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
        public Texture2D TextureCompositeArm;
        public Texture2D TextureAbilitySkill;
        public Texture2D TextureAbilityBurst;
        public Texture2D TextureIcon;
        public Texture2D TextureFull;

        // Default variables

        public GenshinAbility AbilityNormal;
        public GenshinAbility AbilityCharged;
        public GenshinAbility AbilitySkill;
        public GenshinAbility AbilityBurst;

        public string Name;
        public GenshinElement Element;
        public int HeightOffset = 0; // Used for composite arms height. Try and pick whatever looks good for the character

        public float BaseHealthMax = 1000f; // Max health no modifiers
        public float BaseDefenseMax = 1000f; // Max defense no modifiers
        public float BaseAttackMax = 1000f; // Max attack no modifiers
        public WeaponType WeaponType; // Character Weapon Type
        public bool Autoswing = true; // NA autoswing
        public bool AutoswingCA = false; // CA autoswing
        public GenshinRarity Rarity = GenshinRarity.FOURSTAR;

        public string[] BurstQuotes;

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

        // Reaction-related variables

        public int TimerElementGeo = 0; // Geo element application on the player (has element if Timer > 0)
        public int TimerElementAnemo = 0; // Anemo element application on the player (has element if Timer > 0)
        public int TimerElementCryo = 0; // Cryo element application on the player (has element if Timer > 0)
        public int TimerElementElectro = 0; // Electro element application on the player (has element if Timer > 0)
        public int TimerElementDendro = 0; // Dendro element application on the player (has element if Timer > 0)
        public int TimerElementHydro = 0; // Hydro element application on the player (has element if Timer > 0)
        public int TimerElementPyro = 0; // Pyro element application on the player (has element if Timer > 0)

        public int TimerReaction = 0; // General cooldown between all reactions that can affect the player. NPCs have a per-reaction cooldown instead
        public bool ReactionFrozen = true; // Character is currently frozen
        public int ReactionSuperconduct = 0; // Timer for the Superconduct physical resistance debuff
        public Vector2 ReactionFrozenPosition = Vector2.Zero; // Where was the player frozen ? (position set here every frame)
        public int ReactionFrozenDirection = 1; // In which direction is the frozen player looking?
        public int ReactionElectrocharged = -1; // decreasing timer, deals damage to the player when it reaches 0 if they are affected by both electro and hydro. does not trigger if <0 

        // Reset variables

        public float StatEnergyRecharge = 1f; // Energy Recharge % (base = 100%)
        public float StatAttack = 0f; // Bonus Attack% (base = 0%)
        public float StatHealth = 0f; // Health bonus % (of FlatHealth, base = 0%)
        public float StatDefense = 0f; // Defense bonus % (of FlatDefense, base = 0%)
        public float StatElementalMastery = 0f; // Elemental mastery (base = 0)
        public int StatAttackFlat = 0; // Bonus flat damage (base = 0)
        public int StatHealthFlat = 0; // Bonus flat health (base = 0)
        public int StatDefenseFlat = 0; // Bonus flat defense (base = 0)
        public float StatCritChance = 0.05f; // Critical Strike Chance (base = 0.05f, cap = 1f = 100%)
        public float StatCritDamage = 0.5f; // Critical Strike damage bonus (base = 0.5f)

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

        public float StatResistanceGeo = 0f; // 0f = 100% damage taken by Geo, 1f = immune, can be negative to take more damage (base = 0%)
        public float StatResistanceAnemo = 0f; // 0f = 100% damage taken by Anemo, 1f = immune, can be negative to take more damage (base = 0%)
        public float StatResistanceCryo = 0f; // 0f = 100% damage taken by Cryo, 1f = immune, can be negative to take more damage (base = 0%)
        public float StatResistanceElectro = 0f; // 0f = 100% damage taken by Electro, 1f = immune, can be negative to take more damage (base = 0%)
        public float StatResistanceDendro = 0f; // 0f = 100% damage taken by Dendro, 1f = immune, can be negative to take more damage (base = 0%)
        public float StatResistanceHydro = 0f; // 0f = 100% damage taken by Hydro, 1f = immune, can be negative to take more damage (base = 0%)
        public float StatResistancePyro = 0f; // 0f = 100% damage taken by Pyro, 1f = immune, can be negative to take more damage (base = 0%)
        public float StatResistancePhysical = 0f; // 0f = 100% damage taken by Physical (or "none"), 1f = immune, can be negative to take more damage (base = 0%)

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
        public virtual void OnPartyAdd() { } // Called when the character is added to the party
        public virtual void OnPartyRemove() { } // Called when the character is removed from the party
        public virtual bool GetProjectileCrit(GenshinProjectile genshinProjectile) => Main.rand.NextFloat(1) < StatCritChance;
        public virtual int GetProjectileCritDamage(GenshinProjectile genshinProjectile, int damage) => (int)(damage * (1f + StatCritDamage));

        public GenshinCharacter Initialize(GenshinPlayer modPlayer)
        {
            string className = GetType().Name;
            className = className.Remove(0, 9);
            string location = "GenshinMod/Content/Characters/" + className + "/Textures/" + className;
            TextureHead ??= ModContent.Request<Texture2D>(location + "_Head", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureBody ??= ModContent.Request<Texture2D>(location + "_Body", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureLegs ??= ModContent.Request<Texture2D>(location + "_Legs", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureArms ??= ModContent.Request<Texture2D>(location + "_Arms", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureFull ??= ModContent.Request<Texture2D>(location + "_Full", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureCompositeArm ??= ModContent.Request<Texture2D>(location + "_CompositeArm", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureIcon ??= ModContent.Request<Texture2D>(location + "_Icon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureAbilitySkill ??= ModContent.Request<Texture2D>(location + "_Ability_Skill", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureAbilityBurst ??= ModContent.Request<Texture2D>(location + "_Ability_Burst", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            GenshinPlayer = modPlayer;
            Player = modPlayer.Player;
            ICDTrackers = new List<ICDTracker>();
            BurstQuotes = new string[3] {"", "", ""};

            if (Weapon == null) TryEquipWeapon(GenshinWeapon.GetWeakestWeapon(WeaponType));

            SetupFull();
            return this;
        }

        public void SetupFull(bool fullEnergy = false)
        {
            SetDefaults();
            Health = EffectiveHealth;
            if (fullEnergy) Energy = AbilityBurst.Energy;
        }

        public void PreUpdate()
        {
            SafePreUpdate();
            Weapon.WeaponUpdate();

            if (AbilityCurrent != null)
                AbilityCurrent.OnUsePreUpdate();
        }

        public void Update() // Use to setup character stats. No active effects
        {
            SafePreUpdate();
            Weapon.WeaponUpdate();

            if (IsCurrentCharacter)
            {
                Weapon.WeaponUpdateActive();
            }

            if (ReactionSuperconduct > 0)
            {
                ReactionSuperconduct--;
                StatResistancePhysical -= 0.4f;
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

                if (!Main.playerInventory && IsAlive && !ReactionFrozen) // cannot use abilities with inventory open, while frozen or while dead
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
                        else if (Main.mouseRightRelease || AutoswingCA)
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

                    if (IsHoldingAbility)
                    {
                        if ((AbilityHeld == AbilityNormal && !Main.mouseLeft)
                            || (AbilityHeld == AbilityCharged && !Main.mouseRight)
                            || (AbilityHeld == AbilitySkill && !GenshinPlayer.KeySkill)
                            || (AbilityHeld == AbilityBurst && !GenshinPlayer.KeyBurst))
                            StopHoldAbility();
                    }
                }
                else if (IsHoldingAbility)
                {
                    StopHoldAbility();
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
            TimerCanUse--;
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
            StatCritChance = 0.05f;
            StatCritDamage = 0.5f;
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

            StatResistanceGeo = 0f;
            StatResistanceAnemo = 0f;
            StatResistanceCryo = 0f;
            StatResistanceElectro = 0f;
            StatResistanceDendro = 0f;
            StatResistanceHydro = 0f;
            StatResistancePyro = 0f;
            StatResistancePhysical = 0f;

            WeaponSize = 1f;

            TimerElementGeo -= 60;
            TimerElementAnemo -= 60;
            TimerElementCryo--;
            TimerElementElectro--;
            TimerElementDendro--;
            TimerElementHydro--;
            TimerElementPyro--;

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

            // Reactions

            if (ReactionFrozen) // Frozen
            {
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    TimerElementHydro -= 30;
                    TimerElementCryo -= 30;
                    CombatText.NewText(ExtendedHitboxFlat(), Color.White, Main.rand.NextBool(2) ? "tap" : "click");
                }

                TimerElementHydro--;
                TimerElementCryo--;

                if (TimerElementHydro <= 0 || TimerElementCryo <= 0) ReactionFrozen = false;
            }

            ReactionElectrocharged--;
            if (ReactionElectrocharged == 0) // E-charge
            {
                if (AffectedByElement(GenshinElement.HYDRO) && AffectedByElement(GenshinElement.ELECTRO))
                {
                    SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, Player.Center);

                    if (TimerReaction <= 0)
                    {
                        int reactionDamage = (int)(2010f / (Level * 10)); // Electro-Charged deals 2010 dmg
                        reactionDamage = ApplyResistance(reactionDamage, GenshinElement.ELECTRO);
                        if (reactionDamage > 0)
                        {
                            Damage(reactionDamage, GenshinElement.ELECTRO, false, false, 0, true);
                            CombatText.NewText(ExtendedHitboxFlat(), GenshinElementUtils.GetReactionColor(GenshinReaction.OVERLOADED), reactionDamage);
                        }
                        else CombatText.NewText(ExtendedHitboxFlat(), GenshinElementUtils.ColorImmune, "Immune");
                        TimerReaction = 30; // Reaction damage icd.
                    }

                    TimerElementHydro -= 240;
                    TimerElementElectro -= 240;
                    ReactionElectrocharged = 60;

                    Player.velocity *= 0f;
                }
            }
        }

        public void TryUseAbility(GenshinAbility ability)
        {
            if (!ability.IsUsed() && ability.CanUse() && CanUseAbility && Energy >= ability.Energy && GenshinPlayer.TryUseStamina(ability.Stamina) && !IsHoldingAbility)
            {
                Energy -= ability.Energy;
                AbilityCurrent = ability;
                ability.Use();
                if (ability == AbilityBurst && ModContent.GetInstance<GenshinConfigClient>().EnableBurstQuotes)
                    CombatText.NewText(Player.Hitbox, Color.White, BurstQuotes[Main.rand.Next(3)]);
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

        public int GetAbilityLevel(GenshinAbility ability)
        {
            if (ability == AbilityNormal || ability == AbilityCharged) return LevelAbilityNormal;
            if (ability == AbilitySkill) return LevelAbilitySkill;
            if (ability == AbilityBurst) return LevelAbilityBurst;
            return 1;
        }

        public float GetResistanceMult(GenshinElement element)
        {
            float mult = 1f;

            switch (element)
            {
                case GenshinElement.GEO:
                    mult -= StatResistanceGeo;
                    break;
                case GenshinElement.ANEMO:
                    mult -= StatResistanceAnemo;
                    break;
                case GenshinElement.CRYO:
                    mult -= StatResistanceCryo;
                    break;
                case GenshinElement.DENDRO:
                    mult -= StatResistanceDendro;
                    break;
                case GenshinElement.ELECTRO:
                    mult -= StatResistanceElectro;
                    break;
                case GenshinElement.HYDRO:
                    mult -= StatResistanceHydro;
                    break;
                case GenshinElement.PYRO:
                    mult -= StatResistancePyro;
                    break;
                default: // Physical or NONE
                    mult -= StatResistancePhysical;
                    break;
            }

            if (mult < 0f) mult = 0f;
            return mult;
        }
        public int ApplyResistance(int damage, GenshinElement element) => (int)(GetResistanceMult(element) * damage);

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

            switch (abilityType)
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
        public bool AffectedByElement(GenshinElement Element = GenshinElement.NONE)
        { // Without parameters (GenshinElement.NONE), returns false if the player is affected by ANY element
            switch (Element)
            {
                case GenshinElement.GEO:
                    return TimerElementGeo > 0;
                case GenshinElement.ANEMO:
                    return TimerElementAnemo > 0;
                case GenshinElement.CRYO:
                    return TimerElementCryo > 0;
                case GenshinElement.ELECTRO:
                    return TimerElementElectro > 0;
                case GenshinElement.DENDRO:
                    return TimerElementDendro > 0;
                case GenshinElement.HYDRO:
                    return TimerElementHydro > 0;
                case GenshinElement.PYRO:
                    return TimerElementPyro > 0;
                default:
                    foreach (GenshinElement element in System.Enum.GetValues(typeof(GenshinElement)))
                        if (element != GenshinElement.NONE) if (AffectedByElement(element)) return false;
                    return true;
            }
        }

        public void Damage(int value, GenshinElement element, bool crit = false, bool combatText = true, int application = 570, bool trueDamage = false)
        {
            if (OnDamage(value))
            {
                if (application > 0) ApplyElement(element, ref value, application);
                if (!trueDamage)
                {
                    value = ApplyResistance(value, element);
                    value = ApplyDefense(value);
                }
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
                            Energy = 0;
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

        public void InflictElement(GenshinElement Element, int duration)
        {
            switch (Element)
            {
                case GenshinElement.GEO:
                    if (duration > TimerElementGeo) TimerElementGeo = duration;
                    break;
                case GenshinElement.ANEMO:
                    if (duration > TimerElementAnemo) TimerElementAnemo = duration;
                    break;
                case GenshinElement.CRYO:
                    if (duration > TimerElementCryo) TimerElementCryo = duration;
                    break;
                case GenshinElement.ELECTRO:
                    if (duration > TimerElementElectro) TimerElementElectro = duration;
                    break;
                case GenshinElement.DENDRO:
                    if (duration > TimerElementDendro) TimerElementDendro = duration;
                    break;
                case GenshinElement.HYDRO:
                    if (duration > TimerElementHydro) TimerElementHydro = duration;
                    break;
                case GenshinElement.PYRO:
                    if (duration > TimerElementPyro) TimerElementPyro = duration;
                    break;
                default:
                    break;
            }
        }

        public void ApplyElement(GenshinElement element, int application = 570)
        {
            int damage = 0;
            ApplyElement(element, ref damage, application);
        }

        public void ApplyElement(GenshinElement element, ref int damage, int application = 570)
        {
            bool reacted = false; // Individual attacks can only cause 1 reaction
            GenshinElement crystallizeElement = GenshinElement.NONE;
            GenshinElement swirlElement = GenshinElement.NONE;

            if (element == GenshinElement.PYRO)
            {
                if (AffectedByElement(GenshinElement.CRYO) && !reacted) // Melt Strong
                {
                    damage = (int)(damage * 2);
                    CombatText.NewText(ReactionHitbox(), GenshinElementUtils.GetReactionColor(GenshinReaction.MELT), "Melt");
                    TimerElementCryo -= application * 2; // 2x modifier
                    application = 0;
                    reacted = true;
                }

                if (AffectedByElement(GenshinElement.ELECTRO) && !reacted) // Overloaded
                {
                    if (TimerReaction <= 0)
                    {
                        int reactionDamage = (int)(3350f / (Level * 10)); // Overloaded deals 3350 dmg
                        reactionDamage = ApplyResistance(reactionDamage, GenshinElement.PYRO);
                        if (reactionDamage > 0)
                        {
                            Damage(reactionDamage, GenshinElement.PYRO, false, false, 0, true);
                            CombatText.NewText(ExtendedHitboxFlat(), GenshinElementUtils.GetReactionColor(GenshinReaction.OVERLOADED), reactionDamage);
                        }
                        else CombatText.NewText(ExtendedHitboxFlat(), GenshinElementUtils.ColorImmune, "Immune");
                        TimerReaction = 30; // Reaction damage icd.
                    }

                    int type = ModContent.ProjectileType<Content.Projectiles.ProjectileOverloaded>();
                    Projectile.NewProjectile(Player.GetSource_Misc("GenshinMod Elemental Reaction"), Player.Center, Vector2.Zero, type, 0, 15f);
                    SoundEngine.PlaySound(SoundID.Item94, Player.Center);
                    CombatText.NewText(ReactionHitbox(), GenshinElementUtils.GetReactionColor(GenshinReaction.OVERLOADED), "Overloaded");
                    TimerElementElectro -= application; // 1x modifier.
                    application = 0;
                    reacted = true;
                }

                if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Vaporize Weak
                {
                    damage = (int)(damage * 1.5);
                    CombatText.NewText(ReactionHitbox(), GenshinElementUtils.GetReactionColor(GenshinReaction.VAPORIZE), "Vaporize");
                    TimerElementHydro -= (int)(application * 0.5); // 0.5x modifier
                    application = 0;
                    reacted = true;
                }

                if (AffectedByElement(GenshinElement.GEO) && !reacted) // Pyro Crystallize
                {
                    crystallizeElement = GenshinElement.PYRO;
                    TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                }

                if (AffectedByElement(GenshinElement.ANEMO) && !reacted) // Pyro Swirl
                {
                    swirlElement = GenshinElement.PYRO;
                    TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                    reacted = true;
                }
            }


            if (element == GenshinElement.HYDRO)
            {
                if (AffectedByElement(GenshinElement.PYRO) && !reacted) // Vaporize Strong
                {
                    damage = (int)(damage * 2);
                    CombatText.NewText(ReactionHitbox(), GenshinElementUtils.GetReactionColor(GenshinReaction.VAPORIZE), "Vaporize");
                    TimerElementPyro -= application * 2; // 2x modifier
                    application = 0;
                    reacted = true;
                }

                if (AffectedByElement(GenshinElement.CRYO) && !reacted) // Frozen
                {
                    if (!ReactionFrozen)
                    {
                        int minValue = (int)(MathHelper.Min(application, TimerElementCryo));
                        TimerElementCryo = (int)(MathHelper.Max(TimerElementHydro - minValue, 0) + minValue * 2);
                        application = (int)(MathHelper.Max(application - minValue, 0) + minValue * 2);
                        ReactionFrozen = true;
                        ReactionFrozenDirection = GenshinPlayer.IsUsing ? GenshinPlayer.LastUseDirection : Player.direction;
                        ReactionFrozenPosition = Player.position;
                        CombatText.NewText(ReactionHitbox(), GenshinElementUtils.GetReactionColor(GenshinReaction.FROZEN), "Frozen");
                        reacted = true;
                    }
                }

                if (AffectedByElement(GenshinElement.ELECTRO) && !reacted) // Electro-Charged
                {
                    ReactionElectrocharged = 1;
                    CombatText.NewText(ReactionHitbox(), GenshinElementUtils.GetColor(GenshinElement.ELECTRO), "Electro-Charged");
                    reacted = true;
                }

                if (AffectedByElement(GenshinElement.GEO) && !reacted) // Hydro Crystallize
                {
                    crystallizeElement = GenshinElement.HYDRO;
                    TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                }

                if (AffectedByElement(GenshinElement.ANEMO) && !reacted) // Hydro Swirl
                {
                    swirlElement = GenshinElement.HYDRO;
                    TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                    reacted = true;
                }
            }


            if (element == GenshinElement.CRYO)
            {
                if (AffectedByElement(GenshinElement.PYRO) && !reacted) // Melt Weak
                {
                    damage = (int)(damage * 1.5);
                    CombatText.NewText(ReactionHitbox(), GenshinElementUtils.GetReactionColor(GenshinReaction.MELT), "Melt");
                    TimerElementPyro -= (int)(application * 0.5); // 0.5x modifier
                    application = 0;
                    reacted = true;
                }

                if (AffectedByElement(GenshinElement.ELECTRO) && !reacted) // Superconduct
                {
                    if (TimerReaction <= 0)
                    {
                        int reactionDamage = (int)(838f / (Level * 10)); // Superconduct deals 838 dmg
                        reactionDamage = ApplyResistance(reactionDamage, GenshinElement.CRYO);
                        if (reactionDamage > 0)
                        {
                            Damage(reactionDamage, GenshinElement.CRYO, false, false, 0, true);
                            CombatText.NewText(ExtendedHitboxFlat(), GenshinElementUtils.GetReactionColor(GenshinReaction.SUPERCONDUCT), reactionDamage);
                        }
                        else CombatText.NewText(ExtendedHitboxFlat(), GenshinElementUtils.ColorImmune, "Immune");
                        TimerReaction = 30; // Reaction damage icd.
                    }

                    ReactionSuperconduct = 720;
                    int type = ModContent.ProjectileType<Content.Projectiles.ProjectileSuperconduct>();
                    Projectile.NewProjectile(Player.GetSource_Misc("GenshinMod Elemental Reaction"), Player.Center, Vector2.Zero, type, 0, 15f);
                    SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, Player.Center);
                    CombatText.NewText(ReactionHitbox(), GenshinElementUtils.GetReactionColor(GenshinReaction.SUPERCONDUCT), "Superconduct");
                    TimerElementElectro -= application; // 1x modifier.
                    application = 0;
                    reacted = true;
                }

                if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Frozen
                {
                    if (!ReactionFrozen)
                    {
                        int minValue = (int)(MathHelper.Min(application, TimerElementHydro));
                        TimerElementHydro = (int)(MathHelper.Max(TimerElementHydro - minValue, 0) + minValue * 2);
                        application = (int)(MathHelper.Max(application - minValue, 0) + minValue * 2);
                        ReactionFrozen = true;
                        ReactionFrozenDirection = GenshinPlayer.IsUsing ? GenshinPlayer.LastUseDirection : Player.direction;
                        ReactionFrozenPosition = Player.position;
                        CombatText.NewText(ReactionHitbox(), GenshinElementUtils.GetReactionColor(GenshinReaction.FROZEN), "Frozen");
                        reacted = true;
                    }
                }

                if (AffectedByElement(GenshinElement.GEO) && !reacted) // Cryo Crystallize
                {
                    crystallizeElement = GenshinElement.CRYO;
                    TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                }

                if (AffectedByElement(GenshinElement.ANEMO) && !reacted) // Cryo Swirl
                {
                    swirlElement = GenshinElement.CRYO;
                    TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                    reacted = true;
                }
            }

            if (element == GenshinElement.ELECTRO)
            {
                if (AffectedByElement(GenshinElement.PYRO) && !reacted) // Overloaded
                {
                    if (TimerReaction <= 0)
                    {
                        int reactionDamage = (int)(3350f / (Level * 10)); // Overloaded deals 3350 dmg
                        reactionDamage = ApplyResistance(reactionDamage, GenshinElement.PYRO);
                        if (reactionDamage > 0)
                        {
                            Damage(reactionDamage, GenshinElement.PYRO, false, false, 0, true);
                            CombatText.NewText(ExtendedHitboxFlat(), GenshinElementUtils.GetReactionColor(GenshinReaction.OVERLOADED), reactionDamage);
                        }
                        else CombatText.NewText(ExtendedHitboxFlat(), GenshinElementUtils.ColorImmune, "Immune");
                        TimerReaction = 30; // Reaction damage icd.
                    }

                    int type = ModContent.ProjectileType<Content.Projectiles.ProjectileOverloaded>();
                    Projectile.NewProjectile(Player.GetSource_Misc("GenshinMod Elemental Reaction"), Player.Center, Vector2.Zero, type, 0, 15f);
                    SoundEngine.PlaySound(SoundID.Item94, Player.Center);
                    CombatText.NewText(ReactionHitbox(), GenshinElementUtils.GetReactionColor(GenshinReaction.OVERLOADED), "Overloaded");
                    TimerElementPyro -= application; // 1x modifier.
                    application = 0;
                    reacted = true;
                }

                if (AffectedByElement(GenshinElement.CRYO) && !reacted) // Superconduct
                {
                    if (TimerReaction <= 0)
                    {
                        int reactionDamage = (int)(838f / (Level * 10)); // Superconduct deals 838 dmg
                        reactionDamage = ApplyResistance(reactionDamage, GenshinElement.CRYO);
                        if (reactionDamage > 0)
                        {
                            Damage(reactionDamage, GenshinElement.CRYO, false, false, 0, true);
                            CombatText.NewText(ExtendedHitboxFlat(), GenshinElementUtils.GetReactionColor(GenshinReaction.SUPERCONDUCT), reactionDamage);
                        }
                        else CombatText.NewText(ExtendedHitboxFlat(), GenshinElementUtils.ColorImmune, "Immune");
                        TimerReaction = 30; // Reaction damage icd.
                    }

                    ReactionSuperconduct = 720;
                    int type = ModContent.ProjectileType<Content.Projectiles.ProjectileSuperconduct>();
                    Projectile.NewProjectile(Player.GetSource_Misc("GenshinMod Elemental Reaction"), Player.Center, Vector2.Zero, type, 0, 15f);
                    SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, Player.Center);
                    CombatText.NewText(ReactionHitbox(), GenshinElementUtils.GetReactionColor(GenshinReaction.SUPERCONDUCT), "Superconduct");
                    TimerElementCryo -= application; // 1x modifier.
                    application = 0;
                    reacted = true;
                }

                if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Electro-Charged
                {
                    ReactionElectrocharged = 1;
                    CombatText.NewText(ReactionHitbox(), GenshinElementUtils.GetColor(GenshinElement.ELECTRO), "Electro-Charged");
                    reacted = true;
                }

                if (AffectedByElement(GenshinElement.GEO) && !reacted) // Electro Crystallize
                {
                    crystallizeElement = GenshinElement.ELECTRO;
                    TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                }

                if (AffectedByElement(GenshinElement.ANEMO) && !reacted) // Electro Swirl
                {
                    swirlElement = GenshinElement.ELECTRO;
                    TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                    reacted = true;
                }
            }

            if (element == GenshinElement.GEO)
            {
                if (AffectedByElement(GenshinElement.PYRO) && !reacted) // Pyro Crystallize
                {
                    crystallizeElement = GenshinElement.PYRO;
                    TimerElementPyro -= (int)(application * 0.5); // 0.5x modifier
                    reacted = true;
                }
                if (AffectedByElement(GenshinElement.CRYO) && !reacted) // Cryo Crystallize
                {
                    crystallizeElement = GenshinElement.CRYO;
                    TimerElementCryo -= (int)(application * 0.5); // 0.5x modifier
                    reacted = true;
                }
                if (AffectedByElement(GenshinElement.ELECTRO) && !reacted) // Electro Crystallize
                {
                    crystallizeElement = GenshinElement.ELECTRO;
                    TimerElementElectro -= (int)(application * 0.5); // 0.5x modifier
                    reacted = true;
                }
                if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Hydro Crystallize
                {
                    crystallizeElement = GenshinElement.HYDRO;
                    TimerElementHydro -= (int)(application * 0.5); // 0.5x modifier
                    reacted = true;
                }
            }

            if (element == GenshinElement.ANEMO)
            {
                if (AffectedByElement(GenshinElement.PYRO) && !reacted) // Pyro Swirl
                {
                    swirlElement = GenshinElement.PYRO;
                    TimerElementPyro -= (int)(application * 0.5); // 0.5x modifier
                    reacted = true;
                }
                if (AffectedByElement(GenshinElement.CRYO) && !reacted) // Cryo Swirl
                {
                    swirlElement = GenshinElement.CRYO;
                    TimerElementCryo -= (int)(application * 0.5); // 0.5x modifier
                    reacted = true;
                }
                if (AffectedByElement(GenshinElement.ELECTRO) && !reacted) // Electro Swirl
                {
                    swirlElement = GenshinElement.ELECTRO;
                    TimerElementElectro -= (int)(application * 0.5); // 0.5x modifier
                    reacted = true;
                }
                if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Hydro Swirl
                {
                    swirlElement = GenshinElement.HYDRO;
                    TimerElementHydro -= (int)(application * 0.5); // 0.5x modifier
                    reacted = true;
                }
            }

            if (swirlElement != GenshinElement.NONE)
            {
                if (TimerReaction <= 0)
                {
                    int reactionDamage = (int)(1005f / (Level * 10)); // Swirl deals 1005 dmg
                    reactionDamage = ApplyResistance(reactionDamage, GenshinElement.ANEMO);
                    if (reactionDamage > 0)
                    {
                        Damage(reactionDamage, GenshinElement.ANEMO, false, false, 0, true);
                        CombatText.NewText(ExtendedHitboxFlat(), GenshinElementUtils.GetReactionColor(GenshinReaction.SWIRL), reactionDamage);
                    }
                    else CombatText.NewText(ExtendedHitboxFlat(), GenshinElementUtils.ColorImmune, "Immune");
                }

                int type = ModContent.ProjectileType<Content.Projectiles.ProjectileSwirl>();
                int proj = Projectile.NewProjectile(Player.GetSource_Misc("GenshinMod Elemental Reaction"), Player.Center, Vector2.Zero, type, 0, 15f);
                if (Main.projectile[proj].ModProjectile is Content.Projectiles.ProjectileSwirl swirl)
                    swirl.Element = swirlElement;
                CombatText.NewText(ReactionHitbox(), GenshinElementUtils.GetReactionColor(GenshinReaction.SWIRL), "Swirl");
                application = 0;
            }

            if (crystallizeElement != GenshinElement.NONE)
            {
                GenshinElementUtils.ClearCrystallizeCrystals();
                int type = ModContent.ProjectileType<Content.Projectiles.ProjectileCrystallize>();
                CombatText.NewText(ReactionHitbox(), GenshinElementUtils.GetReactionColor(GenshinReaction.CRYSTALLIZE), "Crystallize");

                Vector2 velocity = Player.velocity;
                velocity.Normalize();
                velocity *= Main.rand.NextFloat(4) + 5f;
                velocity.Y = -Main.rand.NextFloat(2) - 2f;
                float shieldValue = 200f * Level;
                Vector2 spawnPos = Player.Center;
                spawnPos.Y += 32f;
                Projectile.NewProjectile(Player.GetSource_Misc("GenshinMod Elemental Reaction"), spawnPos, velocity, type, 0, 0f, Main.myPlayer, (float)crystallizeElement, shieldValue);
                application = 0;
            }

            InflictElement(element, application);
        }

        public Rectangle ExtendedHitboxFlat()
        {
            Rectangle rect = Player.Hitbox;
            rect.X -= 32;
            rect.Y -= 32;
            rect.Width += 64;
            rect.Height += 64;
            return rect;
        }

        public Rectangle ReactionHitbox()
        {
            Rectangle rectangle = ExtendedHitboxFlat();
            rectangle.Y -= 80;
            rectangle.Height = 32;
            return rectangle;
        }
    }

    public class ICDTracker
    {
        public NPC npc;
        public int Timer;
        public int HitCount;

        public ICDTracker(NPC npc)
        {
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
