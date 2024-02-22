using BetterZoom.Edits;
using Terraria.ModLoader;

namespace BetterZoom;

public class BetterZoom : Mod
{
	public override void Load()
	{
		SettingsEdits.Load();
		ZoomEdits.Load();
		base.Load();
	}
}