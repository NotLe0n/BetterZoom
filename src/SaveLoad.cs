using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BetterZoom.src
{
    class SaveLoad : ModPlayer
    {
        public float zoom = 1;
        public float uIScale = 1;
        public float hotbarScale = 1;
        public bool zoomBackground;

        // temporary fix to crash nullref on world load
        public override bool IsLoadingEnabled(Mod mod)
        {
            return true;
        }

        public override void Initialize()
        {
            zoom = 1;
            uIScale = 1;
            hotbarScale = 1;
            zoomBackground = false;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["zoom"] = zoom;
            tag["uiscale"] = uIScale;
            tag["hotbarscale"] = hotbarScale;
            tag["zoombackground"] = zoomBackground;
        }

        public override void LoadData(TagCompound tag)
        {
            zoom = tag.GetFloat("zoom");
            uIScale = tag.GetFloat("uiscale");
            hotbarScale = tag.GetFloat("hotbarscale");
            zoomBackground = tag.GetBool("zoombackground");
        }
    }
}
