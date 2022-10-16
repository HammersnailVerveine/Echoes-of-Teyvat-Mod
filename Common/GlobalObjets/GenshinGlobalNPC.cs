using GenshinMod.Common.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Common.GlobalObjets
{
    public class GenshinGlobalNPC : GlobalNPC
    {
        public static Texture2D[] ElementTexture;

        public bool HalfLifeParticle = false;
        public CharacterElement Element = CharacterElement.NONE;

        public int ElementTimerGeo = 0;
        public int ElementTimerAnemo = 0;
        public int ElementTimerCryo = 0;
        public int ElementTimerElectro = 0;
        public int ElementTimerDendro = 0;
        public int ElementTimerHydro = 0;
        public int ElementTimerPyro = 0;

        public override bool InstancePerEntity => true;
        public override void Load()
        {
            ElementTexture = new Texture2D[7];
            ElementTexture[0] = ModContent.Request<Texture2D>("GenshinMod/Content/UI/Textures/Element_Geo", AssetRequestMode.ImmediateLoad).Value;
            ElementTexture[1] = ModContent.Request<Texture2D>("GenshinMod/Content/UI/Textures/Element_Anemo", AssetRequestMode.ImmediateLoad).Value;
            ElementTexture[2] = ModContent.Request<Texture2D>("GenshinMod/Content/UI/Textures/Element_Cryo", AssetRequestMode.ImmediateLoad).Value;
            ElementTexture[3] = ModContent.Request<Texture2D>("GenshinMod/Content/UI/Textures/Element_Electro", AssetRequestMode.ImmediateLoad).Value;
            ElementTexture[4] = ModContent.Request<Texture2D>("GenshinMod/Content/UI/Textures/Element_Dendro", AssetRequestMode.ImmediateLoad).Value;
            ElementTexture[5] = ModContent.Request<Texture2D>("GenshinMod/Content/UI/Textures/Element_Hydro", AssetRequestMode.ImmediateLoad).Value;
            ElementTexture[6] = ModContent.Request<Texture2D>("GenshinMod/Content/UI/Textures/Element_Pyro", AssetRequestMode.ImmediateLoad).Value;
        }

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

        public override void ResetEffects(NPC npc)
        {
            ElementTimerGeo--;
            ElementTimerAnemo--;
            ElementTimerCryo--;
            ElementTimerElectro--;
            ElementTimerDendro--;
            ElementTimerHydro--;
            ElementTimerPyro--;

            ElementTimerGeo = 1;
            ElementTimerAnemo = 1;
            ElementTimerCryo = 1;
            ElementTimerElectro = 1;
            ElementTimerDendro = 1;
            ElementTimerHydro = 1;
            ElementTimerPyro = 1;
        }

        public override void AI(NPC npc)
        {
            if (npc.life <= npc.lifeMax / 2f && !HalfLifeParticle)
            {
                HalfLifeParticle = true;
                if (!Main.LocalPlayer.dead) SpawnElementalParticle(npc, Element, 1f);
            }
        }

        public override void OnKill(NPC npc)
        {
            if (!Main.LocalPlayer.dead) SpawnElementalParticle(npc, Element, 1f);
        }

        public void SpawnElementalParticle(NPC npc, CharacterElement element, float value, int number = 1)
        {
            int type = ModContent.ProjectileType<Content.Projectiles.ProjectileElementalParticle>();
            for (int i = 0; i < number; i++)
            {
                int proj = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, type, 0, 0f, Main.myPlayer, (float)element, value);
            }
        }

        public void InflictElement(CharacterElement Element, int duration)
        {
            switch (Element)
            {
                case CharacterElement.GEO:
                    if (duration > ElementTimerGeo) ElementTimerGeo = duration;
                    break;
                case CharacterElement.ANEMO:
                    if (duration > ElementTimerAnemo) ElementTimerAnemo = duration;
                    break;
                case CharacterElement.CRYO:
                    if (duration > ElementTimerCryo) ElementTimerCryo = duration;
                    break;
                case CharacterElement.ELECTRO:
                    if (duration > ElementTimerElectro) ElementTimerElectro = duration;
                    break;
                case CharacterElement.DENDRO:
                    if (duration > ElementTimerDendro) ElementTimerDendro = duration;
                    break;
                case CharacterElement.HYDRO:
                    if (duration > ElementTimerHydro) ElementTimerHydro = duration;
                    break;
                case CharacterElement.PYRO:
                    if (duration > ElementTimerPyro) ElementTimerPyro = duration;
                    break;
                default:
                    break;
            }
        }

        public bool AffectedByElement(CharacterElement Element)
        {
            switch (Element)
            {
                case CharacterElement.GEO:
                    return ElementTimerGeo > 0;
                case CharacterElement.ANEMO:
                    return ElementTimerAnemo > 0;
                case CharacterElement.CRYO:
                    return ElementTimerCryo > 0;
                case CharacterElement.ELECTRO:
                    return ElementTimerElectro > 0;
                case CharacterElement.DENDRO:
                    return ElementTimerDendro > 0;
                case CharacterElement.HYDRO:
                    return ElementTimerHydro > 0;
                case CharacterElement.PYRO:
                    return ElementTimerPyro > 0;
                default:
                    if (AffectedByElement(CharacterElement.GEO)) return false;
                    if (AffectedByElement(CharacterElement.ANEMO)) return false;
                    if (AffectedByElement(CharacterElement.CRYO)) return false;
                    if (AffectedByElement(CharacterElement.ELECTRO)) return false;
                    if (AffectedByElement(CharacterElement.DENDRO)) return false;
                    if (AffectedByElement(CharacterElement.HYDRO)) return false;
                    if (AffectedByElement(CharacterElement.PYRO)) return false;
                    return true;
            }
        }

        public void setOffset(ref int offSetX, ref int offSetY, ref int nbElements)
        {
            offSetX = 0;
            while (nbElements > 0 && offSetX > -28)
            {
                nbElements--;
                offSetX -= 14;
            }
            offSetY -= 24;
        }

        public void DrawTexture(Texture2D texture, SpriteBatch spriteBatch, NPC npc, int nbElements,ref int offSetX, ref int offSetY)
        {
            Vector2 position = new Vector2(npc.Center.X + offSetX - Main.screenPosition.X, npc.Center.Y + offSetY - Main.screenPosition.Y);
            spriteBatch.Draw(texture, position, null, Color.White, 0f, texture.Size() * 0.5f, 0.875f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position, null, Color.White * 0.5f, 0f, texture.Size() * 0.5f, 1.025f, SpriteEffects.None, 0f);
            offSetX += 24;
            if (offSetX > 24) setOffset(ref offSetX, ref offSetY, ref nbElements);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int nbElements = -1;
            if (AffectedByElement(CharacterElement.GEO)) nbElements++;
            if (AffectedByElement(CharacterElement.ANEMO)) nbElements++;
            if (AffectedByElement(CharacterElement.CRYO)) nbElements++;
            if (AffectedByElement(CharacterElement.ELECTRO)) nbElements++;
            if (AffectedByElement(CharacterElement.DENDRO)) nbElements++;
            if (AffectedByElement(CharacterElement.HYDRO)) nbElements++;
            if (AffectedByElement(CharacterElement.PYRO)) nbElements++;
            int offSetY = -30;
            int offSetX = -11;
            setOffset(ref offSetX, ref offSetY, ref nbElements);

            if (AffectedByElement(CharacterElement.GEO)) DrawTexture(ElementTexture[0], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(CharacterElement.ANEMO)) DrawTexture(ElementTexture[1], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(CharacterElement.CRYO)) DrawTexture(ElementTexture[2], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(CharacterElement.ELECTRO)) DrawTexture(ElementTexture[3], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(CharacterElement.DENDRO)) DrawTexture(ElementTexture[4], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(CharacterElement.HYDRO)) DrawTexture(ElementTexture[5], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(CharacterElement.PYRO)) DrawTexture(ElementTexture[6], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
        }
    }
}