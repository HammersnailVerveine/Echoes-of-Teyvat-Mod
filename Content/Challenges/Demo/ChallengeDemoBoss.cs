using GenshinMod.Common.GameObjects;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.ModObjects.ModSystems;
using GenshinMod.Content.NPCs.Boss.HypostasisGeo;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Content.Challenges.Demo
{
    public class ChallengeDemoBoss : GenshinChallenge
    {
        public override void Initialize()
        {
            CenterLocation = new Vector2(8040, 5451);
            Border = 880;

            List<Tuple<int, int>> enemies = new List<Tuple<int, int>>();
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<HypostasisGeo>(), 0));
            Waves.Add(new GenshinChallengeWave(enemies));
        }

        public override void OnComplete()
        {
            if (!GenshinDemo.SecondChallenge)
            {
                UnlockablesPlayer unlockables = Main.LocalPlayer.GetModPlayer<UnlockablesPlayer>();
                GenshinPlayer genshinPlayer = Main.LocalPlayer.GetModPlayer<GenshinPlayer>();
                unlockables.UnlockedCharacters.Add(new Content.Characters.Jean.CharacterJean().Initialize(genshinPlayer));
                unlockables.UnlockedCharacters.Add(new Content.Characters.Albedo.CharacterAlbedo().Initialize(genshinPlayer));
                unlockables.UnlockedCharacters.Add(new Content.Characters.Klee.CharacterKlee().Initialize(genshinPlayer));
            }
            GenshinDemo.SecondChallenge = true;
        }

        public override void OnStart()
        {
            GenshinPlayer genshinPlayer = Main.LocalPlayer.GetModPlayer<GenshinPlayer>();
            foreach (GenshinCharacter genshinCharacter in genshinPlayer.CharacterTeam)
            {
                genshinCharacter.RestoreFull(true);
            }
        }
    }
}
