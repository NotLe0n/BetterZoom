using BetterZoom.src.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

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
                if (BetterZoom.zoom != 1)
                {
                    // Journeys end lighting changes make the game crash below 0.64 zoom :(
                    BetterZoom.zoom = MathHelper.Clamp(BetterZoom.zoom, 0.64f, 10);

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
                    Main.GameZoomTarget = BetterZoom.zoom;
                    Transform.Zoom = new Vector2(Main.GameZoomTarget);

                    //Zoom with background if above one
                    if (BetterZoom.zoomBackground)
                        Main.BackgroundViewMatrix.Zoom = new Vector2(Main.GameZoomTarget);
                }
                // change hotbar scale
                if (BetterZoom.hotbarScale != 1f)
                {
                    float[] scale = { BetterZoom.hotbarScale, BetterZoom.hotbarScale, BetterZoom.hotbarScale, BetterZoom.hotbarScale, BetterZoom.hotbarScale, BetterZoom.hotbarScale, BetterZoom.hotbarScale, BetterZoom.hotbarScale, BetterZoom.hotbarScale, BetterZoom.hotbarScale }; // for each hotbar slot
                    Main.hotbarScale = scale;
                }
                if (BetterZoom.uiScale != 1)
                {
                    // between 0.2 and 2
                    BetterZoom.uiScale = MathHelper.Clamp(BetterZoom.uiScale, 0.2f, 2);

                    // change UI scale
                    Main.UIScale = BetterZoom.uiScale;
                }
            }
        }
    }
}
