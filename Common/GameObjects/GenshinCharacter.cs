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

        public int HealthMax;
        public int Defense;

        // Dynamic variables

        public int Health;
        public int Energy;
        public int Mastery;

        public float BonusDamage;
        public float BonusHealth;
        public float BonusDefense;
        public float BonusMastery;
        public float BonusEnergyRecharge;

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
        }

        public void TryUseAbility(GenshinAbility ability)
        {
            if (!ability.IsUsed() && ability.CanUse() && CanUseAbility)
            {
                AbilityCurrent = ability;
                ability.Use();
            }
        }

        public bool CanUseAbility => AbilityCurrent == null;
        public bool IsCurrentCharacter => GenshinPlayer.CharacterCurrent == this;
    }
}
