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
    }
}