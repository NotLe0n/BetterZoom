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
				{"zoom", BetterZoom.zoom},
                {"uiscale", BetterZoom.uiScale},
                {"hotbarscale", BetterZoom.hotbarScale},
                {"zoombackground", BetterZoom.zoomBackground}
            };
        }

        public override void Load(TagCompound tag)
        {
            BetterZoom.zoom = tag.GetFloat("zoom");
            BetterZoom.uiScale = tag.GetFloat("uiscale");
            BetterZoom.hotbarScale = tag.GetFloat("hotbarscale");
            BetterZoom.zoomBackground = tag.GetBool("zoombackground");
        }
    }
}
