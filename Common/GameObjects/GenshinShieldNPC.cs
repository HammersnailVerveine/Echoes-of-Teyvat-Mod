using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.GlobalObjets;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace GenshinMod.Common.GameObjects
{
    public abstract class GenshinShieldNPC
    {
        // Fields

        public GenshinElement Element = GenshinElement.NONE; // Shield Element
        public GenshinGlobalNPC GlobalNPC; // GlobalNPC tied to the shield
        public NPC NPC; // npc tied to the shield
        public int TimeSpent = 0; // Time spent active

        // Fields to setup in OnInitialize()

        public int GaugeUnit; // Shield remaining health in gauge units. Weak attacks will deal 1, strong attacks deal 2. Multiplied based on enemy element. For instance, a Cryo slime should have 32 (multiply the GU of genshin enemies by 4).
        public float ShieldResistance = 1f; // 0f = 0% to 1f = 100%, percentage of damage mitigated by the shield while active. Should de be set in OnInitialize()
        public float KnockBackResist = 0f; // Inverse of NPC.KnockBackResist. 1f means the npc takes no knockback when the shield is active, 0f means it takes full knockback. Should de be set in OnInitialize()
        public bool WeakToBlunt; // Does the shield take bonus damage from blunt attacks?

        public void UpdateBase()
        {
            Update();
        }

        public void OnKillBase()
        {
            OnKill();
        }

        public void ResetEffects()
        {
        }

        public GenshinShieldNPC Initialize(GenshinGlobalNPC globalNPC, NPC npc, GenshinElement element = GenshinElement.NONE, int ai = 0) // "ai" is used to communicate more information if needed
        {
            GlobalNPC = globalNPC;
            NPC = npc;
            OnInitialize(ref element, ai);
            Element = element;
            return this;
        }

        public void Damage(GenshinElement element = GenshinElement.NONE, int application = 0, AttackWeight attackWeight = AttackWeight.LIGHT)
        {
            int damageUnit = 1;
            if (application > GenshinProjectile.ElementApplicationWeak) damageUnit = 2;
            if (application > GenshinProjectile.ElementApplicationMedium) damageUnit = 3;
            if (application > GenshinProjectile.ElementApplicationStrong) damageUnit = 4;

            if (WeakToBlunt)
            {
                if (attackWeight == AttackWeight.MEDIUM) damageUnit *= 2;
                if (attackWeight == AttackWeight.BLUNT) damageUnit *= 4;
            }

            if (element != GenshinElement.NONE)
            { // Elemental damage on shields is multiplied
                switch (Element) // Weaknesses table
                {
                    case GenshinElement.PYRO:
                        switch (element)
                        {
                            case GenshinElement.PYRO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            case GenshinElement.HYDRO:
                                damageUnit = (int)(damageUnit * 2f);
                                break;
                            case GenshinElement.ELECTRO:
                                damageUnit = (int)(damageUnit * 1f);
                                break;
                            case GenshinElement.CRYO:
                                damageUnit = (int)(damageUnit * 0.5f);
                                break;
                            case GenshinElement.DENDRO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            case GenshinElement.GEO:
                                damageUnit = (int)(damageUnit * 0.5f);
                                break;
                            case GenshinElement.ANEMO:
                                damageUnit = (int)(damageUnit * 0.5f);
                                break;
                            default:
                                break;
                        }
                        break;
                    case GenshinElement.HYDRO:
                        switch (element)
                        {
                            case GenshinElement.PYRO:
                                damageUnit = (int)(damageUnit * 0.5f);
                                break;
                            case GenshinElement.HYDRO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            case GenshinElement.ELECTRO:
                                damageUnit = (int)(damageUnit * 1f);
                                break;
                            case GenshinElement.CRYO:
                                damageUnit = (int)(damageUnit * 1f);
                                break;
                            case GenshinElement.DENDRO:
                                damageUnit = (int)(damageUnit * 2f);
                                break;
                            case GenshinElement.GEO:
                                damageUnit = (int)(damageUnit * 0.5f);
                                break;
                            case GenshinElement.ANEMO:
                                damageUnit = (int)(damageUnit * 0.5f);
                                break;
                            default:
                                break;
                        }
                        break;
                    case GenshinElement.ELECTRO:
                        switch (element)
                        {
                            case GenshinElement.PYRO:
                                damageUnit = (int)(damageUnit * 1f);
                                break;
                            case GenshinElement.HYDRO:
                                damageUnit = (int)(damageUnit * 1f);
                                break;
                            case GenshinElement.ELECTRO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            case GenshinElement.CRYO:
                                damageUnit = (int)(damageUnit * 1f);
                                break;
                            case GenshinElement.DENDRO:
                                damageUnit = (int)(damageUnit * 1f);
                                break;
                            case GenshinElement.GEO:
                                damageUnit = (int)(damageUnit * 0.5f);
                                break;
                            case GenshinElement.ANEMO:
                                damageUnit = (int)(damageUnit * 0.5f);
                                break;
                            default:
                                break;
                        }
                        break;
                    case GenshinElement.CRYO:
                        switch (element)
                        {
                            case GenshinElement.PYRO:
                                damageUnit = (int)(damageUnit * 2f);
                                break;
                            case GenshinElement.HYDRO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            case GenshinElement.ELECTRO:
                                damageUnit = (int)(damageUnit * 1f);
                                break;
                            case GenshinElement.CRYO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            case GenshinElement.DENDRO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            case GenshinElement.GEO:
                                damageUnit = (int)(damageUnit * 0.5f);
                                break;
                            case GenshinElement.ANEMO:
                                damageUnit = (int)(damageUnit * 0.5f);
                                break;
                            default:
                                break;
                        }
                        break;
                    case GenshinElement.DENDRO:
                        switch (element)
                        {
                            case GenshinElement.PYRO:
                                damageUnit = (int)(damageUnit * 1f);
                                break;
                            case GenshinElement.HYDRO:
                                damageUnit = (int)(damageUnit * 0.5f);
                                break;
                            case GenshinElement.ELECTRO:
                                damageUnit = (int)(damageUnit * 1f);
                                break;
                            case GenshinElement.CRYO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            case GenshinElement.DENDRO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            case GenshinElement.GEO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            case GenshinElement.ANEMO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            default:
                                break;
                        }
                        break;
                    case GenshinElement.GEO:
                        switch (element)
                        {
                            case GenshinElement.PYRO:
                                damageUnit = (int)(damageUnit * 2f);
                                break;
                            case GenshinElement.HYDRO:
                                damageUnit = (int)(damageUnit * 2f);
                                break;
                            case GenshinElement.ELECTRO:
                                damageUnit = (int)(damageUnit * 2f);
                                break;
                            case GenshinElement.CRYO:
                                damageUnit = (int)(damageUnit * 2f);
                                break;
                            case GenshinElement.DENDRO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            case GenshinElement.GEO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            case GenshinElement.ANEMO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            default:
                                break;
                        }
                        break;
                    case GenshinElement.ANEMO:
                        // Does not exist in the base game, thus follows the same rules as geo (?)
                        switch (element)
                        {
                            case GenshinElement.PYRO:
                                damageUnit = (int)(damageUnit * 2f);
                                break;
                            case GenshinElement.HYDRO:
                                damageUnit = (int)(damageUnit * 2f);
                                break;
                            case GenshinElement.ELECTRO:
                                damageUnit = (int)(damageUnit * 2f);
                                break;
                            case GenshinElement.CRYO:
                                damageUnit = (int)(damageUnit * 2f);
                                break;
                            case GenshinElement.DENDRO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            case GenshinElement.GEO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            case GenshinElement.ANEMO:
                                damageUnit = (int)(damageUnit * 0f);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

            GaugeUnit -= damageUnit;
            if (GaugeUnit < 0) GaugeUnit = 0;
        }

        public virtual void OnInitialize(ref GenshinElement element, int ai) { }
        public virtual void Update() { }
        public virtual void OnKill() { }
        public virtual void Draw(SpriteBatch spriteBatch, Color lightColor, NPC npc) { }
    }
}
