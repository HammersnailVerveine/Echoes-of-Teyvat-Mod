using GenshinMod.Common.GameObjects.Enums;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace GenshinMod
{
	public class GenshinMod : Mod
	{
		internal static GenshinMod Instance { get; private set; }

        public override void Load()
        {
			GenshinElementUtils.LoadTexture();
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
	}
}