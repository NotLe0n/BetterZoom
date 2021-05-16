using BetterZoom.src.Trackers;
using BetterZoom.src.UI;
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
                Camera.ToggleLock();
            }
            // New Path Tracker
            if (BetterZoom.SetTracker.JustPressed)
            {
                var tracker = new PathTrackers(Main.MouseWorld);
                ModContent.GetInstance<BetterZoom>().trackerUI.Append(tracker);
            }
            // Removes Path Tracker
            if (BetterZoom.RemoveTracker.JustPressed)
            {
                for (int i = 0; i < TrackerUI.trackers.Count; i++)
                {
                    if (TrackerUI.trackers[i].IsMouseHovering)
                        TrackerUI.trackers[i].RemoveTracker();
                }
            }
            if (BetterZoom.ShowUI.JustPressed)
            {
                var ui = ModContent.GetInstance<BetterZoom>().userInterface;
                ModContent.GetInstance<BetterZoom>().userInterface.SetState(ui.CurrentState == null ? UI.UIElements.TabPanel.lastTab : null);

            }
            // Control screen Position
            if (Camera.Locked)
            {
                if (Main.keyState.IsKeyDown(Keys.Up))
                    Camera.MoveRelativeTo(new Vector2(0, -5));

                if (Main.keyState.IsKeyDown(Keys.Down))
                    Camera.MoveRelativeTo(new Vector2(0, 5));

                if (Main.keyState.IsKeyDown(Keys.Left))
                    Camera.MoveRelativeTo(new Vector2(-5, 0));

                if (Main.keyState.IsKeyDown(Keys.Right))
                    Camera.MoveRelativeTo(new Vector2(5, 0));

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
