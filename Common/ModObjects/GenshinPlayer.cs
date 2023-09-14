using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.GlobalObjets;
using GenshinMod.Common.Loadables;
using GenshinMod.Common.ModObjects.ModSystems;
using GenshinMod.Common.UI;
using GenshinMod.Common.UI.UIs;
using GenshinMod.Content.Challenges.Demo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace GenshinMod.Common.ModObjects
{
    public class GenshinPlayer : ModPlayer
    {
        // Fields

        public GenshinCharacter CharacterCurrent; // Currently selected character;
        public List<GenshinCharacter> CharacterTeam; // Current team of characters
        public List<GenshinShield> Shields;

        public GenshinChallenge Challenge;

        public byte TimerWet = 0;
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

        public byte FrozenMovementFrame = 0; // Used to display the proper animation frame when the character is frozen

        public float CompositeArmAngle = 0f; // Composite arm rotation
        public Vector2 CompositeArmOffset = Vector2.Zero; // Composite arm position relative to the center of the model player. The arm is not drawn "if == Vector.Zero"
        public bool CompositeArm => CompositeArmOffset != Vector2.Zero;

        public int StaminaMax => StaminaBase + StaminaBonus;

        public bool IsUsing => TimerUse > 0;
        public bool IsDead => TimerDeath > 0;

        // Overrides

        public override void Initialize()
        {
            CharacterTeam = new List<GenshinCharacter>();
            Shields = new List<GenshinShield>();
        }

        public override void PreUpdate()
        {
            foreach (GenshinCharacter character in CharacterTeam)
            {
                character.PreUpdate();
            }
        }

        public override void OnEnterWorld()
        {
            for (int i = 0; i < Main.InventorySlotsTotal; i++)
            {
                Item item = Player.inventory[i];

                if (item.type != ItemID.None)
                {
                    item.TurnToAir();
                    item = new Item();
                }
            }

            Player.trashItem.TurnToAir();

            for (int i = 0; i < Player.armor.Length; i++)
            {
                if (Player.armor[i].type != ItemID.None)
                {
                    Player.armor[i].TurnToAir();
                    Player.armor[i] = new Item();
                }
            }

            for (int i = 0; i < Player.dye.Length; i++)
            {
                if (Player.dye[i].type != ItemID.None)
                {
                    Player.dye[i].TurnToAir();
                    Player.dye[i] = new Item();
                }
            }

            for (int i = 0; i < Player.miscEquips.Length; i++)
            {
                if (Player.miscEquips[i].type != ItemID.None)
                {
                    Player.miscEquips[i].TurnToAir();
                    Player.miscEquips[i] = new Item();
                }
                if (Player.miscDyes[i].type != ItemID.None)
                {
                    Player.miscDyes[i].TurnToAir();
                    Player.miscDyes[i] = new Item();
                }
            }
        }

        public override void UpdateEquips()
        {
            foreach (GenshinCharacter character in CharacterTeam)
            {
                character.Update();

                // TEMP
                character.StatDamageReactionSuperconduct -= 0.5f;
                character.StatDamageReactionElectrocharged -= 0.5f;
                character.StatDamageReactionOverloaded -= 0.5f;
                character.StatDamageReactionShatter -= 0.5f;
                character.StatDamageReactionSwirl -= 0.5f;

                character.StatEnergyRecharge += 0.5f;
                character.StatHealingReceived -= 0.75f;
                character.StatShieldStrength -= 0.5f;
                // TEMP
            }

            foreach (GenshinShield shield in Shields)
            {
                shield.Update();
                Player.noKnockback = true;
            }
        }

        public override void PostUpdate()
        {
            foreach (GenshinCharacter character in CharacterTeam)
                character.PostUpdate();

            if (CharacterCurrent != null)
            {
                if (CharacterCurrent.ReactionFrozen)
                {
                    Player.velocity *= 0f;
                    Player.direction = CharacterCurrent.ReactionFrozenDirection;
                    Player.position = CharacterCurrent.ReactionFrozenPosition;
                }

                if (Player.wet)
                {
                    TimerWet++;
                    if (TimerWet == 150) // Rain follows a 2.5 sec icd for hydro application
                    {
                        TimerWet = 0;
                        CharacterCurrent.ApplyElement(GenshinElement.HYDRO);
                    }
                }
                else TimerWet = 149;
            }

            if (Challenge != null)
                if (Challenge.Update()) Challenge = null;
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

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            GenshinElement element = npc.GetGlobalNPC<GenshinGlobalNPC>().Element;
            CharacterCurrent.Damage(npc.damage, element);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            GenshinElement element = GenshinElement.NONE;
            if (proj.ModProjectile is GenshinProjectile genshinProj) element = genshinProj.Element;
            CharacterCurrent.Damage(proj.damage, element);
        }
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage *= 0f;
            Player.statLife++; // bandaid after tmodloader 1.4.4 changes
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage *= 0f;
            Player.statLife++; // bandaid after tmodloader 1.4.4 changes
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
                        foreach (GenshinCharacter character in CharacterTeam)
                        {
                            character.Revive();
                            character.RestoreFull();
                        }
                        TimerDeath = 1;

                        Player.Teleport(new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16), TeleportationStyleID.DebugTeleport);
                        Main.playerInventory = false;

                        CharacterCurrent.TimerCanUse = 60;
                        CancelChallenge();
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

            CompositeArmOffset = Vector2.Zero;
            CompositeArmAngle = 0f;

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
                {
                    Shields[i].OnKillBase(Shields[i].Health < 1);
                    Shields.RemoveAt(i);
                }
            }

            UIStateTeambuilding teambuildingState = GenshinSystemUI.GetUIState<UIStateTeambuilding>();
            teambuildingState.Visible = Main.playerInventory;
            if (!teambuildingState.Visible)
            {
                UIStateTeambuilding.SelectedSlot = -5;
                UIStateTeambuilding.PlayerTeam = null;
                UIStateTeambuilding.PlayerCharacters = null;
            }

            if (Challenge != null)
            {
                if (Player.position.X > Challenge.CenterLocation.X + Challenge.Border)
                {
                    Player.position.X = Challenge.CenterLocation.X + Challenge.Border - 0.001f;
                    Player.velocity.X *= 0.01f;
                }

                if (Player.position.X < Challenge.CenterLocation.X - Challenge.Border)
                {
                    Player.position.X = Challenge.CenterLocation.X - Challenge.Border + 0.001f;
                    Player.velocity.X *= 0.01f;
                }
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

                if (GenshinKeybindsLoader.Debug.JustPressed)
                {
                    GenshinDemo.FirstChallenge = false;
                    GenshinDemo.SecondChallenge = false;
                }


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

        public void TryApplyDamageToNPC(NPC npc, int damage, float knockback, int direction, bool crit = false, GenshinElement element = GenshinElement.NONE, int gaugeApplication = 0, bool ignoreShields = false, AttackWeight attackWeight = AttackWeight.LIGHT, bool combatText = false, bool transmitPlayer = true)
        {
            GenshinGlobalNPC globalNPC = npc.GetGlobalNPC<GenshinGlobalNPC>();
            if (npc.GetGlobalNPC<GenshinGlobalNPC>().HasShield() && !ignoreShields) foreach (GenshinShieldNPC shield in globalNPC.Shields) shield.Damage(GenshinShieldNPC.GetDamageUnit(gaugeApplication), element, attackWeight);
            else Player.ApplyDamageToNPC(npc, damage, knockback, direction, crit);

            if (combatText)
            {
                if (damage > 0) CombatText.NewText(GenshinGlobalNPC.ExtendedHitboxFlat(npc), GenshinElementUtils.GetColor(element), damage);
                else CombatText.NewText(GenshinGlobalNPC.ExtendedHitboxFlat(npc), GenshinElementUtils.ColorImmune, "Immune");
            }

            if (npc.ModNPC is GenshinNPC genshinNPC) genshinNPC.OnTakeDamage(transmitPlayer ? Player : null, damage);
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (CharacterCurrent == null || IsDead) return;
            SpriteBatch spriteBatch = Main.spriteBatch;

            Vector2 drawPosition = (Player.position + new Vector2(Player.width * 0.5f, Player.gfxOffY + 20 + 530)).Floor();
            drawPosition = Vector2.Transform(drawPosition - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
            Vector2 drawPositionBody = drawPosition;
            drawPositionBody.Y += 28;
            Point coord = new Point((int)(Player.position.X / 16), (int)(Player.position.Y / 16));
            Color lightColor = Lighting.GetColor(coord);
            SpriteEffects effect = (IsUsing ? LastUseDirection : Player.direction) == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int movementFrame = 0;
            int legFrame = 0;
            int useFrame = -1;
            if (!CharacterCurrent.ReactionFrozen)
            {
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
            }
            else
            {
                movementFrame = FrozenMovementFrame;
            }

            Texture2D textureLegs = CharacterCurrent.TextureLegs;
            Rectangle rectangleLegs = textureLegs.Bounds;
            rectangleLegs.Height /= 20;
            rectangleLegs.Y += rectangleLegs.Height * (legFrame == 0 ? movementFrame : legFrame);

            if (useFrame != -1) movementFrame = useFrame;
            FrozenMovementFrame = (byte)movementFrame;

            Texture2D textureBody = CharacterCurrent.TextureBody;
            Rectangle rectangleBody = textureBody.Bounds;
            rectangleBody.Height /= 21;
            if (CompositeArm) rectangleBody.Y = 0;
            else rectangleBody.Y += rectangleBody.Height * (movementFrame + 1);

            Texture2D textureHead = CharacterCurrent.TextureHead;
            Rectangle rectangleHead = textureHead.Bounds;
            rectangleHead.Height /= 20;
            rectangleHead.Y += rectangleHead.Height * movementFrame;

            spriteBatch.Draw(textureLegs, drawPosition, rectangleLegs, lightColor, 0f, textureLegs.Size() * 0.5f, 1f, effect, 0f);
            spriteBatch.Draw(textureBody, drawPositionBody, rectangleBody, lightColor, 0f, textureBody.Size() * 0.5f, 1f, effect, 0f);
            spriteBatch.Draw(textureHead, drawPosition, rectangleHead, lightColor, 0f, textureHead.Size() * 0.5f, 1f, effect, 0f);

            Texture2D textureArms = CharacterCurrent.TextureArms;
            Rectangle rectangleArms = textureArms.Bounds;
            if (!CompositeArm)
            {
                rectangleArms.Height /= 20;
                rectangleArms.Y += rectangleArms.Height * movementFrame;
                spriteBatch.Draw(textureArms, drawPosition, rectangleArms, lightColor, 0f, textureArms.Size() * 0.5f, 1f, effect, 0f);
            }

            foreach (GenshinCharacter character in CharacterTeam)
                character.DrawEffects(spriteBatch, lightColor);

            foreach (GenshinShield shield in Shields)
                shield.Draw(spriteBatch, lightColor, this);

            // Draw elements affecting the current character

            if (CharacterCurrent != null)
            {
                int nbElements = -1;
                foreach (GenshinElement element in Enum.GetValues(typeof(GenshinElement)))
                    if (element != GenshinElement.NONE) if (CharacterCurrent.AffectedByElement(element)) nbElements++;
                if (CharacterCurrent.AffectedByElement(GenshinElement.HYDRO) && CharacterCurrent.ReactionFrozen) nbElements--;
                int offSetY = -30;
                int offSetX = 0;
                setOffset(ref offSetX, ref offSetY, ref nbElements);

                if (CharacterCurrent.AffectedByElement(GenshinElement.GEO)) DrawTexture(GenshinElementUtils.ElementTexture[0], spriteBatch, nbElements, ref offSetX, ref offSetY, CharacterCurrent.TimerElementGeo);
                if (CharacterCurrent.AffectedByElement(GenshinElement.ANEMO)) DrawTexture(GenshinElementUtils.ElementTexture[1], spriteBatch, nbElements, ref offSetX, ref offSetY, CharacterCurrent.TimerElementAnemo);
                if (CharacterCurrent.AffectedByElement(GenshinElement.CRYO)) DrawTexture(GenshinElementUtils.ElementTexture[2], spriteBatch, nbElements, ref offSetX, ref offSetY, CharacterCurrent.TimerElementCryo);
                if (CharacterCurrent.AffectedByElement(GenshinElement.ELECTRO)) DrawTexture(GenshinElementUtils.ElementTexture[3], spriteBatch, nbElements, ref offSetX, ref offSetY, CharacterCurrent.TimerElementElectro);
                if (CharacterCurrent.AffectedByElement(GenshinElement.DENDRO)) DrawTexture(GenshinElementUtils.ElementTexture[4], spriteBatch, nbElements, ref offSetX, ref offSetY, CharacterCurrent.TimerElementDendro);
                if (CharacterCurrent.AffectedByElement(GenshinElement.HYDRO) && !CharacterCurrent.ReactionFrozen) DrawTexture(GenshinElementUtils.ElementTexture[5], spriteBatch, nbElements, ref offSetX, ref offSetY, CharacterCurrent.TimerElementHydro);
                if (CharacterCurrent.AffectedByElement(GenshinElement.PYRO)) DrawTexture(GenshinElementUtils.ElementTexture[6], spriteBatch, nbElements, ref offSetX, ref offSetY, CharacterCurrent.TimerElementPyro);

                if (CharacterCurrent.ReactionFrozen)
                {
                    Vector2 drawPositionAlt = new Vector2(drawPosition.X - 30 * Player.direction, drawPosition.Y + 50);

                    spriteBatch.Draw(textureLegs, drawPositionAlt, rectangleLegs, GenshinElementUtils.GetColor(GenshinElement.CRYO) * 0.4f, 0.05f * Player.direction, textureLegs.Size() * 0.5f, 1.1f, effect, 0f);
                    spriteBatch.Draw(textureBody, drawPositionAlt, rectangleBody, GenshinElementUtils.GetColor(GenshinElement.CRYO) * 0.4f, 0.05f * Player.direction, textureBody.Size() * 0.5f, 1.1f, effect, 0f);
                    spriteBatch.Draw(textureHead, drawPositionAlt, rectangleHead, GenshinElementUtils.GetColor(GenshinElement.CRYO) * 0.4f, 0.05f * Player.direction, textureHead.Size() * 0.5f, 1.1f, effect, 0f);
                    spriteBatch.Draw(textureArms, drawPositionAlt, rectangleArms, GenshinElementUtils.GetColor(GenshinElement.CRYO) * 0.4f, 0.05f * Player.direction, textureArms.Size() * 0.5f, 1.1f, effect, 0f);

                    drawPositionAlt = new Vector2(drawPosition.X + 30 * Player.direction, drawPosition.Y + 50);

                    spriteBatch.Draw(textureLegs, drawPositionAlt, rectangleLegs, GenshinElementUtils.GetColor(GenshinElement.CRYO) * 0.4f, -0.05f * Player.direction, textureLegs.Size() * 0.5f, 1.1f, effect, 0f);
                    spriteBatch.Draw(textureBody, drawPositionAlt, rectangleBody, GenshinElementUtils.GetColor(GenshinElement.CRYO) * 0.4f, -0.05f * Player.direction, textureBody.Size() * 0.5f, 1.1f, effect, 0f);
                    spriteBatch.Draw(textureHead, drawPositionAlt, rectangleHead, GenshinElementUtils.GetColor(GenshinElement.CRYO) * 0.4f, -0.05f * Player.direction, textureHead.Size() * 0.5f, 1.1f, effect, 0f);
                    spriteBatch.Draw(textureArms, drawPositionAlt, rectangleArms, GenshinElementUtils.GetColor(GenshinElement.CRYO) * 0.4f, -0.05f * Player.direction, textureArms.Size() * 0.5f, 1.1f, effect, 0f);

                    spriteBatch.Draw(textureLegs, drawPosition, rectangleLegs, GenshinElementUtils.GetColor(GenshinElement.CRYO) * 0.4f, 0f, textureLegs.Size() * 0.5f, 1f, effect, 0f);
                    spriteBatch.Draw(textureBody, drawPosition, rectangleBody, GenshinElementUtils.GetColor(GenshinElement.CRYO) * 0.4f, 0f, textureBody.Size() * 0.5f, 1f, effect, 0f);
                    spriteBatch.Draw(textureHead, drawPosition, rectangleHead, GenshinElementUtils.GetColor(GenshinElement.CRYO) * 0.4f, 0f, textureHead.Size() * 0.5f, 1f, effect, 0f);
                    spriteBatch.Draw(textureArms, drawPosition, rectangleArms, GenshinElementUtils.GetColor(GenshinElement.CRYO) * 0.4f, 0f, textureArms.Size() * 0.5f, 1f, effect, 0f);
                }
            }


            if (Challenge != null)
            { // Challenge borders
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

                int height = UIStateMisc.TextureBorder.Height;
                for (int k = -1; k < 2; k += 2)
                {
                    for (int i = 0; i < 200; i++)
                    {
                        Vector2 position = new Vector2(Challenge.CenterLocation.X + Challenge.Border * k + Player.width * 0.5f - 4, Player.Center.Y - height * 100 + height * i + Player.gfxOffY);
                        float ColorMult = 1f - Player.Center.Distance(position) / 240f;
                        if (ColorMult > 0f)
                        {
                            position = Vector2.Transform(position - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
                            spriteBatch.Draw(UIStateMisc.TextureBorder, position, Color.White * ColorMult);

                            if (i % 3 == 0)
                            {
                                for (int j = 1; j < 6; j++)
                                {
                                    position.X += 6 * k;
                                    position.Y -= (Timer * 0.2f % (UIStateMisc.TextureBorder.Height * 3f));
                                    ColorMult = 1f - Player.Center.Distance(position + Main.screenPosition) / 240f;
                                    spriteBatch.Draw(UIStateMisc.TextureBorder, position, Color.White * (ColorMult - 0.1f * j));
                                }
                            }
                        }
                    }
                }

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
            else
            { // challenge keys (temp)
                // First key

                Vector2 drawPositionKey = Vector2.Transform(GenshinDemo.PositionChallengeLeft - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
                drawPositionKey.Y += (float)(Math.Sin(Timer * 0.025) * 8f) - 48;
                Color keyColor = Lighting.GetColor((int)GenshinDemo.PositionChallengeLeft.X / 16, (int)GenshinDemo.PositionChallengeLeft.Y / 16) * 1.5f;

                if (Player.Center.Distance(GenshinDemo.PositionChallengeLeft) < 64)
                {
                    Main.spriteBatch.Draw(UIStateMisc.KeyTextureOutline, drawPositionKey, null, Color.White * 0.8f, 0f, UIStateMisc.KeyTextureOutline.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                    Vector2 textPosition = Vector2.Transform(GenshinDemo.PositionChallengeLeft - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
                    textPosition.Y -= 100;
                    textPosition.X += 40;
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "First challenge" + (GenshinDemo.FirstChallenge ? " (Completed)" : ""), textPosition, Color.White, 0f, Vector2.Zero, new Vector2(1f, 1f));
                    textPosition.Y += 30;
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Slime Waves", textPosition, Color.White, 0f, Vector2.Zero, new Vector2(1f, 1f));
                    textPosition.Y += 50;
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Right click to Interact", textPosition, Color.White, 0f, Vector2.Zero, new Vector2(1f, 1f));

                    if (Main.MouseScreen.Distance(drawPositionKey) < 64 && Main.mouseRight && Main.mouseRightRelease)
                    {
                        StartChallenge<ChallengeDemoFirst>();
                        CharacterCurrent.TimerCanUse = 60;
                        SoundEngine.PlaySound(SoundID.MenuOpen);
                    }
                }
                else Main.spriteBatch.Draw(UIStateMisc.KeyTexture, drawPositionKey, null, keyColor * 0.35f, 0f, UIStateMisc.KeyTexture.Size() * 0.5f, 1.05f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(UIStateMisc.KeyTexture, drawPositionKey, null, keyColor, 0f, UIStateMisc.KeyTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);


                drawPositionKey.Y += (float)(Math.Sin(Timer * 0.025) * 4f) - UIStateMisc.KeyTexture.Height / 2;
                Main.spriteBatch.Draw(UIStateMisc.KeyTextureCube, drawPositionKey, null, keyColor * 0.75f, Timer * -0.0075f, UIStateMisc.KeyTextureCube.Size() * 0.5f, 0.6f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(UIStateMisc.KeyTextureCube, drawPositionKey, null, keyColor, Timer * 0.005f, UIStateMisc.KeyTextureCube.Size() * 0.5f, 0.8f, SpriteEffects.None, 0f);

                if (GenshinDemo.FirstChallenge)
                {
                    // Second key

                    drawPositionKey = Vector2.Transform(GenshinDemo.PositionChallengeRight - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
                    drawPositionKey.Y += (float)(Math.Sin(Timer * 0.025) * 8f) - 48;
                    keyColor = Lighting.GetColor((int)GenshinDemo.PositionChallengeRight.X / 16, (int)GenshinDemo.PositionChallengeRight.Y / 16) * 1.5f;

                    if (Player.Center.Distance(GenshinDemo.PositionChallengeRight) < 64)
                    {
                        Main.spriteBatch.Draw(UIStateMisc.KeyTextureOutline, drawPositionKey, null, Color.White * 0.8f, 0f, UIStateMisc.KeyTextureOutline.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                        Vector2 textPosition = Vector2.Transform(GenshinDemo.PositionChallengeRight - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
                        textPosition.Y -= 100;
                        textPosition.X += 40;
                        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Second challenge" + (GenshinDemo.SecondChallenge ? " (Completed)" : ""), textPosition, Color.White, 0f, Vector2.Zero, new Vector2(1f, 1f));
                        textPosition.Y += 30;
                        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Geo Hypostasis", textPosition, Color.White, 0f, Vector2.Zero, new Vector2(1f, 1f));
                        textPosition.Y += 50;
                        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Right click to Interact", textPosition, Color.White, 0f, Vector2.Zero, new Vector2(1f, 1f));

                        if (Main.MouseScreen.Distance(drawPositionKey) < 64 && Main.mouseRight && Main.mouseRightRelease)
                        {
                            StartChallenge<ChallengeDemoBoss>();
                            CharacterCurrent.TimerCanUse = 60;
                            SoundEngine.PlaySound(SoundID.MenuOpen);
                        }
                    }
                    else Main.spriteBatch.Draw(UIStateMisc.KeyTexture, drawPositionKey, null, keyColor * 0.35f, 0f, UIStateMisc.KeyTexture.Size() * 0.5f, 1.05f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(UIStateMisc.KeyTexture, drawPositionKey, null, keyColor, 0f, UIStateMisc.KeyTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);

                    drawPositionKey.Y += (float)(Math.Sin(Timer * 0.025) * 4f) - UIStateMisc.KeyTexture.Height / 2;
                    Main.spriteBatch.Draw(UIStateMisc.KeyTextureCube, drawPositionKey, null, keyColor * 0.75f, (Timer + 60) * 0.0075f, UIStateMisc.KeyTextureCube.Size() * 0.5f, 0.6f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(UIStateMisc.KeyTextureCube, drawPositionKey, null, keyColor, (Timer + 60) * -0.005f, UIStateMisc.KeyTextureCube.Size() * 0.5f, 0.8f, SpriteEffects.None, 0f);
                }
            }
        }

        public void DrawCompositeArm(SpriteBatch spritebatch, bool offsetPosition = true, bool shortArm = false, float offsetX = 0f, float offsetY = 0f, float rotation = float.MaxValue)
        {
            Vector2 offset = CompositeArmOffset;
            if (offsetX != 0f || offsetY != 0f) offset = new Vector2(offsetX, offsetY);
            float angle = CompositeArmAngle;
            if (rotation != float.MaxValue) angle = rotation;

            Texture2D textureCompositeArm = CharacterCurrent.TextureCompositeArm;
            Rectangle rectangleArm = textureCompositeArm.Bounds;
            rectangleArm.Height /= 2;
            if (shortArm) rectangleArm.Y += rectangleArm.Height;

            Vector2 drawPosition = (Player.position + new Vector2(Player.width * 0.5f, Player.gfxOffY + 20)).Floor();
            if (offsetPosition) drawPosition += new Vector2(-6f * (IsUsing ? LastUseDirection : Player.direction), - CharacterCurrent.HeightOffset);
            drawPosition = Vector2.Transform(drawPosition - Main.screenPosition, Main.GameViewMatrix.EffectMatrix) + offset;

            Color lightColor = Lighting.GetColor(new Point((int)(Player.Center.X / 16), (int)(Player.Center.Y / 16)));
            SpriteEffects effect = (IsUsing ? LastUseDirection : Player.direction) == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            spritebatch.Draw(textureCompositeArm, drawPosition, rectangleArm, lightColor, angle, rectangleArm.Size() * 0.5f, 1f, effect, 0f);
        }

        // Methods

        public void setOffset(ref int offSetX, ref int offSetY, ref int nbElements)
        {
            offSetX = 0;
            while (nbElements > 0 && offSetX > -28)
            {
                nbElements--;
                offSetX -= 14;
            }
            offSetY -= 24;
        }

        public void DrawTexture(Texture2D texture, SpriteBatch spriteBatch, int nbElements, ref int offSetX, ref int offSetY, int timeLeft)
        {
            float colorMult = timeLeft > 120 ? 1f : (float)Math.Abs(Math.Sin((timeLeft * 0.5f) / Math.PI / 4f));
            Vector2 position = new Vector2(Player.Center.X + offSetX - Main.screenPosition.X, Player.Center.Y + offSetY - Main.screenPosition.Y);
            spriteBatch.Draw(texture, position, null, Color.White * colorMult, 0f, texture.Size() * 0.5f, 0.875f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position, null, Color.White * 0.5f * colorMult, 0f, texture.Size() * 0.5f, 1.025f, SpriteEffects.None, 0f);
            offSetX += 24;
            if (offSetX > 24) setOffset(ref offSetX, ref offSetY, ref nbElements);
        }

        public void GiveTeamEnergy(GenshinElement element, float value)
        {
            foreach (GenshinCharacter character in CharacterTeam)
                character.GainEnergy(element, character == CharacterCurrent ? value : value * 0.6f);
        }

        public void TrySwapCharacter(int slot)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (CharacterTeam.Count > slot && CharacterCurrent.CanUseAbility && !CharacterCurrent.IsHoldingAbility && !CharacterCurrent.ReactionFrozen)
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

        public void StartChallenge<T>() where T : GenshinChallenge
        {
            var type = typeof(T);
            Challenge = Activator.CreateInstance(type, true) as T;
            Challenge.Start();
        }

        public void CancelChallenge()
        {
            if (Challenge != null) Challenge.Cancel();
            Challenge = null;
        }

        public void OnCharacterDeath()
        {
            TimerDeath = 180;
        }
    }
}
