using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace GenshinMod
{
	public class GenshinMod : Mod
	{
		internal static GenshinMod Instance { get; private set; }

		public override void Unload()
        {
            base.Unload();
		}

		public GenshinMod()
		{
			Instance = this;
		}
	}
}