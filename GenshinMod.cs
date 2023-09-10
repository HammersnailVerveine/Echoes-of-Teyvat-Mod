using GenshinMod.Common.GameObjects.Enums;
using Terraria.ModLoader;
using static GenshinMod.Common.Static.MethodSwaps;

namespace GenshinMod
{
    public class GenshinMod : Mod
    {
        internal static GenshinMod Instance { get; private set; }

        public override void Load()
        {
            GenshinElementUtils.LoadTexture();
            ApplyMethodSwaps();
            base.Load();
        }

        public override void Unload()
        {
            GenshinElementUtils.UnloadTexture();
            base.Unload();
        }

        public GenshinMod()
        {
            Instance = this;
        }

        public void CopyMap()
        {

        }
    }
}