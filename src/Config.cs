using Newtonsoft.Json;
using Terraria.ModLoader.Config;

namespace BetterZoom.src
{
    class Config : ModConfig
    {
        public static Config Instance;
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Better Zoom now uses UI instead of the Config")]
        [Label("Press the hotkey (you have to set it first) to open the UI")]
        [Tooltip("Changing this doesn't do anything. This is just there to add the text")]
        [JsonIgnore]
        public bool dummy2;

        [Header("When you zoom out the tiles don't load any further. This is not a bug")]
        [Label("Understand?")]
        [Tooltip("Changing this doesn't do anything. This is just there to add the text")]
        [JsonIgnore]
        public bool dummy;
    }
}
