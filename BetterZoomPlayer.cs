using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace BetterZoom
{
    class BetterZoomPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {

            // Toggles Lock Screen
            if (BetterZoom.LockScreen.JustPressed)
            {
                fixedscreen = Main.screenPosition;
                Config.Instance.LockScreen = !Config.Instance.LockScreen;
                EntityTracker.RemoveTracker();
            }
            // New Path Tracker
            if (BetterZoom.SetTracker.JustPressed)
            {
                new PathTrackers(Main.MouseWorld);
            }
            // New Entity Tracker
            if (BetterZoom.EntityTracker.JustPressed && Config.Instance.LockScreen)
            {
                new EntityTracker(Main.MouseWorld);
            }
            // Removes Entity Tracker
            if (BetterZoom.RemoveETracker.JustPressed)
            {
                EntityTracker.RemoveTracker();
            }
            // Control screen Position
            if (Config.Instance.LockScreen)
            {
                if (Main.keyState.IsKeyDown(Keys.Up))
                    fixedscreen += new Vector2(0, -5);

                if (Main.keyState.IsKeyDown(Keys.Down))
                    fixedscreen += new Vector2(0, 5);

                if (Main.keyState.IsKeyDown(Keys.Left))
                    fixedscreen += new Vector2(-5, 0);

                if (Main.keyState.IsKeyDown(Keys.Right))
                    fixedscreen += new Vector2(5, 0);

            }
            // Control UI Scale
            if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift))
            {
                if (Main.keyState.IsKeyDown(Keys.OemMinus))
                {
                    Config.Instance.UIScale -= 0.01f;
                }
                if (Main.keyState.IsKeyDown(Keys.OemPlus))
                {
                    Config.Instance.UIScale += 0.01f;
                }
            }
            // Control Zoom
            else if (!(Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift)))
            {
                if (Main.keyState.IsKeyDown(Keys.OemMinus))
                {
                    Config.Instance.ZoomValue -= 0.01f;
                }
                if (Main.keyState.IsKeyDown(Keys.OemPlus))
                {
                    Config.Instance.ZoomValue += 0.01f;
                }
            }
        }
        public static Vector2 fixedscreen = Main.LocalPlayer.position - new Vector2(Main.screenWidth, Main.screenHeight);
        public override void ModifyScreenPosition()
        {
            if (!Main.gameMenu && Config.Instance.LockScreen)
            {
                // Lock screen
                Main.screenPosition = fixedscreen;

                // Lock screen to Entity
                if (EntityTracker.tracker != null)
                {
                    EntityTracker.currentPos = EntityTracker.TrackedNPC.position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
                    Main.screenPosition = EntityTracker.currentPos;
                }
            }
        }
    }
}
