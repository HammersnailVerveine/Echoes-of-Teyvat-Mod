using GenshinMod.Common.Configs;
using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.GlobalObjets;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using static GenshinMod.Common.Static.MethodSwaps;

namespace GenshinMod
{
    public class GenshinMod : Mod
    {
        internal static GenshinMod Instance { get; private set; }

        public override void Load()
        {
            GenshinElementUtils.LoadTexture();
            ApplyMethodSwaps();
            base.Load();
        }

        public override void Unload()
        {
            GenshinElementUtils.UnloadTexture();
            base.Unload();
        }

        public GenshinMod()
        {
            Instance = this;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            GenshinModMessageType msgType = (GenshinModMessageType)reader.ReadByte();
            switch (msgType)
            {
                case GenshinModMessageType.CharacterUseAbilityServer:
                    ModPacket packet = GetPacket();
                    packet.Write((byte)GenshinModMessageType.CharacterUseAbility);
                    packet.Write((byte)whoAmI);
                    packet.Write(reader.ReadByte());
                    packet.Send(-1, whoAmI);
                    return;
                case GenshinModMessageType.CharacterUseAbility:
                    GenshinCharacter character = Main.player[reader.ReadByte()].GetModPlayer<GenshinPlayer>().CharacterCurrent;
                    byte abilityType = reader.ReadByte(); // 0 - 3
                    GenshinAbility ability = null;

                    switch (abilityType)
                    {
                        case 0: // Normal
                            ability = character.AbilityNormal;
                            break;
                        case 1: // Charged
                            ability = character.AbilityCharged;
                            break;
                        case 2: // Skill
                            ability = character.AbilitySkill;
                            break;
                        case 3: // Burst
                            ability = character.AbilityBurst;
                            break;
                        default:
                            break;
                    }

                    if (ability != null)
                    {
                        character.AbilityCurrent = ability;
                        if (ability == character.AbilityBurst && ModContent.GetInstance<GenshinConfigClient>().EnableBurstQuotes)
                            CombatText.NewText(character.Player.Hitbox, Color.White, character.BurstQuotes[Main.rand.Next(3)]);
                        ability.Use();
                    }

                    Main.NewText("use ability " + character.Player.name + " " + ability.AbilityType);
                    return;
                case GenshinModMessageType.CharacterStartHoldAbilityServer:
                    packet = GetPacket();
                    packet.Write((byte)GenshinModMessageType.CharacterStartHoldAbility);
                    packet.Write((byte)whoAmI);
                    packet.Write(reader.ReadByte());
                    packet.Send(-1, whoAmI);
                    return;
                case GenshinModMessageType.CharacterStartHoldAbility:
                    character = Main.player[reader.ReadByte()].GetModPlayer<GenshinPlayer>().CharacterCurrent;
                    abilityType = reader.ReadByte(); // 0 - 3
                    ability = null;

                    switch (abilityType)
                    {
                        case 0: // Normal
                            ability = character.AbilityNormal;
                            break;
                        case 1: // Charged
                            ability = character.AbilityCharged;
                            break;
                        case 2: // Skill
                            ability = character.AbilitySkill;
                            break;
                        case 3: // Burst
                            ability = character.AbilityBurst;
                            break;
                        default:
                            break;
                    }

                    if (ability != null)
                    {
                        character.AbilityHeld = ability;
                        character.GenshinPlayer.IsHolding = true;
                        ability.Hold();
                        character.SyncHoldingAbility = true;
                    }

                    Main.NewText("start holding " + character.Player.name);
                    Main.NewText(character.Player.name + " " + character.IsLocalPlayer + " " + character.IsCurrentCharacter + " " + character.SyncHoldingAbility);
                    return;
                case GenshinModMessageType.CharacterStopHoldAbilityServer:
                    packet = GetPacket();
                    packet.Write((byte)GenshinModMessageType.CharacterStopHoldAbility);
                    packet.Write((byte)whoAmI);
                    packet.Write(reader.ReadBoolean());
                    packet.Send(-1, whoAmI);
                    return;
                case GenshinModMessageType.CharacterStopHoldAbility:
                    character = Main.player[reader.ReadByte()].GetModPlayer<GenshinPlayer>().CharacterCurrent;
                    bool useAbility = reader.ReadBoolean();

                    if (character.AbilityHeld != null)
                    {
                        if (useAbility)
                        {
                            character.AbilityCurrent = character.AbilityHeld;
                            character.AbilityCurrent.Use();
                        }

                        character.AbilityHeld.HoldReset();
                    }

                    character.AbilityHeld = null;
                    character.SyncHoldingAbility = false;
                    Main.NewText("stop holding " + character.Player.name);
                    return;
                case GenshinModMessageType.PlayerSendCurrentCharacterServer:
                    packet = GetPacket();
                    packet.Write((byte)GenshinModMessageType.PlayerSendCurrentCharacter);
                    packet.Write((byte)whoAmI);
                    packet.Write(reader.ReadByte());
                    packet.Send(-1, whoAmI);
                    return;
                case GenshinModMessageType.PlayerSendCurrentCharacter:
                    GenshinPlayer genshinPlayer = Main.player[reader.ReadByte()].GetModPlayer<GenshinPlayer>();
                    Type T = UnlockablesPlayer.UnlockedCharacters[reader.ReadByte()].Item1.GetType();
                    Main.NewText(T.ToString());
                    genshinPlayer.CharacterTeam = new List<GenshinCharacter>();
                    genshinPlayer.CharacterTeam.Add(((GenshinCharacter)Activator.CreateInstance(T)).Initialize(genshinPlayer));
                    genshinPlayer.CharacterCurrent = genshinPlayer.CharacterTeam[0];
                    return;
                case GenshinModMessageType.CombatTextDamageServer:
                    packet = GetPacket();
                    packet.Write((byte)GenshinModMessageType.CombatTextDamage);
                    packet.Write(reader.ReadByte());
                    packet.Write(reader.ReadByte());
                    packet.Write(reader.ReadInt32());
                    packet.Send(-1, whoAmI);
                    return;
                case GenshinModMessageType.CombatTextDamage:
                    NPC target = Main.npc[reader.ReadByte()];
                    GenshinElement element = (GenshinElement)reader.ReadByte();
                    int damage = reader.ReadInt32();
                    if (damage > 0)
                    {
                        CombatText.NewText(GenshinGlobalNPC.ExtendedHitboxFlat(target), GenshinElementUtils.GetColor(element), damage);
                    }
                    else
                    {
                        CombatText.NewText(GenshinGlobalNPC.ExtendedHitboxFlat(target), GenshinElementUtils.ColorImmune, "Immune");
                    }
                    return;
                default:
                    return;
            }
        }
    }
}