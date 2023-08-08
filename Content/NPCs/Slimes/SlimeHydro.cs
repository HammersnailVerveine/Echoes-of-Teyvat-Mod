using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.GlobalObjets;
using GenshinMod.Common.ModObjects;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace GenshinMod.Content.NPCs.Slimes
{
    public class SlimeHydro : GenshinNPC
    {
        public override void SafeSetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
        }

        public override void SafeSetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 10;
            NPC.lifeMax = 200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 1f;

            GenshinGlobalNPC.Element = GenshinElement.HYDRO;
            GenshinGlobalNPC.ResistanceGeo = 1f;
        }

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void SafeAI()
        {
        }

        public override void ResetEffects()
        {
        }
    }
}
