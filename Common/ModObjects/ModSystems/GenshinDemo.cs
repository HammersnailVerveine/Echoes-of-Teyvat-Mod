using GenshinMod.Common.UI.UIs;
using GenshinMod.Content.NPCs.Slimes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace GenshinMod.Common.ModObjects.ModSystems
{
    public class GenshinDemo : ModSystem
    {

        // Progression tags

        public static bool FirstChallenge = false;
        public static bool SecondChallenge = false;

        // Static fields

        private static Vector2 PositionCave = new Vector2(9760, 3003);

        private static Vector2 PositionChallengeCenter = new Vector2(8040, 5451);
        public static Vector2 PositionChallengeLeft = new Vector2(7840, 5451);
        public static Vector2 PositionChallengeRight = new Vector2(8256, 5451);

        private static Vector2 SlimeHole1 = new Vector2(6130, 2950);
        private static Vector2 SlimeHole2 = new Vector2(6208, 2800);

        // Misc fields

        private bool controlUpRelease = false;

        public override void SaveWorldData(TagCompound tag)
        {
            var challengeTags = new List<string>();

            if (FirstChallenge)
            {
                challengeTags.Add("first");
            }

            if (SecondChallenge)
            {
                challengeTags.Add("second");
            }

            tag.Add("GenshinDemo", challengeTags);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var challengeTags = tag.GetList<string>("GenshinDemo");
            FirstChallenge = challengeTags.Contains("first");
            SecondChallenge = challengeTags.Contains("second");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = FirstChallenge;
            flags[1] = SecondChallenge;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            FirstChallenge = flags[0];
            SecondChallenge = flags[1];
        }

        public override void UpdateUI(GameTime gameTime)
        {
            Player player = Main.LocalPlayer;
            GenshinPlayer genshinPlayer = player.GetModPlayer<GenshinPlayer>();

            // Show cave arrow

            if (player.Center.Distance(PositionCave) < 64)
            {
                UIStateMisc.ShowArrowUp = true;
                if (player.controlUp && controlUpRelease)
                {
                    Vector2 position = PositionChallengeCenter;
                    position.X -= player.width / 2;
                    position.Y -= player.height / 2;
                    player.Teleport(position, TeleportationStyleID.DebugTeleport);
                    SoundEngine.PlaySound(SoundID.MenuOpen);
                }
            }
            else UIStateMisc.ShowArrowUp = false;

            // Slimes spawn on the left of the map

            if (player.Center.X < 6600 && genshinPlayer.Timer % 300 == 0)
            {
                bool anyNPC = false;

                foreach (NPC npc in Main.npc)
                    if (npc.Center.Distance(player.Center) < 400 && GenshinProjectile.IsValidTarget(npc)) anyNPC = true;

                if (!anyNPC)
                {
                    int type;

                    switch (Main.rand.Next(6))
                    {
                        case 1:
                            type = ModContent.NPCType<SlimeAnemo>();
                            break;
                        case 2:
                            type = ModContent.NPCType<SlimeCryo>();
                            break;
                        case 3:
                            type = ModContent.NPCType<SlimeElectro>();
                            break;
                        case 4:
                            type = ModContent.NPCType<SlimeGeo>();
                            break;
                        case 5:
                            type = ModContent.NPCType<SlimeHydro>();
                            break;
                        default:
                            type = ModContent.NPCType<SlimePyro>();
                            break;
                    }

                    NPC.NewNPC(player.GetSource_FromThis(), (int)SlimeHole1.X, (int)SlimeHole1.Y, type);
                    NPC.NewNPC(player.GetSource_FromThis(), (int)SlimeHole2.X, (int)SlimeHole2.Y, type);
                }
            }


            // Misc

            UIStateTeambuilding.Available = player.position.Y < 4000;

            controlUpRelease = !player.controlUp;
            Main.time = 37000;
            Main.raining = false;
        }
    }
}