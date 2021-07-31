using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace BetterZoom.src
{
    class HotkeyLoader : ModSystem
    {
        public override void Load()
        {
            Hotkeys.LockScreen = KeybindLoader.RegisterKeybind(Mod, "Lock Screen", Keys.L);
            Hotkeys.SetTracker = KeybindLoader.RegisterKeybind(Mod, "Set Tracker", Keys.K);
            Hotkeys.RemoveTracker = KeybindLoader.RegisterKeybind(Mod, "Remove Tracker", Keys.O);
            Hotkeys.ShowUI = KeybindLoader.RegisterKeybind(Mod, "Show UI", Keys.B);
        }

        public override void Unload()
        {
            Hotkeys.LockScreen =
            Hotkeys.SetTracker =
            Hotkeys.ShowUI =
            Hotkeys.RemoveTracker = null;
        }
    }
}
