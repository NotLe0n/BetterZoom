using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace BetterZoom.src
{
    public class CameraSystem : ModSystem
    {
        /// <summary>
        /// Change Zoom
        /// </summary>
        /// <param name="Transform">Screen Transform Matrix</param>
        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            if (!Main.gameMenu)
            {
                if (BetterZoom.Zoom != 1)
                {
                    // Journeys end lighting changes make the game crash below 0.64 zoom :(
                    BetterZoom.Zoom = MathHelper.Clamp(BetterZoom.Zoom, 0.64f, 10);

                    //prevent crash
                    /*if (BetterZoom.zoom >= -0.18f && BetterZoom.zoom <= 0.18f
                        && !(BetterZoom.zoom <= -0.2f)
                        && !Main.keyState.IsKeyDown(Keys.OemPlus))
                    {
                        BetterZoom.zoom = -0.2f;
                    }
                    if (BetterZoom.zoom >= -0.18f && BetterZoom.zoom <= 0.18f
                        && !(BetterZoom.zoom <= -0.2f)
                        && Main.keyState.IsKeyDown(Keys.OemPlus))
                    {
                        BetterZoom.zoom = 0.2f;
                    }*/

                    //Change zoom
                    Main.GameZoomTarget = BetterZoom.Zoom;
                    Transform.Zoom = new Vector2(Main.GameZoomTarget);

                    //Zoom with background if above one
                    if (BetterZoom.ZoomBackground)
                        Main.BackgroundViewMatrix.Zoom = new Vector2(Main.GameZoomTarget);
                }
                // change hotbar scale
                if (BetterZoom.HotbarScale != 1f)
                {
                    float[] scale = { BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale, BetterZoom.HotbarScale }; // for each hotbar slot
                    Main.hotbarScale = scale;
                }
                if (BetterZoom.UIScale != 1)
                {
                    // between 0.2 and 2
                    BetterZoom.UIScale = MathHelper.Clamp(BetterZoom.UIScale, 0.2f, 2);

                    // change UI scale
                    Main.UIScale = BetterZoom.UIScale;
                }
            }
        }
    }
}
