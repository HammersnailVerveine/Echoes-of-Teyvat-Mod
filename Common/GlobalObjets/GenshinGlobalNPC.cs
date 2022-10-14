using GenshinMod.Common.GameObjects;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Common.GlobalObjets
{
    public class GenshinGlobalNPC : GlobalNPC
    {
        public bool HalfLifeParticle = false;
        public CharacterElement Element = CharacterElement.NONE;

        public override bool InstancePerEntity => true;

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

        public override void AI(NPC npc)
        {
            if (npc.life <= npc.lifeMax / 2f && !HalfLifeParticle)
            {
                HalfLifeParticle = true;
                SpawnElementalParticle(npc, Element, 1f);
            }
        }

        public override void OnKill(NPC npc)
        {
            SpawnElementalParticle(npc, Element, 1f);
        }

        public void SpawnElementalParticle(NPC npc, CharacterElement element, float value, int number = 1)
        {
            int type = ModContent.ProjectileType<Content.Projectiles.ProjectileElementalParticle>();
            for (int i = 0; i < number; i++)
            {
                int proj = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, type, 0, 0f, Main.myPlayer, (float)element, value);
            }
        }
    }
}