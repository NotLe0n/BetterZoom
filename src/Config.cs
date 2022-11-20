using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BetterZoom;

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
	
	[Range(0.3f, 10.0f)]
	[DefaultValue(0.3f)]
	[ReloadRequired]
	[Label("min zoom")]
	public float minZoom;
	
	[Range(0.3f, 10.0f)]
	[DefaultValue(10f)]
	[ReloadRequired]
	[Label("max zoom")]
	public float maxZoom;

	public override void OnChanged()
	{
		if (minZoom > maxZoom) {
			minZoom = maxZoom;
		}
		base.OnChanged();
	}

	public override void OnLoaded()
	{
		Instance = this;
		base.OnLoaded();
	}
}
