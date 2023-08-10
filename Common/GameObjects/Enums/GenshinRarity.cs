using Microsoft.Xna.Framework;

namespace GenshinMod.Common.GameObjects.Enums
{
    public enum GenshinRarity : int
    {
        NONE = 0,
        ONESTAR = 1,
        TWOSTAR = 2,
        THREESTAR = 3,
        FOURSTAR = 4,
        FIVESTAR = 5
    }

    public static class GenshinRarityUtils
    {
        public static float GetStatMultiplierMain(GenshinRarity rarity)
        {
            switch (rarity)
            {
                case GenshinRarity.FOURSTAR:
                    return 1.2f;
                case GenshinRarity.FIVESTAR:
                    return 1.4f;
                default: // 1-3 stars
                    return 1f;
            }
        }
        public static float GetStatMultiplierSecondary(GenshinRarity rarity)
        {
            switch (rarity)
            {
                case GenshinRarity.FOURSTAR:
                    return 1f;
                case GenshinRarity.FIVESTAR:
                    return 1.2f;
                default: // 1-3 stars
                    return 0.8f;
            }
        }

        public static Color GetColor(GenshinRarity rarity)
        {
            switch (rarity)
            {
                case GenshinRarity.TWOSTAR:
                    return new Color(100, 150, 125);
                case GenshinRarity.THREESTAR:
                    return new Color(100, 135, 170);
                case GenshinRarity.FOURSTAR:
                    return new Color(132, 114, 167);
                case GenshinRarity.FIVESTAR:
                    return new Color(174, 124, 50);
                default: // one star
                    return new Color(135, 135, 135);
            }
        }
    }
}