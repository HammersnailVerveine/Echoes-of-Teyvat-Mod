using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
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

        // Stable variables

        public GenshinAbility AbilityNormal;
        public GenshinAbility AbilityCharged;
        public GenshinAbility AbilitySkill;
        public GenshinAbility AbilityBurst;

        public string Name;
        public CharacterElement Element;

        public int FlatHealth; // Max health no modifiers
        public int FlatDefense; // Max defense no modifiers

        // Dynamic variables

        public int Health = 100; // Current health
        public float Energy = 0f; // Current energy

        public float StatDamage = 1f; // Damage % (base = 100%)
        public float StatEnergyRecharge = 1f; // Energy Recharge % (base = 100%)
        public float StatHealth = 0f; // Health bonus % (of FlatHealth, base = 0%)
        public float StatDefense = 0f; // Defense bonus % (of FlatDefense, base = 0%)
        public int StatElementalMastery = 0; // Elemental mastery (base = 0)
        public int StatDamageFlat = 0; // Bonus flat damage (base = 0)
        public int StatHealthFlat = 0; // Bonus flat health (base = 0)

        public List<ICDTracker> ICDTrackers;

        public abstract void SetDefaults();
        public virtual void SafePreUpdate() { }
        public virtual void SafePostUpdate() { }
        public virtual void SafeResetEffects() { }

        public GenshinCharacter Initialize(GenshinPlayer modPlayer)
        {
            string className = GetType().Name;
            className = className.Remove(0, 9);
            TextureHead ??= ModContent.Request<Texture2D>("GenshinMod/Content/Characters/" + className + "/Textures/" + className + "_Head", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureBody ??= ModContent.Request<Texture2D>("GenshinMod/Content/Characters/" + className + "/Textures/" + className + "_Body", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureLegs ??= ModContent.Request<Texture2D>("GenshinMod/Content/Characters/" + className + "/Textures/" + className + "_Legs", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureArms ??= ModContent.Request<Texture2D>("GenshinMod/Content/Characters/" + className + "/Textures/" + className + "_Arms", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureAbilitySkill ??= ModContent.Request<Texture2D>("GenshinMod/Content/Characters/" + className + "/Textures/" + className + "_Ability_Skill", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureAbilityBurst ??= ModContent.Request<Texture2D>("GenshinMod/Content/Characters/" + className + "/Textures/" + className + "_Ability_Burst", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            GenshinPlayer = modPlayer;
            Player = modPlayer.Player;
            ICDTrackers = new List<ICDTracker>();
            SetDefaults();
            return this;
        }

        public void PreUpdate()
        {
            SafePreUpdate();
        }

        public void PostUpdate()
        {
            SafePostUpdate();
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
                if (Main.mouseLeft && Main.mouseLeftRelease && !GenshinPlayer.IsUsing())
                {
                    TryUseAbility(AbilityNormal);
                }
                if (Main.mouseRight && Main.mouseRightRelease && !GenshinPlayer.IsUsing())
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

            for (int i = ICDTrackers.Count - 1; i >= 0; i--)
            {
                ICDTrackers[i].Timer--;
                if (ICDTrackers[i].Timer <= 0) ICDTrackers.RemoveAt(i);
            }
        }

        public void TryUseAbility(GenshinAbility ability)
        {
            if (!ability.IsUsed() && ability.CanUse() && CanUseAbility && Energy >= ability.Energy)
            {
                Energy -= ability.Energy;
                AbilityCurrent = ability;
                ability.Use();
            }
        }

        public void GainEnergy(CharacterElement element, float value)
        {
            if (element == CharacterElement.NONE) value *= 2;
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

        public bool CanUseAbility => AbilityCurrent == null;
        public bool IsCurrentCharacter => GenshinPlayer.CharacterCurrent == this;
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
