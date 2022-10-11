using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Common.GlobalObjets
{
    public class GenshinGlobalNPC : GlobalNPC
    {
        //public override bool InstancePerEntity => true;

        public override void SetDefaults(NPC npc)
        {
            npc.defense = 0;
            npc.lifeMax = (int)(npc.lifeMax * 1.5f);
            npc.value = 0;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            npcLoot.RemoveWhere(rule => 1 == 1);
        }
    }
}