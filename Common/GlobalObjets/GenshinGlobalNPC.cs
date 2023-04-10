using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
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
        public bool BluntTarget = false; // is the target more susceptible to heavy attacks ?
        public int Level = 1;

        public int TimerElementGeo = 0;
        public int TimerElementAnemo = 0;
        public int TimerElementCryo = 0;
        public int TimerElementElectro = 0;
        public int TimerElementDendro = 0;
        public int TimerElementHydro = 0;
        public int TimerElementPyro = 0;

        public bool ReactionFrozen;
        public Vector2 ReactionFrozenPosition;
        public int ReactionFrozenDirection;

        public int ReactionSuperconduct = 0;
        public int ReactionElectrocharged = 0;
        public int ReactionElectrochargedPlayer = 0;
        public int ReactionElectrochargedDamage = 0;

        public int TimerReactionOverloaded = 0; // overloaded damage ICD
        public int TimerReactionSuperconduct = 0; // superconduct damage ICD
        public int TimerReactionElectrocharged = 0; // electrocharged damage ICD

        public float ResistanceGeo = 0.1f; // 0f = 100% damage taken, 1f = immune
        public float ResistanceAnemo = 0.1f;
        public float ResistanceCryo = 0.1f;
        public float ResistanceElectro = 0.1f;
        public float ResistanceDendro = 0.1f;
        public float ResistanceHydro = 0.1f;
        public float ResistancePyro = 0.1f;
        public float ResistancePhysical = 0.1f;

        public float ReductionDefense = 0f; // Add to deal more damage with all attacks
        public float ReductionResistanceGeo = 0f; // Add to deal more damage with X element
        public float ReductionResistanceAnemo = 0f;
        public float ReductionResistanceCryo = 0f;
        public float ReductionResistanceElectro = 0f;
        public float ReductionResistanceDendro = 0f;
        public float ReductionResistanceHydro = 0f;
        public float ReductionResistancePyro = 0f;
        public float ReductionResistancePhysical = 0f;

        public int ElementSymbolDrawOffset = 0; // used to offset the drawing of the element symbols above specific enemies, may be used for some bosses

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
            npc.value = 0;

            if (npc.type == NPCID.ArmoredViking) ResistanceCryo = 1f;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            npcLoot.RemoveWhere(rule => 1 == 1);
        }

        public override void ResetEffects(NPC npc)
        {
            TimerElementGeo -= 60;
            TimerElementAnemo -= 60;
            TimerElementCryo--;
            TimerElementElectro--;
            TimerElementDendro--;
            TimerElementHydro--;
            TimerElementPyro--;
            TimerReactionOverloaded--;
            TimerReactionSuperconduct--;
            TimerReactionElectrocharged--;
            ReactionElectrocharged--;

            ReductionDefense = 0f;
            ReductionResistanceGeo = 0f;
            ReductionResistanceAnemo = 0f;
            ReductionResistanceCryo = 0f;
            ReductionResistanceElectro = 0f;
            ReductionResistanceDendro = 0f;
            ReductionResistanceHydro = 0f;
            ReductionResistancePyro = 0f;
            ReductionResistancePhysical = 0f;
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

        public int ApplyDefense(int damage, float defIgnore, int characterLevel) => (int)(GetDefenseMult(defIgnore, characterLevel) * damage);
        public int ApplyResistance(int damage, GenshinElement element) => (int)(GetResistanceMult(element) * damage);

        public float GetDefenseMult(float defIgnore, int characterLevel)
        {
            float reductionDefense = ReductionDefense < 0.9f ? ReductionDefense : 0.9f;
            return (characterLevel + 100) / (characterLevel + 100 + (Level + 100) * (1 - reductionDefense) * (1 - defIgnore));
        }

        public float GetResistanceMult(GenshinElement element)
        {
            float mult;

            switch(element)
            {
                case GenshinElement.GEO:
                    if (ResistanceGeo >= 1f) return 0f; 
                    mult = ResistanceGeo - ReductionResistanceGeo;
                    break;
                case GenshinElement.ANEMO:
                    if (ResistanceAnemo >= 1f) return 0f;
                    mult = ResistanceAnemo - ReductionResistanceAnemo;
                    break;
                case GenshinElement.CRYO:
                    if (ResistanceCryo >= 1f) return 0f;
                    mult = ResistanceCryo - ReductionResistanceCryo;
                    break;
                case GenshinElement.DENDRO:
                    if (ResistanceDendro >= 1f) return 0f;
                    mult = ResistanceDendro - ReductionResistanceDendro;
                    break;
                case GenshinElement.ELECTRO:
                    if (ResistanceElectro >= 1f) return 0f;
                    mult = ResistanceElectro - ReductionResistanceElectro;
                    break;
                case GenshinElement.HYDRO:
                    if (ResistanceHydro >= 1f) return 0f;
                    mult = ResistanceHydro - ReductionResistanceHydro;
                    break;
                case GenshinElement.PYRO:
                    if (ResistancePyro >= 1f) return 0f;
                    mult = ResistancePyro - ReductionResistancePyro;
                    break;
                default: // Physical
                    if (ResistancePhysical >= 1f) return 0f;
                    mult = ResistancePhysical - ReductionResistancePhysical;
                    break;
            }

            if (mult < 0f) mult *= 0.5f;
            return 1f - mult;
        }

        public bool AffectedByElement(GenshinElement Element)
        {
            switch (Element)
            {
                case GenshinElement.GEO:
                    return TimerElementGeo > 0;
                case GenshinElement.ANEMO:
                    return TimerElementAnemo > 0;
                case GenshinElement.CRYO:
                    return TimerElementCryo > 0;
                case GenshinElement.ELECTRO:
                    return TimerElementElectro > 0;
                case GenshinElement.DENDRO:
                    return TimerElementDendro > 0;
                case GenshinElement.HYDRO:
                    return TimerElementHydro > 0;
                case GenshinElement.PYRO:
                    return TimerElementPyro > 0;
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

        public void DrawTexture(Texture2D texture, SpriteBatch spriteBatch, NPC npc, int nbElements,ref int offSetX, ref int offSetY, int timeLeft)
        {
            float colorMult = timeLeft > 120 ? 1f : (float)Math.Abs(Math.Sin((timeLeft * 0.5f)/Math.PI/4f));
            Vector2 position = new Vector2(npc.Center.X + offSetX - Main.screenPosition.X, npc.Center.Y + offSetY - Main.screenPosition.Y);
            spriteBatch.Draw(texture, position, null, Color.White * colorMult, 0f, texture.Size() * 0.5f, 0.875f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position, null, Color.White * 0.5f * colorMult, 0f, texture.Size() * 0.5f, 1.025f, SpriteEffects.None, 0f);
            offSetX += 24;
            if (offSetX > 24) setOffset(ref offSetX, ref offSetY, ref nbElements);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int nbElements = -1;
            foreach (GenshinElement element in System.Enum.GetValues(typeof(GenshinElement)))
                if (element != GenshinElement.NONE) if (AffectedByElement(element)) nbElements++;
            if (AffectedByElement(GenshinElement.HYDRO) && ReactionFrozen) nbElements--;
            int offSetY = - (30 + ElementSymbolDrawOffset);
            int offSetX = 0;
            setOffset(ref offSetX, ref offSetY, ref nbElements);

            if (AffectedByElement(GenshinElement.GEO)) DrawTexture(ElementTexture[0], spriteBatch, npc, nbElements, ref offSetX, ref offSetY, TimerElementGeo);
            if (AffectedByElement(GenshinElement.ANEMO)) DrawTexture(ElementTexture[1], spriteBatch, npc, nbElements, ref offSetX, ref offSetY, TimerElementAnemo);
            if (AffectedByElement(GenshinElement.CRYO)) DrawTexture(ElementTexture[2], spriteBatch, npc, nbElements, ref offSetX, ref offSetY, TimerElementCryo);
            if (AffectedByElement(GenshinElement.ELECTRO)) DrawTexture(ElementTexture[3], spriteBatch, npc, nbElements, ref offSetX, ref offSetY, TimerElementElectro);
            if (AffectedByElement(GenshinElement.DENDRO)) DrawTexture(ElementTexture[4], spriteBatch, npc, nbElements, ref offSetX, ref offSetY, TimerElementDendro);
            if (AffectedByElement(GenshinElement.HYDRO) && !ReactionFrozen) DrawTexture(ElementTexture[5], spriteBatch, npc, nbElements, ref offSetX, ref offSetY, TimerElementHydro);
            if (AffectedByElement(GenshinElement.PYRO)) DrawTexture(ElementTexture[6], spriteBatch, npc, nbElements, ref offSetX, ref offSetY, TimerElementPyro);

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

                if (element != GenshinElement.NONE && genshinProjectile.CanReact) ApplyElement(npc, genshinProjectile, genshinCharacter, element, ref damage);
                damage = ApplyResistance(damage, element);
                damage = ApplyDefense(damage, genshinProjectile.DefenseIgnore, genshinCharacter.Level);

                if (BluntTarget)
                {
                    switch (genshinProjectile.AttackWeight)
                    {
                        case AttackWeight.LIGHT:
                            damage = 0;
                            break;
                        case AttackWeight.BLUNT:
                            damage *= 5;
                            break;
                        default:
                            break;
                    }
                }

                if (damage > 0) CombatText.NewText(ExtendedHitboxFlat(npc), GenshinElementUtils.GetColor(element), damage, crit);
                else if (genshinProjectile.CanDealDamage)
                {
                    CombatText.NewText(ExtendedHitboxFlat(npc), GenshinElementUtils.ColorImmune, "Immune");
                    npc.life++;
                }
            }
        }

        public void ApplyElement(NPC npc, GenshinProjectile genshinProjectile, GenshinCharacter genshinCharacter, GenshinElement element, ref int damage)
        {
            if (genshinProjectile.IgnoreICD || genshinCharacter.TryApplyElement(npc))
            {
                int application = genshinProjectile.ElementApplication;
                float mastery = genshinCharacter.StatElementalMastery;
                bool reacted = false; // Individual attacks can only cause 1 reaction
                GenshinElement crystallizeElement = GenshinElement.NONE;

                if (element == GenshinElement.PYRO)
                {

                    if (AffectedByElement(GenshinElement.CRYO) && !reacted) // Melt Strong
                    {
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.MELT);
                        damage = (int)(damage * 2 * (1 + (2.78 * (mastery / (mastery + 1400)) * 1) + reactionBonus));

                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.MELT), "Melt");
                        TimerElementCryo -= application * 2; // 2x modifier
                        application = 0;
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.ELECTRO) && !reacted) // Overloaded
                    {
                        Player player = genshinCharacter.Player;
                        int type = ModContent.ProjectileType<Content.Projectiles.ProjectileOverloaded>();
                        Projectile.NewProjectile(player.GetSource_Misc("GenshinMod Elemental Reaction"), npc.Center, Vector2.Zero, type, 0, 15f);
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.OVERLOADED);
                        int reactionDamage = (int)(2f * genshinCharacter.ReactionTransformativeDamage * (1f + ((16f * mastery) / (2000f + mastery)) + reactionBonus));
                        foreach (NPC target in Main.npc)
                        {
                            if (GenshinProjectile.CanHomeInto(target))
                            {
                                if (npc.Center.Distance(target.Center) < 128f)
                                {
                                    GenshinGlobalNPC genshinNPC = target.GetGlobalNPC<GenshinGlobalNPC>();
                                    if (genshinNPC.TimerReactionOverloaded <= 0)
                                    {
                                        int targetDamage = genshinNPC.ApplyResistance(reactionDamage, GenshinElement.PYRO);
                                        if (targetDamage > 0)
                                        {
                                            player.ApplyDamageToNPC(target, targetDamage, 15f, -player.direction, false);
                                            CombatText.NewText(ExtendedHitboxFlat(target), GenshinElementUtils.GetReactionColor(GenshinReaction.OVERLOADED), targetDamage);
                                        }
                                        else CombatText.NewText(ExtendedHitboxFlat(target), GenshinElementUtils.ColorImmune, "Immune");
                                        genshinNPC.TimerReactionOverloaded = 30; // Reaction damage icd.
                                    }
                                }
                            }
                        }

                        SoundEngine.PlaySound(SoundID.Item94, npc.Center);
                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.OVERLOADED), "Overloaded");
                        TimerElementElectro -= application; // 1x modifier.
                        application = 0;
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Vaporize Weak
                    {
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.VAPORIZE);
                        damage = (int)(damage * 1.5 * (1 + (2.78 * (mastery / (mastery + 1400)) * 1) + reactionBonus));

                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.VAPORIZE), "Vaporize");
                        TimerElementHydro -= (int)(application * 0.5); // 0.5x modifier
                        application = 0;
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.GEO) && !reacted) // Pyro Crystallize
                    {
                        crystallizeElement = GenshinElement.PYRO;
                        TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                    }
                }

                if (element == GenshinElement.HYDRO)
                {
                    if (AffectedByElement(GenshinElement.PYRO) && !reacted) // Vaporize Strong
                    {
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.VAPORIZE);
                        damage = (int)(damage * 2 * (1 + (2.78 * (mastery / (mastery + 1400)) * 1) + reactionBonus));

                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.VAPORIZE), "Vaporize");
                        TimerElementPyro -= application * 2; // 2x modifier
                        application = 0;
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.CRYO) && !reacted) // Frozen
                    {
                        if (!ReactionFrozen && CanBefrozen(npc))
                        {
                            float FactorFrozen = 1f + genshinCharacter.GetReactionBonus(GenshinReaction.FROZEN); // affects freeze duration (multiplies element timer loss), not affercted by EM.
                            int minValue = (int)(MathHelper.Min(application, TimerElementCryo));
                            TimerElementCryo = (int)((MathHelper.Max(TimerElementHydro - minValue, 0) + minValue * 2) * FactorFrozen);
                            application = (int)((MathHelper.Max(application - minValue, 0) + minValue * 2) * FactorFrozen);
                            ReactionFrozen = true;
                            ReactionFrozenDirection = npc.direction;
                            ReactionFrozenPosition = npc.position;
                            CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.FROZEN), "Frozen");
                            reacted = true;
                        }
                    }

                    if (AffectedByElement(GenshinElement.ELECTRO) && !reacted) // Electro-Charged
                    {
                        ReactionElectrocharged = 1;
                        ReactionElectrochargedPlayer = genshinCharacter.Player.whoAmI;
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.ELECTROCHARGED);
                        ReactionElectrochargedDamage = (int)(1.2f * genshinCharacter.ReactionTransformativeDamage * (1f + ((16f * mastery) / (2000f + mastery)) + reactionBonus));
                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetColor(GenshinElement.ELECTRO), "Electro-Charged");
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.GEO) && !reacted) // Hydro Crystallize
                    {
                        crystallizeElement = GenshinElement.HYDRO;
                        TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                    }
                }

                if (element == GenshinElement.CRYO)
                {
                    if (AffectedByElement(GenshinElement.PYRO) && !reacted) // Melt Weak
                    {
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.MELT);
                        damage = (int)(damage * 1.5 * (1 + (2.78 * (mastery / (mastery + 1400)) * 1) + reactionBonus));

                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.MELT), "Melt");
                        TimerElementPyro -= (int)(application * 0.5); // 0.5x modifier
                        application = 0;
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.ELECTRO) && !reacted) // Superconduct
                    {
                        Player player = genshinCharacter.Player;
                        int type = ModContent.ProjectileType<Content.Projectiles.ProjectileSuperconduct>();
                        Projectile.NewProjectile(player.GetSource_Misc("GenshinMod Elemental Reaction"), npc.Center, Vector2.Zero, type, 0, 15f);
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.SUPERCONDUCT);
                        int reactionDamage = (int)(0.5f * genshinCharacter.ReactionTransformativeDamage * (1f + ((16f * mastery) / (2000f + mastery)) + reactionBonus));
                        foreach (NPC target in Main.npc)
                        {
                            if (GenshinProjectile.CanHomeInto(target))
                            {
                                if (npc.Center.Distance(target.Center) < 128f)
                                {
                                    GenshinGlobalNPC genshinNPC = target.GetGlobalNPC<GenshinGlobalNPC>();
                                    if (genshinNPC.TimerReactionSuperconduct <= 0)
                                    {
                                        int targetDamage = genshinNPC.ApplyResistance(reactionDamage, GenshinElement.CRYO);
                                        if (targetDamage > 0)
                                        {
                                            player.ApplyDamageToNPC(target, targetDamage, 0.5f, -player.direction, false);
                                            CombatText.NewText(ExtendedHitboxFlat(target), GenshinElementUtils.GetReactionColor(GenshinReaction.SUPERCONDUCT), targetDamage);
                                        }
                                        else CombatText.NewText(ExtendedHitboxFlat(target), GenshinElementUtils.ColorImmune, "Immune");
                                        genshinNPC.TimerReactionSuperconduct = 30; // Reaction damage icd.
                                        genshinNPC.ReactionSuperconduct = 60 * 12; // Phys red debuff 12 sec.
                                    }
                                }
                            }
                        }

                        SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, npc.Center);
                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.SUPERCONDUCT), "Superconduct");
                        TimerElementElectro -= application; // 1x modifier.
                        application = 0;
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Frozen
                    {
                        if (!ReactionFrozen && CanBefrozen(npc))
                        {
                            float FactorFrozen = 1f + genshinCharacter.GetReactionBonus(GenshinReaction.FROZEN); // affects freeze duration (multiplies element timer loss), not affercted by EM.
                            int minValue = (int)(MathHelper.Min(application, TimerElementHydro));
                            TimerElementHydro = (int)((MathHelper.Max(TimerElementHydro - minValue, 0) + minValue * 2) * FactorFrozen);
                            application = (int)((MathHelper.Max(application - minValue, 0) + minValue * 2) * FactorFrozen);
                            ReactionFrozen = true;
                            ReactionFrozenDirection = npc.direction;
                            ReactionFrozenPosition = npc.position;
                            CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.FROZEN), "Frozen");
                            reacted = true;
                        }
                    }

                    if (AffectedByElement(GenshinElement.GEO) && !reacted) // Cryo Crystallize
                    {
                        crystallizeElement = GenshinElement.CRYO;
                        TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
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
                        int reactionDamage = (int)(2f * genshinCharacter.ReactionTransformativeDamage * (1f + ((16f * mastery) / (2000f + mastery)) + reactionBonus));
                        foreach (NPC target in Main.npc)
                        {
                            if (GenshinProjectile.CanHomeInto(target))
                            {
                                if (npc.Center.Distance(target.Center) < 128f)
                                {
                                    GenshinGlobalNPC genshinNPC = target.GetGlobalNPC<GenshinGlobalNPC>();
                                    if (genshinNPC.TimerReactionOverloaded <= 0)
                                    {
                                        int targetDamage = genshinNPC.ApplyResistance(reactionDamage, GenshinElement.PYRO);
                                        if (targetDamage > 0)
                                        {
                                            player.ApplyDamageToNPC(target, targetDamage, 15f, -player.direction, false);
                                            CombatText.NewText(ExtendedHitboxFlat(target), GenshinElementUtils.GetReactionColor(GenshinReaction.OVERLOADED), targetDamage);
                                        }
                                        else CombatText.NewText(ExtendedHitboxFlat(target), GenshinElementUtils.ColorImmune, "Immune");
                                        genshinNPC.TimerReactionOverloaded = 30; // Reaction damage icd.
                                    }
                                }
                            }
                        }

                        SoundEngine.PlaySound(SoundID.Item94, npc.Center);
                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.OVERLOADED), "Overloaded");
                        TimerElementPyro -= application; // 1x modifier.
                        application = 0;
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.CRYO) && !reacted) // Superconduct
                    {
                        Player player = genshinCharacter.Player;
                        int type = ModContent.ProjectileType<Content.Projectiles.ProjectileSuperconduct>();
                        Projectile.NewProjectile(player.GetSource_Misc("GenshinMod Elemental Reaction"), npc.Center, Vector2.Zero, type, 0, 15f);
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.SUPERCONDUCT);
                        int reactionDamage = (int)(0.5f * genshinCharacter.ReactionTransformativeDamage * (1f + ((16f * mastery) / (2000f + mastery)) + reactionBonus));
                        foreach (NPC target in Main.npc)
                        {
                            if (GenshinProjectile.CanHomeInto(target))
                            {
                                if (npc.Center.Distance(target.Center) < 128f)
                                {
                                    GenshinGlobalNPC genshinNPC = target.GetGlobalNPC<GenshinGlobalNPC>();
                                    if (genshinNPC.TimerReactionSuperconduct <= 0)
                                    {
                                        int targetDamage = genshinNPC.ApplyResistance(reactionDamage, GenshinElement.CRYO);
                                        if (targetDamage > 0)
                                        {
                                            player.ApplyDamageToNPC(target, targetDamage, 0.5f, -player.direction, false);
                                            CombatText.NewText(ExtendedHitboxFlat(target), GenshinElementUtils.GetReactionColor(GenshinReaction.SUPERCONDUCT), targetDamage);
                                        }
                                        else CombatText.NewText(ExtendedHitboxFlat(target), GenshinElementUtils.ColorImmune, "Immune");
                                        genshinNPC.TimerReactionSuperconduct = 30; // Reaction damage icd.
                                        genshinNPC.ReactionSuperconduct = 60 * 12; // Phys red debuff 12 sec.
                                    }
                                }
                            }
                        }

                        SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, npc.Center);
                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.SUPERCONDUCT), "Superconduct");
                        TimerElementCryo -= application; // 1x modifier.
                        application = 0;
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Electro-Charged
                    {
                        ReactionElectrocharged = 1;
                        ReactionElectrochargedPlayer = genshinCharacter.Player.whoAmI;
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.ELECTROCHARGED);
                        ReactionElectrochargedDamage = (int)(1.2f * genshinCharacter.ReactionTransformativeDamage * (1f + ((16f * mastery) / (2000f + mastery)) + reactionBonus));
                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetColor(GenshinElement.ELECTRO), "Electro-Charged");
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.GEO) && !reacted) // Electro Crystallize
                    {
                        crystallizeElement = GenshinElement.ELECTRO;
                        TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                    }
                }

                if (element == GenshinElement.GEO)
                {
                    if (AffectedByElement(GenshinElement.PYRO) && !reacted) // Pyro Crystallize
                    {
                        crystallizeElement = GenshinElement.PYRO;
                        TimerElementPyro -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
                    }
                    if (AffectedByElement(GenshinElement.CRYO) && !reacted) // Cryo Crystallize
                    {
                        crystallizeElement = GenshinElement.CRYO;
                        TimerElementCryo -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
                    }
                    if (AffectedByElement(GenshinElement.ELECTRO) && !reacted) // Electro Crystallize
                    {
                        crystallizeElement = GenshinElement.ELECTRO;
                        TimerElementElectro -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
                    }
                    if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Hydro Crystallize
                    {
                        crystallizeElement = GenshinElement.HYDRO;
                        TimerElementHydro -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
                    }
                }

                if (crystallizeElement != GenshinElement.NONE)
                {
                    // Might be improved : sets the timeleft of every crystal except the latest 3 (counting the one that's going to spawn right after) to 2 seconds.
                    Projectile first = null;
                    Projectile second = null;

                    foreach (Projectile projectile in Main.projectile)
                    { 
                        if (projectile.ModProjectile is Content.Projectiles.ProjectileCrystallize crystal)
                        {
                            if (first == null)
                                first = projectile;
                            else if (second == null)
                            {
                                if (first.timeLeft > projectile.timeLeft)
                                    second = projectile;
                                else
                                {
                                    second = first;
                                    first = projectile;
                                }
                            }
                            else if (projectile.timeLeft > second.timeLeft)
                            {
                                if (projectile.timeLeft > first.timeLeft)
                                {
                                    second = first;
                                    first = projectile;
                                }
                                else
                                    second = projectile;
                            }
                        }
                    }

                    if (second != null)
                    {
                        foreach (Projectile projectile in Main.projectile)
                        {
                            if (projectile != first && projectile != second && projectile.timeLeft > 120)
                                projectile.timeLeft = 120;
                        }
                    }

                    int type = ModContent.ProjectileType<Content.Projectiles.ProjectileCrystallize>();
                    CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.CRYSTALLIZE), "Crystallize");

                    Vector2 velocity = genshinProjectile.Owner.Center - npc.Center;
                    velocity.Normalize();
                    velocity *= Main.rand.NextFloat(4) + 5f;
                    velocity.Y = -Main.rand.NextFloat(2) - 2f;
                    float shieldValue = 200f * genshinCharacter.Level * (1f + (4.44f * (mastery / (mastery + 1400))));
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity, type, 0, 0f, Main.myPlayer, (float)crystallizeElement, shieldValue);
                    application = 0;
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
                if (TimerElementHydro <= 0 || TimerElementCryo <= 0) ReactionFrozen = false;
            }

            if (ReactionSuperconduct > 0)
            {
                ReactionSuperconduct--;
                ReductionResistancePhysical += 0.4f;
            }

            if (ReactionElectrocharged <= 0 && ReactionElectrochargedDamage > 0)
            {
                if (AffectedByElement(GenshinElement.HYDRO) && AffectedByElement(GenshinElement.ELECTRO))
                {
                    Player player = Main.player[ReactionElectrochargedPlayer];
                    SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, npc.Center);

                    foreach (NPC target in Main.npc)
                    {
                        if (GenshinProjectile.CanHomeInto(target))
                        {
                            if (npc.Center.Distance(target.Center) < 128f)
                            {
                                GenshinGlobalNPC genshinNPC = target.GetGlobalNPC<GenshinGlobalNPC>();
                                if (genshinNPC.AffectedByElement(GenshinElement.HYDRO))
                                {
                                    if (genshinNPC.TimerReactionElectrocharged <= 0)
                                    {
                                        int targetDamage = genshinNPC.ApplyResistance(ReactionElectrochargedDamage, GenshinElement.ELECTRO);
                                        if (targetDamage > 0)
                                        {
                                            player.ApplyDamageToNPC(target, targetDamage, 0f, player.direction, false);
                                            CombatText.NewText(ExtendedHitboxFlat(target), GenshinElementUtils.GetColor(GenshinElement.ELECTRO), targetDamage);
                                        }
                                        else CombatText.NewText(ExtendedHitboxFlat(target), GenshinElementUtils.ColorImmune, "Immune");
                                    }
                                    genshinNPC.TimerReactionElectrocharged = 30;
                                    genshinNPC.ReactionElectrocharged = 60;
                                    genshinNPC.TimerElementHydro -= 240;
                                    genshinNPC.TimerElementElectro -= 240;
                                }
                            }
                        }
                    }
                }
                else
                {
                    ReactionElectrochargedDamage = 0;
                    ReactionElectrochargedPlayer = 0;
                }
            }
        }

        public void InflictElement(GenshinElement Element, int duration)
        {
            switch (Element)
            {
                case GenshinElement.GEO:
                    if (duration > TimerElementGeo) TimerElementGeo = duration;
                    break;
                case GenshinElement.ANEMO:
                    if (duration > TimerElementAnemo) TimerElementAnemo = duration;
                    break;
                case GenshinElement.CRYO:
                    if (duration > TimerElementCryo) TimerElementCryo = duration;
                    break;
                case GenshinElement.ELECTRO:
                    if (duration > TimerElementElectro) TimerElementElectro = duration;
                    break;
                case GenshinElement.DENDRO:
                    if (duration > TimerElementDendro) TimerElementDendro = duration;
                    break;
                case GenshinElement.HYDRO:
                    if (duration > TimerElementHydro) TimerElementHydro = duration;
                    break;
                case GenshinElement.PYRO:
                    if (duration > TimerElementPyro) TimerElementPyro = duration;
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
            rectangle.Y -= 80;
            rectangle.Height = 32;
            return rectangle;
        }
    }
}