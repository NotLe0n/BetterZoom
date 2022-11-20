using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BetterZoom;

internal class Config : ModConfig
{
	public static Config Instance;

	public override ConfigScope Mode => ConfigScope.ClientSide;

	[Label("Scale Background")]
	[Tooltip("This toggles if the background should zoom too")]
	public bool scaleBackground;

	[Slider]
	[Range(0.1f, 10.0f)]
	[DefaultValue(1f)]
	[Tooltip("This changes the size of your cursor")]
	[Label("Cursor Scale")]
	public float cursorScale;
	
	[Range(0.3f, 10.0f)]
	[DefaultValue(0.3f)]
	[ReloadRequired]
	[Tooltip("This changes the minimum amount of zoom you can set\nYou need to set this in the mod menu, since it requires a reload")]
	[Label("Min Zoom")]
	public float minZoom;
	
	[ReloadRequired]
	[Range(0.3f, 10.0f)]
	[DefaultValue(10f)]
	[Tooltip("This changes the maximal amount of zoom you can set\nYou need to set this in the mod menu, since it requires a reload")]
	[Label("Max Zoom")]
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
