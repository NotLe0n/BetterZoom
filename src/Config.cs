using System;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using BetterZoom.Edits;
using Terraria.ModLoader.Config;

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
	
	[Range(0.1f, 10.0f)]
	[CustomModConfigItem(typeof(DualRangeElement))]
	public FloatRange zoomRange = new() {
		min = 0.3f, max = 10.0f
	};

	[Range(0.5f, 5)]
	[CustomModConfigItem(typeof(DualRangeElement))]
	public FloatRange UIScaleRange = new() {
		min = 0.5f, max = 4f
	};

	[DefaultValue(false)]
	public bool disableUIZoomHotkey;

	[ReloadRequired]
	[DefaultValue(true)]
	public bool renderMoreTiles;

	[DefaultValue(1f)]
	[Range(0.1f, 1)]
	public float tileRenderLimit;
	
	[DefaultValue(1)]
	[Range(0.1f, 10f)]
	[Increment(0.1f)]
	public float zoomSpeed;
	
	public override void OnChanged()
	{
		RenderEdits.ReloadRenderTargets();
		base.OnChanged();
	}

	public override void OnLoaded()
	{
		StringBuilder b = new StringBuilder("BetterZoom Config:");
		b.AppendLine("{");
		foreach (var field in typeof(Config).GetFields()) {
			b.AppendLine($"\t{field.Name}: {field.GetValue(this)?.ToString() ?? "null"}");
		}
		b.AppendLine("}");
		
		Mod.Logger.Info(b.ToString());
		base.OnLoaded();
	}
}
