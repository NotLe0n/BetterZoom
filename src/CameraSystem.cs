using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace BetterZoom;

public sealed class CameraSystem : ModSystem
{
	private static readonly Config config = ModContent.GetInstance<Config>();
	
	public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
	{
		if (Main.gameMenu) {
			return;
		}

		//Zoom with background if above one
		if (config.scaleBackground) {
			Main.BackgroundViewMatrix.Zoom = new Vector2(Main.GameZoomTarget);
		}

		Main.cursorScale = config.cursorScale;
	}
}