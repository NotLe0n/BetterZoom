using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BetterZoom.src
{
    class SaveLoad : ModPlayer
    {
        public override TagCompound Save()
        {
            return new TagCompound {
				// {"somethingelse", somethingelse}, // To save more data, add additional lines
				{"zoom", BetterZoom.Zoom},
                {"uiscale", BetterZoom.UIScale},
                {"hotbarscale", BetterZoom.HotbarScale},
                {"zoombackground", BetterZoom.ZoomBackground}
            };
        }

        public override void Load(TagCompound tag)
        {
            BetterZoom.Zoom = tag.GetFloat("zoom");
            BetterZoom.UIScale = tag.GetFloat("uiscale");
            BetterZoom.HotbarScale = tag.GetFloat("hotbarscale");
            BetterZoom.ZoomBackground = tag.GetBool("zoombackground");
        }
    }
}
