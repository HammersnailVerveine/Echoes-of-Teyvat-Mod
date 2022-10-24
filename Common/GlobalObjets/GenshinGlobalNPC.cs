using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
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

        public bool ReactionFrozen;
        public Vector2 ReactionFrozenPosition;
        public int ReactionFrozenDirection;

        public static bool CanBefrozen(NPC npc) => npc.knockBackResist > 0f; //|| npc.type == NPCID.TargetDummy;
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

            ProcessReactions(npc);
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
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, type, 0, 0f, Main.myPlayer, (float)element, value);
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
            if (AffectedByElement(GenshinElement.HYDRO) && ReactionFrozen) nbElements--;
            int offSetY = -30;
            int offSetX = 0;
            setOffset(ref offSetX, ref offSetY, ref nbElements);

            if (AffectedByElement(GenshinElement.GEO)) DrawTexture(ElementTexture[0], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(GenshinElement.ANEMO)) DrawTexture(ElementTexture[1], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(GenshinElement.CRYO)) DrawTexture(ElementTexture[2], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(GenshinElement.ELECTRO)) DrawTexture(ElementTexture[3], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(GenshinElement.DENDRO)) DrawTexture(ElementTexture[4], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(GenshinElement.HYDRO) && !ReactionFrozen) DrawTexture(ElementTexture[5], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);
            if (AffectedByElement(GenshinElement.PYRO)) DrawTexture(ElementTexture[6], spriteBatch, npc, nbElements, ref offSetX, ref offSetY);

            if (ReactionFrozen)
            {
                Texture2D texture = TextureAssets.Npc[npc.type].Value;
                Vector2 position = npc.Center - Main.screenPosition;
                Rectangle rect = npc.frame;
                rect.Height -= 2;
                position.Y -= 6;
                SpriteEffects effects = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                spriteBatch.Draw(texture, position, rect, GenshinElementUtils.GetColor(GenshinElement.CRYO) * 0.4f, npc.rotation + 0.05f * npc.direction, rect.Size() * 0.5f, 1.2f, effects, 0f);
                spriteBatch.Draw(texture, position, rect, GenshinElementUtils.GetColor(GenshinElement.CRYO) * 0.4f, npc.rotation - 0.05f * npc.direction, rect.Size() * 0.5f, 1.2f, effects, 0f);
                spriteBatch.Draw(texture, position, rect, GenshinElementUtils.GetColor(GenshinElement.CRYO) * 0.9f, npc.rotation, rect.Size() * 0.5f, 1f, effects, 0f);
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.ModProjectile is GenshinProjectile genshinProjectile)
            {
                GenshinCharacter genshinCharacter = genshinProjectile.OwnerCharacter;
                if (genshinCharacter == null) return;
                GenshinElement element = genshinProjectile.Element;

                damage = genshinCharacter.ApplyDamageMult(damage, element, genshinProjectile.AbilityType);

                if (element == GenshinElement.NONE || !genshinProjectile.CanReact)
                {
                    if (damage > 0) CombatText.NewText(ExtendedHitboxFlat(npc), GenshinElementUtils.GetColor(element), damage, crit);
                    //CombatText.NewText(ExtendedHitbox(npc), Color.PaleVioletRed, genshinProjectile.AbilityType.ToString()); // test
                    return;
                }
                //CombatText.NewText(ExtendedHitbox(npc), Color.SlateBlue, genshinProjectile.AbilityType.ToString()); // test

                ApplyElement(npc, genshinProjectile, genshinCharacter, element, ref damage);

                if (damage > 0) CombatText.NewText(ExtendedHitboxFlat(npc), GenshinElementUtils.GetColor(element), damage, crit);
            }
        }

        public void ApplyElement(NPC npc, GenshinProjectile genshinProjectile, GenshinCharacter genshinCharacter, GenshinElement element, ref int damage)
        {
            if (genshinProjectile.IgnoreICD || genshinCharacter.TryApplyElement(npc))
            {
                int application = genshinProjectile.ElementApplication;
                float mastery = genshinCharacter.StatElementalMastery;
                bool reacted = false; // Individual attacks can only cause 1 reaction

                if (element == GenshinElement.PYRO)
                {

                    if (AffectedByElement(GenshinElement.CRYO) && !reacted) // Melt Strong
                    {
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.MELT);
                        damage = (int)(damage * 2 * (1 + (2.78 * (mastery / (mastery + 1400)) * 1) + reactionBonus));

                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.MELT), "Melt");
                        ElementTimerCryo -= application * 2; // 2x modifier
                        application = 0;
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Vaporize Weak
                    {
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.VAPORIZE);
                        damage = (int)(damage * 1.5 * (1 + (2.78 * (mastery / (mastery + 1400)) * 1) + reactionBonus));

                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.VAPORIZE), "Vaporize");
                        ElementTimerHydro -= (int)(application * 0.5); // 0.5x modifier
                        application = 0;
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.ELECTRO) && !reacted) // Overloaded
                    {
                        Player player = genshinCharacter.Player;
                        int type = ModContent.ProjectileType<Content.Projectiles.ProjectileOverloaded>();
                        Projectile.NewProjectile(player.GetSource_Misc("GenshinMod Elemental Reaction"), npc.Center, Vector2.Zero, type, 0, 15f);
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.OVERLOADED);
                        int reactionDamage = (int)(2 * genshinCharacter.ReactionTransformativeDamage * (1 + ((16 * mastery) / (2000 + mastery)) + reactionBonus));
                        // https://library.keqingmains.com/combat-mechanics/damage/damage-formula#proc
                        foreach (NPC target in Main.npc)
                        {
                            if (GenshinProjectile.CanHomeInto(npc))
                            {
                                if (npc.Center.Distance(target.Center) < 128f)
                                {
                                    player.ApplyDamageToNPC(target, Main.DamageVar(reactionDamage), 15f, player.direction, false);
                                    CombatText.NewText(ExtendedHitboxFlat(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.OVERLOADED), reactionDamage);
                                }
                            }
                        }

                        SoundEngine.PlaySound(SoundID.Item94, npc.Center);
                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.OVERLOADED), "Overloaded");
                        ElementTimerElectro -= application; // 1x modifier.
                        application = 0;
                        reacted = true;
                    }
                }

                if (element == GenshinElement.HYDRO)
                {
                    if (AffectedByElement(GenshinElement.PYRO) && !reacted) // Vaporize Strong
                    {
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.VAPORIZE);
                        damage = (int)(damage * 2 * (1 + (2.78 * (mastery / (mastery + 1400)) * 1) + reactionBonus));

                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.VAPORIZE), "Vaporize");
                        ElementTimerPyro -= application * 2; // 2x modifier
                        application = 0;
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.CRYO) && !reacted) // Frozen
                    {
                        if (!ReactionFrozen && CanBefrozen(npc))
                        {
                            float FactorFrozen = 1f + genshinCharacter.GetReactionBonus(GenshinReaction.FROZEN); // affects freeze duration (multiplies element timer loss), not affercted by EM.
                            int minValue = (int)(MathHelper.Min(application, ElementTimerCryo));
                            ElementTimerCryo = (int)((MathHelper.Max(ElementTimerHydro - minValue, 0) + minValue * 2) * FactorFrozen);
                            application = (int)((MathHelper.Max(application - minValue, 0) + minValue * 2) * FactorFrozen);
                            ReactionFrozen = true;
                            ReactionFrozenDirection = npc.direction;
                            ReactionFrozenPosition = npc.position;
                            CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.FROZEN), "Frozen");
                            reacted = true;
                        }
                    }
                }

                if (element == GenshinElement.CRYO)
                {
                    if (AffectedByElement(GenshinElement.PYRO) && !reacted) // Melt Weak
                    {
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.MELT);
                        damage = (int)(damage * 1.5 * (1 + (2.78 * (mastery / (mastery + 1400)) * 1) + reactionBonus));

                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.MELT), "Melt");
                        ElementTimerPyro -= (int)(application * 0.5); // 0.5x modifier
                        application = 0;
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Frozen
                    {
                        if (!ReactionFrozen && CanBefrozen(npc))
                        {
                            float FactorFrozen = 1f + genshinCharacter.GetReactionBonus(GenshinReaction.FROZEN); // affects freeze duration (multiplies element timer loss), not affercted by EM.
                            int minValue = (int)(MathHelper.Min(application, ElementTimerHydro));
                            ElementTimerHydro = (int)((MathHelper.Max(ElementTimerHydro - minValue, 0) + minValue * 2) * FactorFrozen);
                            application = (int)((MathHelper.Max(application - minValue, 0) + minValue * 2) * FactorFrozen);
                            ReactionFrozen = true;
                            ReactionFrozenDirection = npc.direction;
                            ReactionFrozenPosition = npc.position;
                            CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.FROZEN), "Frozen");
                            reacted = true;
                        }
                    }
                }


                if (element == GenshinElement.ELECTRO)
                {

                    if (AffectedByElement(GenshinElement.PYRO) && !reacted) // Overloaded
                    {
                        Player player = genshinCharacter.Player;
                        int type = ModContent.ProjectileType<Content.Projectiles.ProjectileOverloaded>();
                        Projectile.NewProjectile(player.GetSource_Misc("GenshinMod Elemental Reaction"), npc.Center, Vector2.Zero, type, 0, 15f);
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.OVERLOADED);
                        int reactionDamage = (int)(2 * genshinCharacter.ReactionTransformativeDamage * (1 + ((16 * mastery) / (2000 + mastery)) + reactionBonus)); 
                        // https://library.keqingmains.com/combat-mechanics/damage/damage-formula#proc
                        foreach (NPC target in Main.npc)
                        {
                            if (GenshinProjectile.CanHomeInto(npc))
                            {
                                if (npc.Center.Distance(target.Center) < 128f)
                                {
                                    player.ApplyDamageToNPC(target, Main.DamageVar(reactionDamage), 15f, player.direction, false);
                                    CombatText.NewText(ExtendedHitboxFlat(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.OVERLOADED), reactionDamage);
                                }
                            }
                        }

                        SoundEngine.PlaySound(SoundID.Item94, npc.Center);
                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.OVERLOADED), "Overloaded");
                        ElementTimerPyro -= application; // 1x modifier.
                        application = 0;
                        reacted = true;
                    }
                }
                InflictElement(element, application);
            }
        }

        public void ProcessReactions(NPC npc)
        {
            if (ReactionFrozen) // Frozen
            {
                npc.velocity *= 0f;
                npc.direction = ReactionFrozenDirection;
                npc.position = ReactionFrozenPosition;
                if (ElementTimerHydro <= 0 || ElementTimerCryo <= 0) ReactionFrozen = false;
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


        public static Rectangle ExtendedHitboxFlat(NPC npc)
        {
            Rectangle rect = npc.Hitbox;
            rect.X -= 32;
            rect.Y -= 32;
            rect.Width += 64;
            rect.Height += 64;
            return rect;
        }
        

        public static Rectangle ExtendedHitboxMult(NPC npc)
        {
            Rectangle rect = npc.Hitbox;
            rect.X -= (int)(npc.width / 2);
            rect.Y -= (int)(npc.height / 2);
            rect.Width += npc.width;
            rect.Height += npc.height;
            return rect;
        }

        public static Rectangle ReactionHitbox(NPC npc)
        {
            Rectangle rectangle = ExtendedHitboxFlat(npc);
            rectangle.Y -= 48;
            return rectangle;
        }
    }
}