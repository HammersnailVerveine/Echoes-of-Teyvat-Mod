using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Common.GlobalObjets
{
    public class GenshinGlobalItem : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            //return base.CanUseItem(item, player);
            return false;
        }


        public override bool OnPickup(Item item, Player player)
        {
            return false;
        }
    }
}