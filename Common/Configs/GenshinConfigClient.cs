using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace GenshinMod.Common.Configs
{
	public class GenshinConfigClient : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;
        public override bool Autoload(ref string name) => true;

		// The things in brackets are known as "Attributes".
		
		[Header("Misc")]

		[DefaultValue(false)]
		[BackgroundColor(50, 200, 100)]
		public bool EnableBurstQuotes;
	}
}
