using GenshinMod.Common.GameObjects;
using GenshinMod.Common.ModObjects;
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
        public GenshinElement Element = GenshinElement.NONE;

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

        public void SpawnElementalParticle(NPC npc, GenshinElement element, float value, int number = 1)
        {
            int type = ModContent.ProjectileType<Content.Projectiles.ProjectileElementalParticle>();
            for (int i = 0; i < number; i++)
            {
                int proj = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, type, 0, 0f, Main.myPlayer, (float)element, value);
            }
        }

        public void InflictElement(GenshinElement Element, int duration)
        {
            switch (Element)
            {
                case GenshinElement.GEO:
                    if (duration > ElementTimerGeo) ElementTimerGeo = duration;
                    break;
                case GenshinElement.ANEMO:
                    if (duration > ElementTimerAnemo) ElementTimerAnemo = duration;
                    break;
                case GenshinElement.CRYO:
                    if (duration > ElementTimerCryo) ElementTimerCryo = duration;
                    break;
                case GenshinElement.ELECTRO:
                    if (duration > ElementTimerElectro) ElementTimerElectro = duration;
                    break;
                case GenshinElement.DENDRO:
                    if (duration > ElementTimerDendro) ElementTimerDendro = duration;
                    break;
                case GenshinElement.HYDRO:
                    if (duration > ElementTimerHydro) ElementTimerHydro = duration;
                    break;
                case GenshinElement.PYRO:
                    if (duration > ElementTimerPyro) ElementTimerPyro = duration;
                    break;
                default:
                    break;
            }
        }

        public bool AffectedByElement(GenshinElement Element)
        {
            switch (Element)
            {
                case GenshinElement.GEO:
                    return ElementTimerGeo > 0;
                case GenshinElement.ANEMO:
                    return ElementTimerAnemo > 0;
                case GenshinElement.CRYO:
                    return ElementTimerCryo > 0;
                case GenshinElement.ELECTRO:
                    return ElementTimerElectro > 0;
                case GenshinElement.DENDRO:
                    return ElementTimerDendro > 0;
                case GenshinElement.HYDRO:
                    return ElementTimerHydro > 0;
                case GenshinElement.PYRO:
                    return ElementTimerPyro > 0;
                default:
                    foreach (GenshinElement element in System.Enum.GetValues(typeof(GenshinElement)))
                        if (element != GenshinElement.NONE) if (AffectedByElement(element)) return false;
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
            foreach (GenshinElement element in System.Enum.GetValues(typeof(GenshinElement)))
                if (element != GenshinElement.NONE) if (AffectedByElement(element)) nbElements++;
            int offSetY = -30;
            int offSetX = 0;
            setOffset(ref offSetX, ref offSetY, ref nbElements);

            if (AffectedByElement(GenshinElement.GEO)) DrawTexture(ElementTexture[0], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(GenshinElement.ANEMO)) DrawTexture(ElementTexture[1], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(GenshinElement.CRYO)) DrawTexture(ElementTexture[2], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(GenshinElement.ELECTRO)) DrawTexture(ElementTexture[3], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(GenshinElement.DENDRO)) DrawTexture(ElementTexture[4], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(GenshinElement.HYDRO)) DrawTexture(ElementTexture[5], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(GenshinElement.PYRO)) DrawTexture(ElementTexture[6], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.ModProjectile is GenshinProjectile genshinProjectile)
            {
                GenshinElement element = genshinProjectile.Element;
                if (element == GenshinElement.NONE || !genshinProjectile.CanReact)
                {
                    CombatText.NewText(npc.Hitbox, GenshinElementUtils.GetColor(element), damage, crit);
                    return;
                }

                GenshinCharacter genshinCharacter = genshinProjectile.OwnerCharacter;

                if (genshinProjectile.IgnoreICD || genshinCharacter.TryApplyElement(npc))
                {
                    int application = genshinProjectile.ElementApplication;
                    int mastery = genshinCharacter.StatElementalMastery;

                    if (element == GenshinElement.PYRO)
                    {
                        if (AffectedByElement(GenshinElement.HYDRO)) // Vaporize Weak
                        {
                            ElementTimerHydro -= (int)(application * 0.5);
                            damage = (int)(damage * 1.5 * (1 + (2.78 * (mastery / (mastery + 1400)) * 1)));
                            application = 0;

                            Rectangle rectangle = npc.Hitbox;
                            rectangle.Y -= 48;
                            CombatText.NewText(rectangle, GenshinElementUtils.GetReactionColor(GenshinReaction.VAPORIZE), "Vaporize");
                        }
                    }

                    if (element == GenshinElement.HYDRO)
                    {
                        if (AffectedByElement(GenshinElement.PYRO)) // Vaporize Strong
                        {
                            ElementTimerPyro -= application * 2;
                            damage = (int)(damage * 2 * (1 + (2.78 * (mastery / (mastery + 1400)) * 1)));
                            //damage = (int)(damage * 2 * (1 + (2.78 * (mastery / (mastery + 1400)) * 1) + reactionDMGBonus));
                            application = 0;

                            Rectangle rectangle = npc.Hitbox;
                            rectangle.Y -= 48;
                            CombatText.NewText(rectangle, GenshinElementUtils.GetReactionColor(GenshinReaction.VAPORIZE), "Vaporize");
                        }
                    }

                    InflictElement(element, application);
                }
                CombatText.NewText(npc.Hitbox, GenshinElementUtils.GetColor(element), damage, crit);
            }
        }
    }
}