using BetterZoom.src.Trackers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace BetterZoom.src
{
    class Hotkeys : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // Toggles Lock Screen
            if (BetterZoom.LockScreen.JustPressed)
            {
                Camera.fixedscreen = Main.screenPosition;
                Camera.locked = !Camera.locked;
            }
            // New Path Tracker
            if (BetterZoom.SetTracker.JustPressed)
            {
                new PathTrackers(Main.MouseWorld);
            }
            // Removes Path Tracker
            if (BetterZoom.RemoveTracker.JustPressed)
            {
                PathTrackers.Remove();
            }
            if (BetterZoom.ShowUI.JustPressed)
            {
                var ui = ModContent.GetInstance<BetterZoom>().UserInterface;
                ModContent.GetInstance<BetterZoom>().UserInterface.SetState(ui.CurrentState == null ? UI.UIElements.Tab.lastTab : null);

            }
            // Control screen Position
            if (Camera.locked)
            {
                if (Main.keyState.IsKeyDown(Keys.Up))
                    Camera.fixedscreen += new Vector2(0, -5);

                if (Main.keyState.IsKeyDown(Keys.Down))
                    Camera.fixedscreen += new Vector2(0, 5);

                if (Main.keyState.IsKeyDown(Keys.Left))
                    Camera.fixedscreen += new Vector2(-5, 0);

                if (Main.keyState.IsKeyDown(Keys.Right))
                    Camera.fixedscreen += new Vector2(5, 0);

            }
            // Control UI Scale
            if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift))
            {
                if (Main.keyState.IsKeyDown(Keys.OemMinus))
                {
                    BetterZoom.uiScale -= 0.01f;
                }
                if (Main.keyState.IsKeyDown(Keys.OemPlus))
                {
                    BetterZoom.uiScale += 0.01f;
                }
            }
            // Control Zoom
            else if (!(Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift)))
            {
                if (Main.keyState.IsKeyDown(Keys.OemMinus))
                {
                    BetterZoom.zoom -= 0.01f;
                }
                if (Main.keyState.IsKeyDown(Keys.OemPlus))
                {
                    BetterZoom.zoom += 0.01f;
                }
            }
        }
    }
}
