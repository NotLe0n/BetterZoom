using BetterZoom.Edits;
using Terraria.ModLoader;

namespace BetterZoom;

public class BetterZoom : Mod
{
	public const float MIN_GAME_ZOOM = 0.3f;
	public const float MIN_UI_ZOOM = 0.3f;
	public const float MAX_GAME_ZOOM = 10.0f;
	public const float MAX_UI_ZOOM = 4.0f;

	public override void Load()
	{
		SettingsEdits.Load();
		ZoomEdits.Load();
		base.Load();
	}
}