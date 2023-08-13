using GenshinMod.Common.GameObjects;
using GenshinMod.Common.ModObjects;
using GenshinMod.Common.ModObjects.ModSystems;
using GenshinMod.Content.NPCs.Slimes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Content.Challenges.Demo
{
    public class ChallengeDemoFirst : GenshinChallenge
    {
        public override void Initialize()
        {
            CenterLocation = new Vector2(8040, 5451);
            Border = 500;

            List<Tuple<int, int>> enemies = new List<Tuple<int, int>>();
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeAnemo>(), 240));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeAnemo>(), 280));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeAnemo>(), 320));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeAnemo>(), 360));
            Waves.Add(new GenshinChallengeWave(enemies));

            enemies = new List<Tuple<int, int>>();
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeGeo>(), 320));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeGeo>(), 360));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeGeo>(), 400));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeGeo>(), -320));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeGeo>(), -360));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeGeo>(), -400));
            Waves.Add(new GenshinChallengeWave(enemies));

            enemies = new List<Tuple<int, int>>();
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimePyro>(), 320));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimePyro>(), 360));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimePyro>(), 400));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimePyro>(), 440));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeElectro>(), -320));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeElectro>(), -360));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeElectro>(), -400));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeElectro>(), -440));
            Waves.Add(new GenshinChallengeWave(enemies));

            enemies = new List<Tuple<int, int>>();
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeHydro>(), 320));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeHydro>(), 360));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeHydro>(), 400));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeCryo>(), 440));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeCryo>(), 480));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeCryo>(), -320));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeCryo>(), -360));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeCryo>(), -400));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeHydro>(), -440));
            enemies.Add(new Tuple<int, int>(ModContent.NPCType<SlimeHydro>(), -480));
            Waves.Add(new GenshinChallengeWave(enemies));
        }

        public override void OnComplete()
        {
            GenshinDemo.FirstChallenge = true;
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
