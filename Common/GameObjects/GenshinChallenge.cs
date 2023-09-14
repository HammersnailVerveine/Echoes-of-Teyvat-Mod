using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace GenshinMod.Common.GameObjects
{
    public abstract class GenshinChallenge
    {
        public List<GenshinChallengeWave> Waves;
        public Vector2 CenterLocation;
        public float Border;

        public List<NPC> NPCs;

        public GenshinChallenge()
        {
            Waves = new List<GenshinChallengeWave>();
            NPCs = new List<NPC>();
            Initialize();
        }

        public bool OngoingWave => NPCs.Count > 0;

        public virtual bool Update() // Returns true when the challenge ends
        {
            for (int i = NPCs.Count - 1; i >= 0; i--)
                if (!NPCs[i].active)
                    NPCs.RemoveAt(i);

            if (NPCs.Count == 0)
            {
                if (Waves.Count > 0)
                {
                    if (Waves[0].ManageDelay()) return StartWave();
                }
                else return true;
            }

            return false;
        }

        public void Start()
        {
            OnStart();
            StartWave();
            Waves[0].delay = 0;
        }

        public bool StartWave() // Returns true when the challenge ends
        {
            if (Waves[0].delay < 0)
            {
                Waves.RemoveAt(0);
                if (Waves.Count == 0)
                {
                    OnComplete();
                    return true;
                }
                return false;
            }

            NPCs = new List<NPC>();
            foreach (Tuple<int, int> tuple in Waves[0].EnemyTypesAndOffsetX)
            {
                NPC npc = Main.npc[NPC.NewNPC(Main.LocalPlayer.GetSource_FromThis(), (int)CenterLocation.X + tuple.Item2, (int)CenterLocation.Y, tuple.Item1)];
                NPCs.Add(npc);
                if (npc.ModNPC is GenshinNPC genshinNPC) genshinNPC.CanDespawn = false;
            }
            return false;
        }

        public void Cancel()
        {
            for (int i = NPCs.Count - 1; i >= 0; i--)
                if (NPCs[i].active)
                {
                    if (NPCs[i].ModNPC is GenshinNPC genshinNPC)
                    {
                        genshinNPC.CanDespawn = true;
                        genshinNPC.OnChallengeDespawn();
                    }
                    NPCs[i].active = false;
                }
        }

        public virtual void Initialize() { }
        public virtual void OnComplete() { }
        public virtual void OnStart() { }

    }

    public class GenshinChallengeWave
    {
        public List<Tuple<int, int>> EnemyTypesAndOffsetX;
        public int delay = 180;

        public GenshinChallengeWave(List<Tuple<int, int>> enemyTypesAndOffsetX)
        {
            EnemyTypesAndOffsetX = enemyTypesAndOffsetX;
        }

        public bool ManageDelay()
        {
            delay--;
            return delay <= 0;
        }
    }
}
