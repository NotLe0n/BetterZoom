using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace BetterZoom.src;

public class CameraSystem : ModSystem
{
    public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
    {
        if (!Main.gameMenu)
        {
            //Zoom with background if above one
            if (Config.Instance.scaleBackground) {
				Main.BackgroundViewMatrix.Zoom = new Vector2(Main.GameZoomTarget);
			}

			Main.cursorScale = Config.Instance.cursorScale;

			// change hotbar scale
			/*if (BetterZoom.HotbarScale != 1f)
            {
                float[] scale = { BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale }; // for each hotbar slot
                Main.hotbarScale = scale;
            }*/
		}
    }
}