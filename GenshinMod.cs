using GenshinMod.Common.Configs;
using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
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
                case GenshinModMessageType.CharacterUseAbility:
                    byte abilityType = reader.ReadByte(); // 0 - 3
                    GenshinCharacter character = Main.player[whoAmI].GetModPlayer<GenshinPlayer>().CharacterCurrent;
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
                    return;
                default:
                    return;
            }
        }
    }
}