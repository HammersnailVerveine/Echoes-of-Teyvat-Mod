using GenshinMod.Common.GameObjects;
using GenshinMod.Common.Loadables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Common.ModObjects
{
    public class GenshinPlayer : ModPlayer
    {
        public GenshinCharacter CharacterCurrent;
        public List<GenshinCharacter> CharacterTeam;
        public int timerMovement = 0;
        public int timerUse = 0;
        public int timerUseRef = 0;
        public int lastUseDirection = 1;

        public bool IsUsing() => timerUse > 0;

        public override void Initialize()
        {
            CharacterTeam = new List<GenshinCharacter>();
            CharacterTeam.Add(new Content.Characters.Klee.CharacterKlee().Initialize(this));
            CharacterTeam.Add(new Content.Characters.Barbara.CharacterBarbara().Initialize(this));
            CharacterCurrent = CharacterTeam[0];
        }

        public override void PreUpdate()
        {
            //PlayerInput.ScrollWheelDeltaForUI = 0;
            PlayerInput.ScrollWheelDelta = 0;
            Player.statManaMax2 = 0;

            foreach (GenshinCharacter character in CharacterTeam)
                character.PreUpdate();
        }

        public override void PostUpdate()
        {
            foreach (GenshinCharacter character in CharacterTeam)
                character.PostUpdate();
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (Player.active)
            {
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

        public override void ResetEffects()
        {
            if (Player.velocity.X != 0)
            {
                timerMovement += 1 + (int)(Math.Abs(Player.velocity.X) * 1.25f);
                if (timerMovement >= 140) timerMovement = 0;
            }
            else timerMovement = 0;

            if (timerUse > 0)
            {
                timerUse--;
                if (timerUse <= 0) timerUseRef = 0;
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
                if (GenshinKeybindsLoader.Character1.JustPressed) CharacterCurrent = CharacterTeam[0];
                if (GenshinKeybindsLoader.Character2.JustPressed) CharacterCurrent = CharacterTeam[1];
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (CharacterCurrent == null) return;
            SpriteBatch spriteBatch = Main.spriteBatch;

            Vector2 drawPosition = (Player.position + new Vector2(Player.width * 0.5f, Player.gfxOffY + 20 + 530)).Floor();
            drawPosition = Vector2.Transform(drawPosition - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
            Point coord = new Point((int)(Player.position.X / 16), (int)(Player.position.Y / 16));
            SpriteEffects effect = (IsUsing() ? lastUseDirection : Player.direction) == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

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
                    while ((movementFrame + 1) * 10 < timerMovement)
                    {
                        movementFrame++;
                    }
                    movementFrame += 6;

                    if (IsUsing() && lastUseDirection != Player.direction)
                    {
                        legFrame = 19 - movementFrame + 6;
                    }
                }
            }

            int useFrame = -1;
            if (timerUse > 0)
            {
                useFrame = 3;
                while (useFrame * (timerUseRef / 4) >= timerUse)
                {
                    useFrame--;
                }
                useFrame = 1 + (3 - useFrame);
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

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            GiveTeamEnergy(CharacterElement.NONE, 1); // TEMP
        }

        public void GiveTeamEnergy(CharacterElement element, float value)
        {
            foreach (GenshinCharacter character in CharacterTeam)
                character.GainEnergy(element, character == CharacterCurrent ? value : value * 0.6f);   
        }
    }
}
