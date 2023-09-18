using GenshinMod.Common.GameObjects;
using GenshinMod.Common.ModObjects.ModSystems;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using GenshinMod.Content.Characters;
using Terraria;
using Terraria.ID;
using GenshinMod.Common.GameObjects.Enums;

namespace GenshinMod.Common.ModObjects
{
    public class UnlockablesPlayer : ModPlayer
    {
        public static List<Tuple<GenshinCharacter, bool>> UnlockedCharacters = new List<Tuple<GenshinCharacter, bool>>();
        GenshinPlayer GenshinPlayer => Player.GetModPlayer<GenshinPlayer>();

        public override void Initialize()
        {
        }

        public override void OnEnterWorld()
        {
            UnlockedCharacters = new List<Tuple<GenshinCharacter, bool>>();

            AddCharacter<Content.Characters.Amber.CharacterAmber>(true);
            AddCharacter<Content.Characters.Kaeya.CharacterKaeya>(true);
            AddCharacter<Content.Characters.Lisa.CharacterLisa>(true);
            AddCharacter<Content.Characters.Noelle.CharacterNoelle>(true);
            AddCharacter<Content.Characters.Barbara.CharacterBarbara>(true);
            AddCharacter<Content.Characters.Jean.CharacterJean>(false);
            AddCharacter<Content.Characters.Albedo.CharacterAlbedo>(false);
            AddCharacter<Content.Characters.Klee.CharacterKlee>(false);

            GenshinPlayer.CharacterTeam = new List<GenshinCharacter>();
            GenshinPlayer.CharacterTeam.Add(UnlockedCharacters[0].Item1);
            GenshinPlayer.CharacterCurrent = GenshinPlayer.CharacterTeam[0];

            if (GenshinDemo.SecondChallenge)
            {
                UnlockCharacter<Content.Characters.Jean.CharacterJean>();
                UnlockCharacter<Content.Characters.Albedo.CharacterAlbedo>();
                UnlockCharacter<Content.Characters.Klee.CharacterKlee>();
            }

            /*
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = GenshinMod.Instance.GetPacket();
                packet.Write((byte)GenshinModMessageType.PlayerSendCurrentCharacterServer);
                for (int i = 0; i < UnlockedCharacters.Count; i++)
                {
                    if (UnlockedCharacters[i].Item1.GetType() == GenshinPlayer.CharacterCurrent.GetType())
                    {
                        packet.Write((byte)i);
                        break;
                    }
                }
                packet.Send();
            }
            */
        }

        public void UnlockCharacter<T>()
        {
            for (int i = 0; i < UnlockedCharacters.Count; i ++)
            {
                Tuple<GenshinCharacter, bool> tuple = UnlockedCharacters[i];
                if (tuple.Item1.GetType() == typeof(T)) {
                    UnlockedCharacters[i] = new Tuple<GenshinCharacter, bool>(((GenshinCharacter)Activator.CreateInstance(typeof(T))).Initialize(GenshinPlayer), true);
                    return;
                }
            }
        }

        public void AddCharacter<T>(bool unlocked) where T : GenshinCharacter => UnlockedCharacters.Add(new Tuple<GenshinCharacter, bool>(((GenshinCharacter)Activator.CreateInstance(typeof(T))).Initialize(GenshinPlayer), unlocked));
        public Tuple<GenshinCharacter, bool> GetCharacter<T>(bool unlocked) where T : GenshinCharacter => new Tuple<GenshinCharacter, bool>(((GenshinCharacter)Activator.CreateInstance(typeof(T))).Initialize(GenshinPlayer), unlocked);
    }
}
