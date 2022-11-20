using BetterZoom.Edits;
using Terraria.ModLoader;

namespace BetterZoom;

public class BetterZoom : Mod
{
	public static readonly float MinGameZoom = Config.Instance.minZoom;
	public static readonly float MaxGameZoom = Config.Instance.maxZoom;
	public const float MIN_UI_ZOOM = 0.3f;
	public const float MAX_UI_ZOOM = 4.0f;

	public override void Load()
	{
		SettingsEdits.Load();
		ZoomEdits.Load();
		base.Load();
	}
}