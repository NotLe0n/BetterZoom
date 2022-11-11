using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace BetterZoom;

public class CameraSystem : ModSystem
{
	public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
	{
		if (Main.gameMenu) {
			return;
		}

		//Zoom with background if above one
		if (Config.Instance.scaleBackground) {
			Main.BackgroundViewMatrix.Zoom = new Vector2(Main.GameZoomTarget);
		}

		Main.cursorScale = Config.Instance.cursorScale;
	}
}