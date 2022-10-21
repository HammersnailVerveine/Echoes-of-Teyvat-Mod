using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
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
        public GenshinCharacter CharacterCurrent; // Currently selected character;
        public List<GenshinCharacter> CharacterTeam; // Current team of 4 characters
        public int TimerMovement = 0; // Used for animations
        public int TimerUse = 0; // Used for animations (swing)
        public int TimerUseRef = 0; // Used for animations
        public int LastUseDirection = 1; // Animation swing direction
        public bool ReverseUseArmDirection = false;
        public int Timer = 0; // Increased by 1 every frame

        public int StaminaBase = 100; // Maximum Base stamina
        public int StaminaBonus = 140; // Bonus stamina (max = base + bonus) (140 max)
        public float Stamina = 0; // current stamina
        public float StaminaConsumption = 1f; // Multiplies stamina consumption
        public int TimerStamina = 0; // Reset to 150 on stamina use

        public int StaminaMax => StaminaBase + StaminaBonus;

        public bool IsUsing() => TimerUse > 0;

        public override void Initialize()
        {
            CharacterTeam = new List<GenshinCharacter>();
            // TEMP
            CharacterTeam.Add(new Content.Characters.Klee.CharacterKlee().Initialize(this));
            CharacterTeam.Add(new Content.Characters.Barbara.CharacterBarbara().Initialize(this));
            CharacterTeam.Add(new Content.Characters.Kaeya.CharacterKaeya().Initialize(this));
            // TEMP
            CharacterCurrent = CharacterTeam[0];
        }

        public override void PreUpdate()
        {
            foreach (GenshinCharacter character in CharacterTeam)
                character.PreUpdate();
        }

        public override void UpdateEquips()
        {
            foreach (GenshinCharacter character in CharacterTeam)
                character.Update();
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

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            CharacterCurrent.Damage(damage, crit);
            crit = false;
            damage = 1;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            CharacterCurrent.Damage(damage, crit);
            crit = false;
            damage = 1;
        }

        public override void ResetEffects()
        {
            PlayerInput.ScrollWheelDelta = 0;
            Player.statLifeMax2 = 1000;
            Player.statLife = Player.statLifeMax2;

            Timer++;

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
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (CharacterCurrent != null)
            {
                if (GenshinKeybindsLoader.AbilitySkill.JustPressed) CharacterCurrent.TryUseAbility(CharacterCurrent.AbilitySkill);
                if (GenshinKeybindsLoader.AbilityBurst.JustPressed) CharacterCurrent.TryUseAbility(CharacterCurrent.AbilityBurst);
                if (GenshinKeybindsLoader.Character1.JustPressed) TrySwapCharacter(0);
                if (GenshinKeybindsLoader.Character2.JustPressed) TrySwapCharacter(1);
                if (GenshinKeybindsLoader.Character3.JustPressed) TrySwapCharacter(2);
                if (GenshinKeybindsLoader.Character4.JustPressed) TrySwapCharacter(3);
                if (GenshinKeybindsLoader.Character5.JustPressed) TrySwapCharacter(4);
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (CharacterCurrent == null) return;
            SpriteBatch spriteBatch = Main.spriteBatch;

            Vector2 drawPosition = (Player.position + new Vector2(Player.width * 0.5f, Player.gfxOffY + 20 + 530)).Floor();
            drawPosition = Vector2.Transform(drawPosition - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
            Point coord = new Point((int)(Player.position.X / 16), (int)(Player.position.Y / 16));
            SpriteEffects effect = (IsUsing() ? LastUseDirection : Player.direction) == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

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

                    if (IsUsing() && LastUseDirection != Player.direction)
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

            spriteBatch.Draw(textureLegs, drawPosition, rectangleLegs, Lighting.GetColor(coord), 0f, textureLegs.Size() * 0.5f, 1f, effect, 0f);
            spriteBatch.Draw(textureBody, drawPosition, rectangleBody, Lighting.GetColor(coord), 0f, textureBody.Size() * 0.5f, 1f, effect, 0f);
            spriteBatch.Draw(textureHead, drawPosition, rectangleHead, Lighting.GetColor(coord), 0f, textureHead.Size() * 0.5f, 1f, effect, 0f);
            spriteBatch.Draw(textureArms, drawPosition, rectangleArms, Lighting.GetColor(coord), 0f, textureArms.Size() * 0.5f, 1f, effect, 0f);
        }

        public void GiveTeamEnergy(GenshinElement element, float value)
        {
            foreach (GenshinCharacter character in CharacterTeam)
                character.GainEnergy(element, character == CharacterCurrent ? value : value * 0.6f);   
        }

        public void TrySwapCharacter(int slot)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (CharacterTeam.Count > slot && CharacterCurrent.CanUseAbility)
            {
                if (CharacterCurrent != CharacterTeam[slot])
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
    }
}
