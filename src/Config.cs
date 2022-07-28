using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BetterZoom.src;

internal class Config : ModConfig
{
	public static Config Instance;

	public override ConfigScope Mode => ConfigScope.ClientSide;

	[Label("Scale Background")]
	public bool scaleBackground;

	[Slider]
	[Range(0.1f, 10.0f)]
	[DefaultValue(1f)]
	[Label("Cursor Scale")]
	public float cursorScale;

	public override void OnLoaded()
	{
		Instance = this;
		base.OnLoaded();
	}
}
