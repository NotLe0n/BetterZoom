using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BetterZoom.src
{
    public class BetterZoom : Mod
    {
        public static Vector2 RealMouseWorld => Main.GameViewMatrix.Translation + Main.screenPosition + (Main.MouseScreen / Zoom);
        public static float Zoom 
        {
            get => Main.LocalPlayer.GetModPlayer<SaveLoad>().zoom;
            set => Main.LocalPlayer.GetModPlayer<SaveLoad>().zoom = value; 
        }
        public static float UIScale
        {
            get => Main.LocalPlayer.GetModPlayer<SaveLoad>().uIScale;
            set => Main.LocalPlayer.GetModPlayer<SaveLoad>().uIScale = value;
        }
        public static float HotbarScale
        {
            get => Main.LocalPlayer.GetModPlayer<SaveLoad>().hotbarScale;
            set => Main.LocalPlayer.GetModPlayer<SaveLoad>().hotbarScale = value;
        }
        public static bool ZoomBackground
        {
            get => Main.LocalPlayer.GetModPlayer<SaveLoad>().zoomBackground;
            set => Main.LocalPlayer.GetModPlayer<SaveLoad>().zoomBackground = value;
        }

        public override void Unload()
        {
            Config.Instance = null;
        }
    }
}