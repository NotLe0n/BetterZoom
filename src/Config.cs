using System.ComponentModel;
using Terraria.ModLoader.Config;

// ReSharper disable InconsistentNaming
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace BetterZoom;

internal sealed class Config : ModConfig
{
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

	[Range(0.3f, 6f)]
	[DefaultValue(0.3f)]
	[ReloadRequired]
	public float minUIScale;
	
	[ReloadRequired]
	[Range(0.3f, 6f)]
	[DefaultValue(6f)]
	public float maxUIScale;
	
	public override void OnChanged()
	{
		if (minZoom > maxZoom) {
			minZoom = maxZoom;
		}
		
		if (minUIScale > maxUIScale) {
			minUIScale = maxUIScale;
		}
		
		base.OnChanged();
	}
}
