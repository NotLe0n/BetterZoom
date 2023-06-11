using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BetterZoom;

internal sealed class Config : ModConfig
{
	public static Config Instance;

	public override ConfigScope Mode => ConfigScope.ClientSide;
	
	public bool scaleBackground;

	[Slider]
	[Range(0.1f, 10.0f)]
	[DefaultValue(1f)]
	public float cursorScale;
	
	[Range(0.3f, 10.0f)]
	[DefaultValue(0.3f)]
	[ReloadRequired]
	public float minZoom;
	
	[ReloadRequired]
	[Range(0.3f, 10.0f)]
	[DefaultValue(10f)]
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
