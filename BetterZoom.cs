using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterZoom
{
    public class BetterZoom : Mod
    {
        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            if (!Main.gameMenu)
            {
                if (Config.Instance.ZoomValue != 1)
                {
                    Config.Instance.ZoomValue = MathHelper.Clamp(Config.Instance.ZoomValue, -1, 10);
                    //prevent crash
                    if (Config.Instance.ZoomValue >= -0.18f && Config.Instance.ZoomValue <= 0.18f
                        && !(Config.Instance.ZoomValue <= -0.2f)
                        && !Main.keyState.IsKeyDown(Keys.OemPlus))
                    {
                        Config.Instance.ZoomValue = -0.2f;
                    }
                    if (Config.Instance.ZoomValue >= -0.18f && Config.Instance.ZoomValue <= 0.18f
                        && !(Config.Instance.ZoomValue <= -0.2f)
                        && Main.keyState.IsKeyDown(Keys.OemPlus))
                    {
                        Config.Instance.ZoomValue = 0.2f;
                    }
                    //Change zoom
                    Main.GameZoomTarget = Config.Instance.ZoomValue;
                    Transform.Zoom = new Vector2(Main.GameZoomTarget);

                    //Flip background if below zero
                    if (Config.Instance.FlipBackground && Config.Instance.ZoomValue < 0)
                        Main.BackgroundViewMatrix.Zoom = new Vector2(-1, -1);

                    //Zoom with background if above one
                    if (Config.Instance.ZoomBackground)
                        Main.BackgroundViewMatrix.Zoom = new Vector2(Main.GameZoomTarget);
                }

                float scaleval = Config.Instance.HotbarScale;
                if (scaleval != 1f)
                {
                    float[] scale = { scaleval, scaleval, scaleval, scaleval, scaleval, scaleval, scaleval, scaleval, scaleval, scaleval, };
                    Main.hotbarScale = scale;
                }
                if (Config.Instance.UIScale != 1)
                {
                    Main.UIScale = Config.Instance.UIScale;
                }
            }
        }
    }
    public class CamPosition : ModPlayer
    {
        public static Vector2 fixedscreen;
        public override void ModifyScreenPosition()
        {
            if (!Main.gameMenu && Config.Instance.LockScreen)
                Main.screenPosition = fixedscreen;
        }
    }
}