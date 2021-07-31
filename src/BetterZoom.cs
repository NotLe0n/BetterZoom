using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BetterZoom.src
{
    public class BetterZoom : Mod
    {
        public static Vector2 RealMouseWorld => Main.GameViewMatrix.Translation + Main.screenPosition + (Main.MouseScreen / Zoom);
        public static float Zoom { get; set; } = 1;
        public static float UIScale { get; set; } = 1;
        public static float HotbarScale { get; set; } = 1;
        public static bool ZoomBackground { get; set; } = false;

        public override void Unload()
        {
            Config.Instance = null;
        }
    }
}