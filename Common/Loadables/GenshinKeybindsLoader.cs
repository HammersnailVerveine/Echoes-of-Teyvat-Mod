using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace GenshinMod.Common.Loadables
{
    public class GenshinKeybindsLoader : ILoadable
    {
        public static ModKeybind AbilitySkill { get; private set; }
        public static ModKeybind AbilityBurst { get; private set; }
        public static ModKeybind Character1 { get; private set; }
        public static ModKeybind Character2 { get; private set; }
        public static ModKeybind Character3 { get; private set; }
        public static ModKeybind Character4 { get; private set; }
        public static ModKeybind Character5 { get; private set; }
        public static ModKeybind Debug { get; private set; }


        void ILoadable.Load(Mod mod)
        {
            AbilitySkill = KeybindLoader.RegisterKeybind(mod, "Elemental Skill", Keys.E);
            AbilityBurst = KeybindLoader.RegisterKeybind(mod, "Elemental Burst", Keys.Q);
            Character1 = KeybindLoader.RegisterKeybind(mod, "Character #1", Keys.D1);
            Character2 = KeybindLoader.RegisterKeybind(mod, "Character #2", Keys.D2);
            Character3 = KeybindLoader.RegisterKeybind(mod, "Character #3", Keys.D3);
            Character4 = KeybindLoader.RegisterKeybind(mod, "Character #4", Keys.D4);
            Character5 = KeybindLoader.RegisterKeybind(mod, "Character #5", Keys.D5);
            Debug = KeybindLoader.RegisterKeybind(mod, "Debug", Keys.OemTilde);
        }

        void ILoadable.Unload()
        {
            AbilitySkill = null;
            AbilityBurst = null;
            Character1 = null;
            Character2 = null;
            Character3 = null;
            Character4 = null;
            Character5 = null;
            Debug = null;
        }

    }
}
