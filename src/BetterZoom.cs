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
    public class BetterZoom : Mod
    {
        // Hotkeys
        public static ModHotKey LockScreen, SetTracker, RemoveTracker, ShowUI;

        // UI
        internal UserInterface UserInterface;
        internal UserInterface TrackerUserInterface;
        private UIState UI;

        /// <summary>
        /// Load Hotkeys and UI
        /// </summary>
        public override void Load()
        {
            // Hotkeys
            LockScreen = RegisterHotKey("Lock Screen", "L");
            SetTracker = RegisterHotKey("Set Tracker", "K");
            RemoveTracker = RegisterHotKey("Remove Tracker", "O");
            ShowUI = RegisterHotKey("Show UI", "B");
            // UI
            if (!Main.dedServ)
            {
                UI = new BZUI();
                UI.Activate();
                UserInterface = new UserInterface();
                UserInterface.SetState(null);

                TrackerUserInterface = new UserInterface();
                TrackerUserInterface.SetState(new TrackerUI());
            }
        }

        public override void Unload()
        {
            // Other static Fields
            Trackers.EntityTracker.TrackedEntity = null;
            Trackers.EntityTracker.tracker = null;
            Camera.locked = false;

            // UI
            UserInterface = null;
            UI = null;
            TrackerUserInterface = null;

            // Hotkeys
            LockScreen =
            SetTracker =
            ShowUI =
            RemoveTracker = null;
        }

        private GameTime _lastUpdateUiGameTime;
        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (UserInterface.CurrentState != null)
                UserInterface.Update(gameTime);
            if (!TrackerUI.hide)
                TrackerUserInterface.Update(gameTime);
        }

        /// <summary>
        /// Add UI
        /// </summary>
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Better Zoom: UI",
                    delegate
                    {
                        if (UserInterface.CurrentState != null)
                            UserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        return true;
                    }, InterfaceScaleType.UI));
            }

            int RulerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Ruler"));
            if (RulerIndex != -1)
            {
                layers.Insert(RulerIndex, new LegacyGameInterfaceLayer(
                    "Better Zoom: TrackerUI",
                    delegate
                    {
                        if (!TrackerUI.hide)
                            TrackerUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        return true;
                    }, InterfaceScaleType.Game));
            }
        }
        public static float zoom = 1, uiScale = 1, hotbarScale = 1;
        public static bool flipBackground = true, zoomBackground = false;

        /// <summary>
        /// Change Zoom
        /// </summary>
        /// <param name="Transform">Screen Transform Matrix</param>
        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            if (!Main.gameMenu)
            {
                if (zoom != 1)
                {
                    // Between -1 and 10
                    zoom = MathHelper.Clamp(zoom, -1, 10);

                    //prevent crash
                    if (zoom >= -0.18f && zoom <= 0.18f
                        && !(zoom <= -0.2f)
                        && !Main.keyState.IsKeyDown(Keys.OemPlus))
                    {
                        zoom = -0.2f;
                    }
                    if (zoom >= -0.18f && zoom <= 0.18f
                        && !(zoom <= -0.2f)
                        && Main.keyState.IsKeyDown(Keys.OemPlus))
                    {
                        zoom = 0.2f;
                    }

                    //Change zoom
                    Main.GameZoomTarget = zoom;
                    Transform.Zoom = new Vector2(Main.GameZoomTarget);

                    //Flip background if below zero
                    if (flipBackground && zoom < 0)
                        Main.BackgroundViewMatrix.Zoom = new Vector2(-1, -1);

                    //Zoom with background if above one
                    if (zoomBackground)
                        Main.BackgroundViewMatrix.Zoom = new Vector2(Main.GameZoomTarget);
                }
                // change hotbar scale
                if (hotbarScale != 1f)
                {
                    float[] scale = { hotbarScale, hotbarScale, hotbarScale, hotbarScale, hotbarScale, hotbarScale, hotbarScale, hotbarScale, hotbarScale, hotbarScale }; // for each hotbar slot
                    Main.hotbarScale = scale;
                }
                if (uiScale != 1)
                {
                    // between 0.2 and 2
                    uiScale = MathHelper.Clamp(uiScale, 0.2f, 2);

                    // change UI scale
                    Main.UIScale = uiScale;
                }
            }
        }
    }
}