namespace GenshinMod.Common.GameObjects.Enums
{
    public enum WeaponType : int
    {
        BOW = 0,
        CATALYST = 1,
        POLEARM = 2,
        CLAYMORE = 3,
        SWORD = 4
    }
    public static class WeaponTypeUtils
    {
        public static string ToString(WeaponType weapon)
        {
            switch (weapon)
            {
                case WeaponType.BOW:
                    return "Bow";
                case WeaponType.CATALYST:
                    return "Catalyst";
                case WeaponType.POLEARM:
                    return "Polearm";
                case WeaponType.CLAYMORE:
                    return "Claymore";
                case WeaponType.SWORD:
                    return "Sword";
                default:
                    return "None";
            }
        }
    }
}