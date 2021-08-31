using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BetterZoom.src
{
    class SaveLoad : ModPlayer
    {
        public float zoom = 1;
        public float uIScale = 1;
        public float hotbarScale = 1;
        public bool zoomBackground = false;

        public override TagCompound Save()
        {
            return new TagCompound {
				{"zoom", BetterZoom.Zoom},
                {"uiscale", BetterZoom.UIScale},
                {"hotbarscale", BetterZoom.HotbarScale},
                {"zoombackground", BetterZoom.ZoomBackground}
            };
        }

        public override void Load(TagCompound tag)
        {
            zoom = tag.GetFloat("zoom");
            uIScale = tag.GetFloat("uiscale");
            hotbarScale = tag.GetFloat("hotbarscale");
            zoomBackground = tag.GetBool("zoombackground");
        }
    }
}
