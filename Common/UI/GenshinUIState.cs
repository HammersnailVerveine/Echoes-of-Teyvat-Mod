using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.UI;

namespace GenshinMod.Common.UI
{
    public abstract class GenshinUIState : UIState
    { // Taken from Orchid Mod
        private readonly string name;

        public GenshinUIState()
        {
            name = GetType().Name;
        }

        public Mod Mod { get; internal set; }
        public string Name { get => name; }

        public virtual InterfaceScaleType ScaleType { get; set; } = InterfaceScaleType.UI;
        public virtual bool Visible { get; set; } = true;
        public virtual float Priority { get; set; } = 0f;
        public abstract int InsertionIndex(List<GameInterfaceLayer> layers);

        public virtual void Unload() { }
        public virtual void OnResolutionChanged(int width, int height) { }
        public virtual void OnUIScaleChanged() { }
    }
}