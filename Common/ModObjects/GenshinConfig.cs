using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace GenshinMod.Common.ModObjects
{
	public class GenshinConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;
		public override bool Autoload(ref string name) => true;

		[Label("Enable elemental burst quotes")]
		[Tooltip("$Displays a quote when an elemental burst is used")]
		[DefaultValue(false)]
		public bool EnableBurstQuotes;
	}
}