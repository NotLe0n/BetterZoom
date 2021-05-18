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

        public static Vector2 RealMouseWorld => Main.GameViewMatrix.Translation + Main.screenPosition + (Main.MouseScreen / zoom);
        public static float zoom = 1;
        public static float uiScale = 1;
        public static float hotbarScale = 1;
        public static bool zoomBackground = false;

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
        }

        public override void Unload()
        {
            Config.Instance = null;

            // Hotkeys
            LockScreen =
            SetTracker =
            ShowUI =
            RemoveTracker = null;
        }
    }
}