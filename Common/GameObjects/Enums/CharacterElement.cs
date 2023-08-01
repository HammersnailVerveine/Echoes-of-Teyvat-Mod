using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace GenshinMod.Common.GameObjects.Enums
{
    public enum GenshinElement : int
    {
        NONE = 0,
        GEO = 1,
        ANEMO = 2,
        CRYO = 3,
        ELECTRO = 4,
        DENDRO = 5,
        HYDRO = 6,
        PYRO = 7
    }
    public enum GenshinReaction : int
    {
        NONE = 0,
        VAPORIZE = 1,
        OVERLOADED = 2,
        MELT = 3,
        ELECTROCHARGED = 4,
        FROZEN = 5,
        SUPERCONDUCT = 6,
        SWIRL = 7,
        CRYSTALLIZE = 8,
        BURNING = 9,
        BLOOM = 10,
        HYPERBLOOM = 11,
        BURGEON = 12,
        QUICKEN = 13,
        AGGRAVATE = 14,
        SPREAD = 15,
        SHATTER = 16
    }
    public static class GenshinElementUtils
    {
        public static Texture2D[] ElementTexture;

        public static void LoadTexture()
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
        public static void UnloadTexture()
        {
            ElementTexture = new Texture2D[7];
            ElementTexture[0] = null;
            ElementTexture[1] = null;
            ElementTexture[2] = null;
            ElementTexture[3] = null;
            ElementTexture[4] = null;
            ElementTexture[5] = null;
            ElementTexture[6] = null;
            ElementTexture = null;
        }

        public static Color ColorImmune => new Color(168, 168, 168);

        public static Color GetColor(GenshinElement element)
        {
            switch (element)
            {
                case GenshinElement.GEO:
                    return new Color(255, 204, 102);
                case GenshinElement.ANEMO:
                    return new Color(102, 255, 204);
                case GenshinElement.CRYO:
                    return new Color(153, 255, 255);
                case GenshinElement.DENDRO:
                    return new Color(0, 234, 82);
                case GenshinElement.ELECTRO:
                    return new Color(225, 155, 255);
                case GenshinElement.HYDRO:
                    return new Color(51, 204, 255);
                case GenshinElement.PYRO:
                    return new Color(255, 155, 0);
                default: // NONE
                    return new Color(255, 255, 255);
            }
        }
        public static Color GetReactionColor(GenshinReaction reaction)
        {
            switch (reaction)
            {
                case GenshinReaction.VAPORIZE:
                    return new Color(255, 204, 102);
                case GenshinReaction.OVERLOADED:
                    return new Color(255, 128, 155);
                case GenshinReaction.MELT:
                    return new Color(255, 204, 102);
                case GenshinReaction.ELECTROCHARGED:
                    return new Color(225, 155, 255);
                case GenshinReaction.FROZEN:
                    return new Color(153, 255, 255);
                case GenshinReaction.SUPERCONDUCT:
                    return new Color(180, 180, 255);
                case GenshinReaction.SWIRL:
                    return new Color(102, 255, 204);
                case GenshinReaction.CRYSTALLIZE:
                    return new Color(255, 204, 102);
                case GenshinReaction.BURNING:
                    return new Color(255, 255, 255);
                case GenshinReaction.BLOOM:
                    return new Color(0, 234, 82);
                case GenshinReaction.HYPERBLOOM:
                    return new Color(225, 155, 255);
                case GenshinReaction.BURGEON:
                    return new Color(255, 155, 0);
                case GenshinReaction.QUICKEN:
                    return new Color(0, 234, 82);
                case GenshinReaction.AGGRAVATE:
                    return new Color(225, 155, 255);
                case GenshinReaction.SPREAD:
                    return new Color(0, 234, 82);
                case GenshinReaction.SHATTER:
                    return new Color(255, 255, 255);
                default: // NONE
                    return new Color(255, 255, 255);
            }
        }
    }
}