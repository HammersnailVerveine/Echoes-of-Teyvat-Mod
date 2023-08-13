using GenshinMod.Common.GameObjects;
using GenshinMod.Common.ModObjects.ModSystems;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace GenshinMod.Common.ModObjects
{
    public class UnlockablesPlayer : ModPlayer
    {
        public List<GenshinCharacter> UnlockedCharacters = new List<GenshinCharacter>();
        GenshinPlayer GenshinPlayer => Player.GetModPlayer<GenshinPlayer>();

        public override void Initialize()
        {
            UnlockedCharacters = new List<GenshinCharacter>();

            UnlockedCharacters.Add(new Content.Characters.Amber.CharacterAmber().Initialize(GenshinPlayer));
            UnlockedCharacters.Add(new Content.Characters.Kaeya.CharacterKaeya().Initialize(GenshinPlayer));
            UnlockedCharacters.Add(new Content.Characters.Lisa.CharacterLisa().Initialize(GenshinPlayer));
            UnlockedCharacters.Add(new Content.Characters.Noelle.CharacterNoelle().Initialize(GenshinPlayer));
            UnlockedCharacters.Add(new Content.Characters.Barbara.CharacterBarbara().Initialize(GenshinPlayer));

            GenshinPlayer.CharacterTeam = new List<GenshinCharacter>();
            GenshinPlayer.CharacterTeam.Add(UnlockedCharacters[0]);
            GenshinPlayer.CharacterCurrent = GenshinPlayer.CharacterTeam[0];
        }

        public override void OnEnterWorld()
        {
            if (GenshinDemo.SecondChallenge)
            {
                UnlockedCharacters.Add(new Content.Characters.Jean.CharacterJean().Initialize(GenshinPlayer));
                UnlockedCharacters.Add(new Content.Characters.Albedo.CharacterAlbedo().Initialize(GenshinPlayer));
                UnlockedCharacters.Add(new Content.Characters.Klee.CharacterKlee().Initialize(GenshinPlayer));
            }
        }
    }
}
