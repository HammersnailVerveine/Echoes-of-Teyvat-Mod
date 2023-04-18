using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.GlobalObjets;
using GenshinMod.Common.Loadables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Common.ModObjects
{
    public class GenshinPlayer : ModPlayer
    {
        // Fields

        public GenshinCharacter CharacterCurrent; // Currently selected character;
        public List<GenshinCharacter> CharacterTeam; // Current team of characters
        public List<GenshinShield> Shields;

        public int TimerMovement = 0; // Used for animations
        public int TimerUse = 0; // Used for animations (swing)
        public bool IsHolding = false; // Used for hold animation
        public int TimerUseRef = 0; // Used for animations
        public int LastUseDirection = 1; // Animation swing direction
        public bool ReverseUseArmDirection = false;
        public int Timer = 0; // Increased by 1 every frame

        public int TimerDeath = 0; // Set to 180 when the active character dies. Another character is selected when it reaches 0.
        public int TimerInvincibility = 0; // Cannot take damage if > 0

        public int StaminaBase = 100; // Maximum Base stamina
        public int StaminaBonus = 0; // Bonus stamina (max = base + bonus) (140 max)
        public float Stamina = 0; // current stamina
        public float StaminaConsumption = 1f; // Multiplies stamina consumption
        public int TimerStamina = 0; // Reset to 150 on stamina use

        public bool KeySkill = false; // Is the Skill key pressed ?
        public bool KeySkillRelease = true; // Was the Skill key released last frame ?
        public bool KeyBurst = false; // Is the Burst key pressed ?
        public bool KeyBurstRelease = true; // Was the Burst key released last frame ?

        public int StaminaMax => StaminaBase + StaminaBonus;

        public bool IsUsing => TimerUse > 0;
        public bool IsDead => TimerDeath > 0;

        // Overrides

        public override void Initialize()
        {
            CharacterTeam = new List<GenshinCharacter>();
            Shields = new List<GenshinShield>();
            // TEMP
            CharacterTeam.Add(new Content.Characters.Klee.CharacterKlee().Initialize(this));
            CharacterTeam.Add(new Content.Characters.Barbara.CharacterBarbara().Initialize(this));
            CharacterTeam.Add(new Content.Characters.Kaeya.CharacterKaeya().Initialize(this));
            CharacterTeam.Add(new Content.Characters.Lisa.CharacterLisa().Initialize(this));
            CharacterTeam.Add(new Content.Characters.Albedo.CharacterAlbedo().Initialize(this));
            // TEMP
            CharacterCurrent = CharacterTeam[0];
        }

        public override void PreUpdate()
        {
            foreach (GenshinCharacter character in CharacterTeam)
            {
                character.PreUpdate();
            }
        }

        public override void UpdateEquips()
        {
            foreach (GenshinCharacter character in CharacterTeam)
            {
                character.Update();

                // TEMP
                character.StatEnergyRecharge += 1f;
                character.StatHealingReceived -= 0.5f;
                character.StatShieldStrength += 10f;
                // TEMP
            }

            foreach (GenshinShield shield in Shields)
            {
                shield.Update(this);
                Player.noKnockback = true;
            }
        }

        public override void PostUpdate()
        {
            foreach (GenshinCharacter character in CharacterTeam)
                character.PostUpdate();
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (Player.active)
            { // Hide player
                Player.head = -1;
                Player.body = -1;
                Player.legs = -1;
                Player.handon = -1;
                Player.handoff = -1;
                Player.back = -1;
                Player.front = -1;
                Player.shoe = -1;
                Player.waist = -1;
                Player.shield = -1;
                Player.neck = -1;
                Player.face = -1;
                Player.balloon = -1;
                Player.wings = -1;
                Player.invis = true;
                drawInfo.hideHair = true;
            }
        }

        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            if (IsDead || TimerInvincibility > 0) return false;
            return base.CanBeHitByNPC(npc, ref cooldownSlot);
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            if (IsDead || TimerInvincibility > 0) return false;
            return base.CanBeHitByProjectile(proj);
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            GenshinElement element = npc.GetGlobalNPC<GenshinGlobalNPC>().Element;
            CharacterCurrent.Damage(damage, element, crit);
            crit = false;
            damage = 1;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            GenshinElement element = GenshinElement.NONE;
            if (proj.ModProjectile is GenshinProjectile genshinProj) element = genshinProj.Element;
            CharacterCurrent.Damage(damage, element, crit);
            crit = false;
            damage = 1;
        }

        public override void ResetEffects()
        {
            //PlayerInput.ScrollWheelDelta = 0;
            PlayerInput.ScrollWheelDeltaForUI = 0;
            Player.statLifeMax2 = 1000;
            Player.statLife = Player.statLifeMax2;
            IsHolding = false;

            Timer++;
            TimerInvincibility--;

            if (IsDead)
            {
                TimerDeath--;
                Player.moveSpeed = 0f;
                Player.velocity *= 0f;

                if (TimerDeath == 0)
                {
                    GenshinCharacter characterSwap = null;
                    foreach (GenshinCharacter character in CharacterTeam)
                    {
                        if (character.IsAlive)
                        {
                            characterSwap = character;
                            break;
                        }
                    }

                    if (characterSwap != null)
                    { // At least 1 character is alive, swap to it
                        CharacterCurrent = characterSwap;
                        CharacterCurrent.OnSwapInGlobal();
                        TimerInvincibility = 120;
                        SoundEngine.PlaySound(SoundID.MenuOpen);
                    }
                    else
                    { // All the team is dead
                        // TEMP
                        foreach (GenshinCharacter character in CharacterTeam)
                            character.Revive();
                        TimerDeath = 1;
                        // TEMP
                    }
                }
            }

            if (Player.velocity.X != 0)
            {
                TimerMovement += 1 + (int)(Math.Abs(Player.velocity.X) * 1.25f);
                if (TimerMovement >= 140) TimerMovement = 0;
            }
            else TimerMovement = 0;

            if (TimerUse > 0)
            {
                TimerUse--;
                if (TimerUse <= 0)
                {
                    TimerUseRef = 0;
                    ReverseUseArmDirection = false;
                }
            }

            TimerStamina--;
            if (Stamina < StaminaMax && TimerStamina <= 0)
            {
                Stamina += 25f / 60f; // Stamina regen = 25 per second
                if (Stamina > StaminaMax) Stamina = StaminaMax;
            }

            foreach (CombatText ct in Main.combatText)
            {
                if (ct.color == CombatText.DamagedHostile || ct.color == CombatText.DamagedHostileCrit
                    || ct.color == CombatText.DamagedFriendly || ct.color == CombatText.DamagedFriendlyCrit)
                    ct.active = false;
            }

            foreach (GenshinCharacter character in CharacterTeam)
                character.ResetEffects();

            for (int i = Shields.Count - 1; i >= 0; i--)
            {
                Shields[i].ResetEffects();
                if (Shields[i].Duration < 1 || Shields[i].Health < 1)
                    Shields.RemoveAt(i);
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (CharacterCurrent != null && !IsDead)
            {
                if (GenshinKeybindsLoader.Character1.JustPressed) TrySwapCharacter(0);
                if (GenshinKeybindsLoader.Character2.JustPressed) TrySwapCharacter(1);
                if (GenshinKeybindsLoader.Character3.JustPressed) TrySwapCharacter(2);
                if (GenshinKeybindsLoader.Character4.JustPressed) TrySwapCharacter(3);
                if (GenshinKeybindsLoader.Character5.JustPressed) TrySwapCharacter(4);


                if (GenshinKeybindsLoader.AbilitySkill.JustReleased)
                {
                    KeySkillRelease = true;
                    KeySkill = false;
                }

                if (GenshinKeybindsLoader.AbilitySkill.Current)
                {
                    KeySkill = true;
                    if (GenshinKeybindsLoader.AbilitySkill.Old) KeySkillRelease = false;
                }

                if (GenshinKeybindsLoader.AbilityBurst.JustReleased)
                {
                    KeyBurstRelease = true;
                    KeyBurst = false;
                }

                if (GenshinKeybindsLoader.AbilityBurst.Current)
                {
                    KeyBurst = true;
                    if (GenshinKeybindsLoader.AbilityBurst.Old) KeyBurstRelease = false;
                }
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (CharacterCurrent == null || IsDead) return;
            SpriteBatch spriteBatch = Main.spriteBatch;

            Vector2 drawPosition = (Player.position + new Vector2(Player.width * 0.5f, Player.gfxOffY + 20 + 530)).Floor();
            drawPosition = Vector2.Transform(drawPosition - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
            Point coord = new Point((int)(Player.position.X / 16), (int)(Player.position.Y / 16));
            Color lightColor = Lighting.GetColor(coord);
            SpriteEffects effect = (IsUsing ? LastUseDirection : Player.direction) == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int movementFrame = 0;
            int legFrame = 0;
            if (Player.velocity != Vector2.Zero)
            {
                if (Player.velocity.Y != 0)
                {
                    movementFrame = 5;
                }
                else
                {
                    while ((movementFrame + 1) * 10 < TimerMovement)
                    {
                        movementFrame++;
                    }
                    movementFrame += 6;

                    if (IsUsing && LastUseDirection != Player.direction)
                    {
                        legFrame = 19 - movementFrame + 6;
                    }
                }
            }

            int useFrame = -1;
            if (TimerUse > 0)
            {
                useFrame = 3;
                while (useFrame * (TimerUseRef / 4) >= TimerUse)
                {
                    useFrame--;
                }
                useFrame = 1 + (ReverseUseArmDirection ? useFrame : (3 - useFrame));
            }
            if (IsHolding) useFrame = 2;

            Texture2D textureLegs = CharacterCurrent.TextureLegs;
            Rectangle rectangleLegs = textureLegs.Bounds;
            rectangleLegs.Height /= 20;
            rectangleLegs.Y += rectangleLegs.Height * (legFrame == 0 ? movementFrame : legFrame);

            if (useFrame != -1) movementFrame = useFrame;

            Texture2D textureBody = CharacterCurrent.TextureBody;
            Rectangle rectangleBody = textureBody.Bounds;
            rectangleBody.Height /= 20;
            rectangleBody.Y += rectangleBody.Height * movementFrame;

            Texture2D textureHead = CharacterCurrent.TextureHead;
            Rectangle rectangleHead = textureHead.Bounds;
            rectangleHead.Height /= 20;
            rectangleHead.Y += rectangleHead.Height * movementFrame;

            Texture2D textureArms = CharacterCurrent.TextureArms;
            Rectangle rectangleArms = textureArms.Bounds;
            rectangleArms.Height /= 20;
            rectangleArms.Y += rectangleArms.Height * movementFrame;

            spriteBatch.Draw(textureLegs, drawPosition, rectangleLegs, lightColor, 0f, textureLegs.Size() * 0.5f, 1f, effect, 0f);
            spriteBatch.Draw(textureBody, drawPosition, rectangleBody, lightColor, 0f, textureBody.Size() * 0.5f, 1f, effect, 0f);
            spriteBatch.Draw(textureHead, drawPosition, rectangleHead, lightColor, 0f, textureHead.Size() * 0.5f, 1f, effect, 0f);
            spriteBatch.Draw(textureArms, drawPosition, rectangleArms, lightColor, 0f, textureArms.Size() * 0.5f, 1f, effect, 0f);

            foreach (GenshinCharacter character in CharacterTeam)
                character.DrawEffects(spriteBatch, lightColor);

            foreach (GenshinShield shield in Shields)
                shield.Draw(spriteBatch, lightColor, this);
        }

        // Methods

        public void GiveTeamEnergy(GenshinElement element, float value)
        {
            foreach (GenshinCharacter character in CharacterTeam)
                character.GainEnergy(element, character == CharacterCurrent ? value : value * 0.6f);   
        }

        public void TrySwapCharacter(int slot)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (CharacterTeam.Count > slot && CharacterCurrent.CanUseAbility && !CharacterCurrent.IsHoldingAbility)
            {
                if (CharacterCurrent != CharacterTeam[slot] && CharacterTeam[slot].IsAlive)
                {
                    if (CharacterCurrent.OnSwapOutGlobal())
                    {
                        CharacterCurrent = CharacterTeam[slot];
                        CharacterCurrent.OnSwapInGlobal();
                        SoundEngine.PlaySound(SoundID.MenuOpen);
                        return;
                    }
                }
            }
            //SoundEngine.PlaySound(SoundID.MenuClose);
            return;
        }

        public bool TryUseStamina(float value)
        {
            value = (int)(value * StaminaConsumption);
            if (value <= 0) return true;
            if (Stamina >= value)
            {
                Stamina -= value;
                TimerStamina = 150; // delays stamina recovery by 2.5 sec;
                return true;
            }
            return false;
        }

        public void AddShield(GenshinShield shield)
        {
            for (int i = Shields.Count - 1; i >= 0; i--)
            {
                if (Shields[i].GetType() == shield.GetType())
                    Shields.RemoveAt(i);
            }

            Shields.Add(shield);
        }

        public void OnCharacterDeath()
        {
            TimerDeath = 180;
        }
    }
}
