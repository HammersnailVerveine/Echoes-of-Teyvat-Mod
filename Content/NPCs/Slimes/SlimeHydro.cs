using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.GlobalObjets;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace GenshinMod.Content.NPCs.Slimes
{
    public class SlimeHydro : GenshinNPC
    {
        private bool WasHit;
        private const int State_Waiting = 0;
        private const int State_Jumping_Begin = 1;
        private const int State_Jumping = 2;

        public override void SafeSetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SafeSetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 150;
            NPC.lifeMax = 200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 1f;
            NPC.alpha = 32;

            GenshinGlobalNPC.Element = GenshinElement.HYDRO;
            GenshinGlobalNPC.ResistanceHydro = 1f;
        }

        public override void OnTakeDamage(Player player, int damage)
        {
            WasHit = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void SafeAI()
        {
            NPC.velocity.Y += 0.2f;
            NPC.velocity.X *= 0.95f;

            switch (AI_State)
            {
                case State_Waiting:
                    AIStateWaiting();
                    break;
                case State_Jumping_Begin:
                    AIStateJumpingBegin();
                    break;
                case State_Jumping:
                    AIStateJumping();
                    break;
                default:
                    break;
            }
        }

        public void AIStateWaiting()
        {
            NPC.velocity.X *= 0.95f;
            SetFrame(TimerInState % 60 > 30 ? 1 : 0);

            if (TargetLocalPlayer)
            {
                if ((Main.rand.NextBool(4) || WasHit) && NPC.collideY && TimerInState % 60 == 45)
                {
                    SetAI(AI_Field_State, State_Jumping_Begin);
                    SetAI(AI_Field_Misc, Main.rand.NextFloat(5f) + 5f);
                    WasHit = false;
                }
            }
        }

        public void AIStateJumpingBegin()
        {
            switch(TimerInState)
            {
                case 1:
                    SetFrame(1);
                    break;
                case 20:
                    SetFrame(0);
                    break;
                case 40:
                    SetFrame(2);
                    break;
            }

            if (TimerInState >= 60)
            {
                SetAI(AI_Field_State, State_Jumping, false);
                NPC.velocity.Y = - AI_Misc;
                NPC.velocity.X = 5f * (VectorToTarget.X > 0f ? 1 : -1);
            }
        }

        public void AIStateJumping()
        {
            SetFrame(3);
            if ((NPC.collideY && TimerInState >= 10) || TimerInState > 120)
            {
                SetAI(AI_Field_State, State_Waiting, false);
            }
        }

        public override void SafeResetEffects()
        {
            GenshinGlobalNPC.TimerElementHydro = 600;
        }
    }
}
