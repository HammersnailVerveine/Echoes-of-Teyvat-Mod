using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Common.GlobalObjets
{
    public class GenshinGlobalNPC : GlobalNPC
    {
        // Misc

        public List<GenshinShieldNPC> Shields;
        public float BaseKnockBackResist = 1f;

        // Not reset every frame

        public GenshinElement Element = GenshinElement.NONE;
        public int Level = 1; // Enemy level (1-10)

        public int ElementSymbolDrawOffset = 0; // used to offset the drawing of the element symbols above specific enemies, may be used for some bosses

        public bool BluntTarget = false; // is the target more susceptible to heavy attacks ?
        public bool GiveEnergyParticlesLife = true; // should the NPC release particles at half and 0 health
        public bool GiveEnergyParticlesHit = true; // should the NPC release particles when hit by projectiles

        public float ResistanceGeo = 0.1f; // 0f = 100% damage taken, 1f = immune
        public float ResistanceAnemo = 0.1f; // 0f = 100% damage taken, 1f = immune
        public float ResistanceCryo = 0.1f; // 0f = 100% damage taken, 1f = immune
        public float ResistanceElectro = 0.1f; // 0f = 100% damage taken, 1f = immune 
        public float ResistanceDendro = 0.1f; // 0f = 100% damage taken, 1f = immune
        public float ResistanceHydro = 0.1f; // 0f = 100% damage taken, 1f = immune
        public float ResistancePyro = 0.1f; // 0f = 100% damage taken, 1f = immune
        public float ResistancePhysical = 0.1f; // 0f = 100% damage taken, 1f = immune

        // Reset variables

        public float ReductionDefense = 0f; // Add to deal more damage with all attacks
        public float ReductionResistanceGeo = 0f; // Add to deal more damage with Geo element
        public float ReductionResistanceAnemo = 0f; // Add to deal more damage with Anemo element
        public float ReductionResistanceCryo = 0f; // Add to deal more damage with Cryo element
        public float ReductionResistanceElectro = 0f; // Add to deal more damage with Electro element
        public float ReductionResistanceDendro = 0f; // Add to deal more damage with Dendro element
        public float ReductionResistanceHydro = 0f; // Add to deal more damage with Hydro element
        public float ReductionResistancePyro = 0f; // Add to deal more damage with Pyro element
        public float ReductionResistancePhysical = 0f; // Add to deal more damage with Physical (or "none") element

        // Reaction-related variables

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
        public int TimerReactionSwirl = 0; // swirl damage ICD
        public byte HitsSwirl = 0; // Swirl can hit 2 times per 0.5 sec
        public byte HitsSuperconduct = 0; // Superconduct can hit 2 times per 0.5 sec

        private bool HalfLifeParticle = false;

        public bool CanBefrozen() => BaseKnockBackResist > 0f; //|| npc.type == NPCID.TargetDummy;
        public override bool InstancePerEntity => true;

        public override void SetDefaults(NPC npc)
        {
            BaseKnockBackResist = npc.knockBackResist;
            npc.defense = 0;
            npc.value = 0;

            if (npc.type == NPCID.ArmoredViking) ResistanceCryo = 1f; // test

            if (npc.type == NPCID.Zombie)
            {
                /*
                if (Main.rand.NextBool(2))
                {
                    Element = GenshinElement.CRYO;
                    TimerElementCryo = 9999999;
                }
                else
                {
                    Element = GenshinElement.ELECTRO;
                    TimerElementElectro = 9999999;
                }
                */

                int elementRand = Main.rand.Next(7);
                /*
                switch (Main.rand.Next(7))
                {
                    case 1:
                        Element = GenshinElement.ELECTRO;
                        TimerElementElectro = 9999999;
                        break;
                    case 2:
                        Element = GenshinElement.PYRO;
                        TimerElementPyro = 9999999;
                        break;
                    case 3:
                        Element = GenshinElement.CRYO;
                        TimerElementCryo = 9999999;
                        break;
                    case 4:
                        Element = GenshinElement.DENDRO;
                        TimerElementDendro = 9999999;
                        break;
                    case 5:
                        Element = GenshinElement.ANEMO;
                        TimerElementAnemo = 9999999;
                        break;
                    case 6:
                        Element = GenshinElement.HYDRO;
                        TimerElementHydro = 9999999;
                        break;
                    default:
                        Element = GenshinElement.GEO;
                        TimerElementGeo = 9999999;
                        break;
                }
                */

                GenshinShieldNPC shield = new Content.ShieldsNPC.BasicShieldNPC().Initialize(this, npc, (GenshinElement)(elementRand + 1));
                AddShield(shield);
                Main.NewText((GenshinElement)(elementRand + 1));
            }
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
            TimerReactionSwirl--;
            if (TimerReactionSwirl <= 0) HitsSwirl = 0;
            if (TimerReactionSuperconduct <= 0) HitsSuperconduct = 0;

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

            npc.knockBackResist = BaseKnockBackResist;

            if (Shields != null)
            {
                for (int i = Shields.Count - 1; i >= 0; i--)
                {
                    Shields[i].ResetEffects();
                    if (Shields[i].GaugeUnit < 1)
                    {
                        Shields[i].OnKillBase();
                        Shields.RemoveAt(i);
                    }
                }
            }
        }

        public override void AI(NPC npc)
        {
            if (npc.life <= npc.lifeMax / 2f && !HalfLifeParticle)
            {
                HalfLifeParticle = true;
                if (!Main.LocalPlayer.dead)
                {
                    if (npc.ModNPC is GenshinNPC genshinNPC) {
                        if (genshinNPC.TimeAlive > 1)
                            SpawnElementalParticle(npc, Element, 1f);
                    }
                    else SpawnElementalParticle(npc, Element, 1f);
                }
            }

            if (Shields != null)
            {
                foreach (GenshinShieldNPC shield in Shields)
                {
                    shield.Update();
                    npc.knockBackResist = BaseKnockBackResist - shield.KnockBackResist;
                    if (npc.knockBackResist < 0f) npc.knockBackResist = 0f;
                }
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

            switch (element)
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
                default: // Physical or NONE
                    if (ResistancePhysical >= 1f) return 0f;
                    mult = ResistancePhysical - ReductionResistancePhysical;
                    break;
            }

            if (mult < 0f) mult *= 0.5f;
            return 1f - mult;
        }

        public bool AffectedByElement(GenshinElement Element = GenshinElement.NONE, bool checkShieldFirst = true)
        { // Without parameters (GenshinElement.NONE), returns false if the npc is affected by ANY element
            switch (Element)
            {
                case GenshinElement.GEO:
                    if (HasShield() && checkShieldFirst) return Shields[0].Element == GenshinElement.GEO;
                    return TimerElementGeo > 0;
                case GenshinElement.ANEMO:
                    if (HasShield() && checkShieldFirst) return Shields[0].Element == GenshinElement.ANEMO;
                    return TimerElementAnemo > 0;
                case GenshinElement.CRYO:
                    if (HasShield() && checkShieldFirst) return Shields[0].Element == GenshinElement.CRYO;
                    return TimerElementCryo > 0;
                case GenshinElement.ELECTRO:
                    if (HasShield() && checkShieldFirst) return Shields[0].Element == GenshinElement.ELECTRO;
                    return TimerElementElectro > 0;
                case GenshinElement.DENDRO:
                    if (HasShield() && checkShieldFirst) return Shields[0].Element == GenshinElement.DENDRO;
                    return TimerElementDendro > 0;
                case GenshinElement.HYDRO:
                    if (HasShield() && checkShieldFirst) return Shields[0].Element == GenshinElement.HYDRO;
                    return TimerElementHydro > 0;
                case GenshinElement.PYRO:
                    if (HasShield() && checkShieldFirst) return Shields[0].Element == GenshinElement.PYRO;
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

        public void DrawTexture(Texture2D texture, SpriteBatch spriteBatch, NPC npc, int nbElements, ref int offSetX, ref int offSetY, int timeLeft)
        {
            float colorMult = timeLeft > 120 ? 1f : (float)Math.Abs(Math.Sin((timeLeft * 0.5f) / Math.PI / 4f));
            Vector2 position = new Vector2(npc.Center.X + offSetX - Main.screenPosition.X, npc.Center.Y + offSetY - Main.screenPosition.Y);
            spriteBatch.Draw(texture, position, null, Color.White * colorMult, 0f, texture.Size() * 0.5f, 0.875f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position, null, Color.White * 0.5f * colorMult, 0f, texture.Size() * 0.5f, 1.025f, SpriteEffects.None, 0f);
            offSetX += 24;
            if (offSetX > 24) setOffset(ref offSetX, ref offSetY, ref nbElements);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Draw shields

            if (Shields != null)
            {
                foreach (GenshinShieldNPC shield in Shields)
                    shield.Draw(spriteBatch, drawColor, npc);
            }

            // Draw elemental auras above NPC

            int nbElements = -1;
            foreach (GenshinElement element in System.Enum.GetValues(typeof(GenshinElement)))
                if (element != GenshinElement.NONE) if (AffectedByElement(element)) nbElements++;
            if (AffectedByElement(GenshinElement.HYDRO, false) && ReactionFrozen) nbElements--;
            int offSetY = -(30 + ElementSymbolDrawOffset);
            int offSetX = 0;
            setOffset(ref offSetX, ref offSetY, ref nbElements);

            if (AffectedByElement(GenshinElement.GEO, false)) DrawTexture(GenshinElementUtils.ElementTexture[0], spriteBatch, npc, nbElements, ref offSetX, ref offSetY, TimerElementGeo);
            if (AffectedByElement(GenshinElement.ANEMO, false)) DrawTexture(GenshinElementUtils.ElementTexture[1], spriteBatch, npc, nbElements, ref offSetX, ref offSetY, TimerElementAnemo);
            if (AffectedByElement(GenshinElement.CRYO, false)) DrawTexture(GenshinElementUtils.ElementTexture[2], spriteBatch, npc, nbElements, ref offSetX, ref offSetY, TimerElementCryo);
            if (AffectedByElement(GenshinElement.ELECTRO, false)) DrawTexture(GenshinElementUtils.ElementTexture[3], spriteBatch, npc, nbElements, ref offSetX, ref offSetY, TimerElementElectro);
            if (AffectedByElement(GenshinElement.DENDRO, false)) DrawTexture(GenshinElementUtils.ElementTexture[4], spriteBatch, npc, nbElements, ref offSetX, ref offSetY, TimerElementDendro);
            if (AffectedByElement(GenshinElement.HYDRO, false) && !ReactionFrozen) DrawTexture(GenshinElementUtils.ElementTexture[5], spriteBatch, npc, nbElements, ref offSetX, ref offSetY, TimerElementHydro);
            if (AffectedByElement(GenshinElement.PYRO, false)) DrawTexture(GenshinElementUtils.ElementTexture[6], spriteBatch, npc, nbElements, ref offSetX, ref offSetY, TimerElementPyro);

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

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.ModProjectile is GenshinProjectile genshinProjectile && genshinProjectile.CanDealDamage)
            {
                GenshinCharacter genshinCharacter = genshinProjectile.OwnerCharacter;
                if (genshinCharacter == null) return;
                GenshinElement element = genshinProjectile.Element;

                int damage = genshinCharacter.ApplyDamageMult(projectile.damage, element, genshinProjectile.AbilityType);
                bool crit = genshinCharacter.GetProjectileCrit(genshinProjectile);
                if (crit) damage = genshinCharacter.GetProjectileCritDamage(genshinProjectile, damage);

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
                            damage = (int)(damage * 2.5f);
                            break;
                        default:
                            break;
                    }
                }

                if (element != GenshinElement.NONE && genshinProjectile.AttackWeight == AttackWeight.BLUNT && ReactionFrozen)
                { // Shatter reaction
                    ReactionFrozen = false;
                    TimerElementCryo = 0;
                    float mastery = genshinCharacter.StatElementalMastery;
                    float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.SHATTER);
                    int reactionDamage = (int)(1.5f * genshinCharacter.ReactionTransformativeDamage * (1f + ((16f * mastery) / (2000f + mastery)) + reactionBonus));
                    int targetDamage = ApplyResistance(reactionDamage, GenshinElement.NONE);
                    if (targetDamage > 0)
                    {
                        genshinCharacter.GenshinPlayer.TryApplyDamageToNPC(npc, targetDamage, 15f, - genshinCharacter.Player.direction, false, GenshinElement.NONE, GenshinProjectile.ElementApplicationMedium);
                        CombatText.NewText(ExtendedHitboxFlat(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.SHATTER), targetDamage);
                    }
                    else CombatText.NewText(ExtendedHitboxFlat(npc), GenshinElementUtils.ColorImmune, "Immune");
                    CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.SHATTER), "Shatter");
                }

                if (damage > 0)
                {
                    CombatText.NewText(ExtendedHitboxFlat(npc), GenshinElementUtils.GetColor(element), damage, crit);
                    genshinCharacter.GenshinPlayer.TryApplyDamageToNPC(npc, damage, projectile.knockBack, projectile.direction, false, element, genshinProjectile.ElementApplication);
                }
                else CombatText.NewText(ExtendedHitboxFlat(npc), GenshinElementUtils.ColorImmune, "Immune");
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0f;
            npc.life++; // bandaid after tmodloader 1.4.4 changes
        }

        public bool HasShield()
        {
            if (Shields == null) return false;
            else return Shields.Count > 0;
        }

        public void AddShield(GenshinShieldNPC shield)
        {
            Shields ??= new List<GenshinShieldNPC>();

            for (int i = Shields.Count - 1; i >= 0; i--)
            {
                if (Shields[i].GetType() == shield.GetType())
                    Shields.RemoveAt(i);
            }

            Shields.Add(shield);
        }

        public void ApplyElement(NPC npc, GenshinProjectile genshinProjectile, GenshinCharacter genshinCharacter, GenshinElement element, ref int damage)
        {
            if (genshinProjectile.IgnoreICD || genshinCharacter.TryApplyElement(npc))
            {
                int application = genshinProjectile.ElementApplication;
                float mastery = genshinCharacter.StatElementalMastery;
                bool reacted = false; // Individual attacks can only cause 1 reaction
                GenshinElement crystallizeElement = GenshinElement.NONE;
                GenshinElement swirlElement = GenshinElement.NONE;

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
                        if (HasShield()) Shields[0].Damage(GenshinElement.NONE, GenshinProjectile.ElementApplicationStrong + 1);
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
                                            genshinCharacter.GenshinPlayer.TryApplyDamageToNPC(target, targetDamage, 15f, -player.direction, false, GenshinElement.PYRO, GenshinProjectile.ElementApplicationMedium);
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
                        if (!HasShield()) TimerElementElectro -= application; // 1x modifier.
                        application = 0;
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Vaporize Weak
                    {
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.VAPORIZE);
                        damage = (int)(damage * 1.5 * (1 + (2.78 * (mastery / (mastery + 1400))) + reactionBonus));

                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.VAPORIZE), "Vaporize");
                        if (!HasShield()) TimerElementHydro -= (int)(application * 0.5); // 0.5x modifier
                        application = 0;
                        reacted = true;
                        if (HasShield()) Shields[0].Damage(GenshinElement.NONE, GenshinProjectile.ElementApplicationStrong + 1);
                    }

                    if (AffectedByElement(GenshinElement.GEO) && !reacted) // Pyro Crystallize
                    {
                        crystallizeElement = GenshinElement.PYRO;
                        if (!HasShield()) TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                    }

                    if (AffectedByElement(GenshinElement.ANEMO) && !reacted && !(genshinProjectile is Content.Projectiles.ProjectileSwirl)) // Pyro Swirl
                    {
                        swirlElement = GenshinElement.PYRO;
                        if (!HasShield()) TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
                    }
                }

                if (element == GenshinElement.HYDRO)
                {
                    if (AffectedByElement(GenshinElement.PYRO) && !reacted) // Vaporize Strong
                    {
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.VAPORIZE);
                        damage = (int)(damage * 2 * (1 + (2.78 * (mastery / (mastery + 1400))) + reactionBonus));

                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.VAPORIZE), "Vaporize");
                        if (!HasShield()) TimerElementPyro -= application * 2; // 2x modifier
                        application = 0;
                        reacted = true;
                        if (HasShield()) Shields[0].Damage(GenshinElement.NONE, GenshinProjectile.ElementApplicationStrong + 1);
                    }

                    if (AffectedByElement(GenshinElement.CRYO) && !reacted && !HasShield()) // Frozen
                    {
                        if (!ReactionFrozen && CanBefrozen())
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
                        //if (HasShield()) TimerElementElectro = (int)(GenshinProjectile.ElementApplicationWeak / 2f);
                        ReactionElectrocharged = 1;
                        ReactionElectrochargedPlayer = genshinCharacter.Player.whoAmI;
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.ELECTROCHARGED);
                        ReactionElectrochargedDamage = (int)(1.2f * genshinCharacter.ReactionTransformativeDamage * (1f + ((16f * mastery) / (2000f + mastery)) + reactionBonus));
                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetColor(GenshinElement.ELECTRO), "Electro-Charged");
                        reacted = true;
                        if (HasShield()) InflictElement(element, application);
                    }

                    if (AffectedByElement(GenshinElement.GEO) && !reacted) // Hydro Crystallize
                    {
                        crystallizeElement = GenshinElement.HYDRO;
                        if (!HasShield()) TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                    }

                    if (AffectedByElement(GenshinElement.ANEMO) && !reacted && !(genshinProjectile is Content.Projectiles.ProjectileSwirl)) // Hydro Swirl
                    {
                        swirlElement = GenshinElement.HYDRO;
                        if (!HasShield()) TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
                    }
                }

                if (element == GenshinElement.CRYO)
                {
                    if (AffectedByElement(GenshinElement.PYRO) && !reacted) // Melt Weak
                    {
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.MELT);
                        damage = (int)(damage * 1.5 * (1 + (2.78 * (mastery / (mastery + 1400)) * 1) + reactionBonus));

                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.MELT), "Melt");
                        if (!HasShield()) TimerElementPyro -= (int)(application * 0.5); // 0.5x modifier
                        application = 0;
                        reacted = true;
                        if (HasShield()) Shields[0].Damage(GenshinElement.NONE, GenshinProjectile.ElementApplicationStrong + 1);
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
                                    if (genshinNPC.HitsSuperconduct < 2)
                                    {
                                        int targetDamage = genshinNPC.ApplyResistance(reactionDamage, GenshinElement.CRYO);
                                        if (targetDamage > 0)
                                        {
                                            genshinCharacter.GenshinPlayer.TryApplyDamageToNPC(target, targetDamage, 0.5f, -player.direction, false, GenshinElement.CRYO, GenshinProjectile.ElementApplicationMedium);
                                            CombatText.NewText(ExtendedHitboxFlat(target), GenshinElementUtils.GetReactionColor(GenshinReaction.SUPERCONDUCT), targetDamage);
                                        }
                                        else CombatText.NewText(ExtendedHitboxFlat(target), GenshinElementUtils.ColorImmune, "Immune");
                                        genshinNPC.ReactionSuperconduct = 60 * 12; // Phys red debuff 12 sec.
                                        if (genshinNPC.HitsSuperconduct == 0) genshinNPC.TimerReactionSuperconduct = 30; // Reaction damage icd.
                                        genshinNPC.HitsSuperconduct++;
                                    }
                                }
                            }
                        }

                        SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, npc.Center);
                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.SUPERCONDUCT), "Superconduct");
                        if (!HasShield()) TimerElementElectro -= application; // 1x modifier.
                        application = 0;
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Frozen
                    {
                        if (!ReactionFrozen && CanBefrozen())
                        {
                            //if (HasShield()) TimerElementHydro = (int)(GenshinProjectile.ElementApplicationWeak / 2f);
                            float FactorFrozen = 1f + genshinCharacter.GetReactionBonus(GenshinReaction.FROZEN); // affects freeze duration (multiplies element timer loss), not affercted by EM.
                            int minValue = (int)(MathHelper.Min(application, TimerElementHydro));
                            TimerElementHydro = (int)((MathHelper.Max(TimerElementHydro - minValue, 0) + minValue * 2) * FactorFrozen);
                            application = (int)((MathHelper.Max(application - minValue, 0) + minValue * 2) * FactorFrozen);
                            if (HasShield()) InflictElement(element, application);
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
                        if (!HasShield()) TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                    }

                    if (AffectedByElement(GenshinElement.ANEMO) && !reacted && !(genshinProjectile is Content.Projectiles.ProjectileSwirl)) // Cryo Swirl
                    {
                        swirlElement = GenshinElement.CRYO;
                        if (!HasShield()) TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
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
                                            genshinCharacter.GenshinPlayer.TryApplyDamageToNPC(target, targetDamage, 15f, -player.direction, false, GenshinElement.PYRO, GenshinProjectile.ElementApplicationMedium);
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
                        if (!HasShield()) TimerElementPyro -= application; // 1x modifier.
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
                                            genshinCharacter.GenshinPlayer.TryApplyDamageToNPC(target, targetDamage, 0.5f, -player.direction, false, GenshinElement.CRYO, GenshinProjectile.ElementApplicationMedium);
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
                        if (!HasShield()) TimerElementCryo -= application; // 1x modifier.
                        application = 0;
                        reacted = true;
                    }

                    if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Electro-Charged
                    {
                        //if (HasShield()) TimerElementHydro = (int)(GenshinProjectile.ElementApplicationWeak / 2f);
                        ReactionElectrocharged = 1;
                        ReactionElectrochargedPlayer = genshinCharacter.Player.whoAmI;
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.ELECTROCHARGED);
                        ReactionElectrochargedDamage = (int)(1.2f * genshinCharacter.ReactionTransformativeDamage * (1f + ((16f * mastery) / (2000f + mastery)) + reactionBonus));
                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetColor(GenshinElement.ELECTRO), "Electro-Charged");
                        reacted = true;
                        if (HasShield()) InflictElement(element, application);
                    }

                    if (AffectedByElement(GenshinElement.GEO) && !reacted) // Electro Crystallize
                    {
                        crystallizeElement = GenshinElement.ELECTRO;
                        if (!HasShield()) TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                    }

                    if (AffectedByElement(GenshinElement.ANEMO) && !reacted && !(genshinProjectile is Content.Projectiles.ProjectileSwirl)) // Electro Swirl
                    {
                        swirlElement = GenshinElement.ELECTRO;
                        if (!HasShield()) TimerElementGeo -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
                    }
                }

                if (element == GenshinElement.GEO)
                {
                    if (AffectedByElement(GenshinElement.PYRO) && !reacted) // Pyro Crystallize
                    {
                        crystallizeElement = GenshinElement.PYRO;
                        if (!HasShield()) TimerElementPyro -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
                    }
                    if (AffectedByElement(GenshinElement.CRYO) && !reacted) // Cryo Crystallize
                    {
                        crystallizeElement = GenshinElement.CRYO;
                        if (!HasShield()) TimerElementCryo -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
                    }
                    if (AffectedByElement(GenshinElement.ELECTRO) && !reacted) // Electro Crystallize
                    {
                        crystallizeElement = GenshinElement.ELECTRO;
                        if (!HasShield()) TimerElementElectro -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
                    }
                    if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Hydro Crystallize
                    {
                        crystallizeElement = GenshinElement.HYDRO;
                        if (!HasShield()) TimerElementHydro -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
                    }
                }

                if (element == GenshinElement.ANEMO)
                {
                    if (AffectedByElement(GenshinElement.PYRO) && !reacted) // Pyro Swirl
                    {
                        swirlElement = GenshinElement.PYRO;
                        if (!HasShield()) TimerElementPyro -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
                    }
                    if (AffectedByElement(GenshinElement.CRYO) && !reacted) // Cryo Swirl
                    {
                        swirlElement = GenshinElement.CRYO;
                        if (!HasShield()) TimerElementCryo -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
                    }
                    if (AffectedByElement(GenshinElement.ELECTRO) && !reacted) // Electro Swirl
                    {
                        swirlElement = GenshinElement.ELECTRO;
                        if (!HasShield()) TimerElementElectro -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
                    }
                    if (AffectedByElement(GenshinElement.HYDRO) && !reacted) // Hydro Swirl
                    {
                        swirlElement = GenshinElement.HYDRO;
                        if (!HasShield()) TimerElementHydro -= (int)(application * 0.5); // 0.5x modifier
                        reacted = true;
                    }
                }

                if (swirlElement != GenshinElement.NONE)
                {
                    Player player = genshinCharacter.Player;
                    int type = ModContent.ProjectileType<Content.Projectiles.ProjectileSwirl>();
                    int proj = Projectile.NewProjectile(player.GetSource_Misc("GenshinMod Elemental Reaction"), npc.Center, Vector2.Zero, type, 0, 0f);
                    if (Main.projectile[proj].ModProjectile is Content.Projectiles.ProjectileSwirl swirl)
                    {
                        swirl.Element = swirlElement;
                        float reactionBonus = genshinCharacter.GetReactionBonus(GenshinReaction.SWIRL);
                        int reactionDamage = (int)(0.6f * genshinCharacter.ReactionTransformativeDamage * (1f + ((16f * mastery) / (2000f + mastery)) + reactionBonus));
                        foreach (NPC target in Main.npc)
                        {
                            if (GenshinProjectile.CanHomeInto(target))
                            {
                                if (npc.Center.Distance(target.Center) < 128f)
                                {
                                    GenshinGlobalNPC genshinNPC = target.GetGlobalNPC<GenshinGlobalNPC>();
                                    if (genshinNPC.HitsSwirl < 2)
                                    {
                                        genshinNPC.ApplyElement(npc, swirl, genshinCharacter, swirlElement, ref reactionDamage);
                                        int targetDamage = genshinNPC.ApplyResistance(reactionDamage, swirlElement);
                                        if (targetDamage > 0)
                                        {
                                            genshinCharacter.GenshinPlayer.TryApplyDamageToNPC(target, targetDamage, 0f, -player.direction, false, GenshinElement.ANEMO, GenshinProjectile.ElementApplicationMedium);
                                            CombatText.NewText(ExtendedHitboxFlat(target), GenshinElementUtils.GetReactionColor(GenshinReaction.SWIRL), targetDamage);
                                        }
                                        else CombatText.NewText(ExtendedHitboxFlat(target), GenshinElementUtils.ColorImmune, "Immune");
                                        if (genshinNPC.HitsSwirl == 0) genshinNPC.TimerReactionSwirl = 30; // Reaction damage icd.
                                        genshinNPC.HitsSwirl++;
                                    }
                                }
                            }
                        }

                        CombatText.NewText(ReactionHitbox(npc), GenshinElementUtils.GetReactionColor(GenshinReaction.SWIRL), "Swirl");
                        application = 0;
                    }
                }

                if (crystallizeElement != GenshinElement.NONE)
                {
                    GenshinElementUtils.ClearCrystallizeCrystals();

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

                if (!HasShield()) InflictElement(element, application);
            }
        }

        public void ProcessReactions(NPC npc)
        {
            if (ReactionFrozen) // Frozen
            {
                npc.velocity *= 0f;
                npc.direction = ReactionFrozenDirection;
                npc.position = ReactionFrozenPosition;
                if (TimerElementHydro <= 0 && !AffectedByElement(GenshinElement.HYDRO) || TimerElementCryo <= 0) ReactionFrozen = false;
            }

            if (ReactionSuperconduct > 0)
            {
                ReactionSuperconduct--;
                ReductionResistancePhysical += 0.4f;
            }

            if (ReactionElectrocharged <= 0 && ReactionElectrochargedDamage > 0)
            {
                if (AffectedByElement(GenshinElement.HYDRO) || AffectedByElement(GenshinElement.HYDRO, false) && AffectedByElement(GenshinElement.ELECTRO) || AffectedByElement(GenshinElement.ELECTRO, false))
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
                                            player.GetModPlayer<GenshinPlayer>().TryApplyDamageToNPC(target, targetDamage, 0f, player.direction, false, GenshinElement.ELECTRO, GenshinProjectile.ElementApplicationWeak);
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
            rect.X -= (int)(npc.width / 2f);
            rect.Y -= (int)(npc.height / 2f);
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