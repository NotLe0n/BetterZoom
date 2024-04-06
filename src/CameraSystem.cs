using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace BetterZoom;

public sealed class CameraSystem : ModSystem
{
	private static readonly Config Config = ModContent.GetInstance<Config>();
	
	public override void ModifyTransformMatrix(ref SpriteViewMatrix transform)
	{
		if (Main.gameMenu) {
			return;
		}

		// Zoom with background if above one
		if (Config.scaleBackground) {
			Main.BackgroundViewMatrix.Zoom = new Vector2(Main.GameZoomTarget);
		}

		Main.cursorScale = Config.cursorScale;
	}
}