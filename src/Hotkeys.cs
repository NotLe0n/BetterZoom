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
        // Hotkeys
        public static ModKeybind LockScreen, SetTracker, RemoveTracker, ShowUI;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // Toggles Lock Screen
            if (LockScreen.JustPressed)
            {
                Camera.ToggleLock();
            }
            // New Path Tracker
            if (SetTracker.JustPressed)
            {
                var tracker = new PathTrackers(Main.MouseWorld);
                ModContent.GetInstance<UISystem>().trackerUI.Append(tracker);
            }
            // Removes Path Tracker
            if (RemoveTracker.JustPressed)
            {
                for (int i = 0; i < TrackerUI.trackers.Count; i++)
                {
                    if (TrackerUI.trackers[i].IsMouseHovering)
                        TrackerUI.trackers[i].RemoveTracker();
                }
            }
            if (ShowUI.JustPressed)
            {
                var ui = ModContent.GetInstance<UISystem>().userInterface;
                ModContent.GetInstance<UISystem>().userInterface.SetState(ui.CurrentState == null ? UI.UIElements.TabPanel.lastTab : null);

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
                    BetterZoom.UIScale -= 0.01f;
                }
                if (Main.keyState.IsKeyDown(Keys.OemPlus))
                {
                    BetterZoom.UIScale += 0.01f;
                }
            }
            // Control Zoom
            else if (!(Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift)))
            {
                if (Main.keyState.IsKeyDown(Keys.OemMinus))
                {
                    BetterZoom.Zoom -= 0.01f;
                }
                if (Main.keyState.IsKeyDown(Keys.OemPlus))
                {
                    BetterZoom.Zoom += 0.01f;
                }
            }
        }
    }
}
