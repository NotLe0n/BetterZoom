using Newtonsoft.Json;
using System.ComponentModel;
using Terraria;
using Terraria.ModLoader.Config;

namespace BetterZoom
{
    class Config : ModConfig
    {
        public static Config Instance;
        public override ConfigScope Mode => ConfigScope.ClientSide;
        [Slider]
        [Range(-1f, 10f)]
        [DefaultValue(1)]
        [Label("Zoom")]
        [Tooltip("Change this to modify the zoom (you can also press the + and - keys)")]
        public float ZoomValue;

        [Label("Lock Screen")]
        [Tooltip("Lock the screen to a single Position")]
        [DefaultValue(false)]
        public bool LockScreen;

        [Label("Zoom Background")]
        [Tooltip("Toggle to allow the background to zoom with the game")]
        [DefaultValue(false)]
        public bool ZoomBackground;

        [Label("Flip Background")]
        [Tooltip("Toggle to flip the background if your zoom is negative")]
        [DefaultValue(true)]
        public bool FlipBackground;

        [Slider]
        [Range(0.2f, 5f)]
        [DefaultValue(1)]
        [Label("Hotbar scale")]
        [Tooltip("Change this to modify the size of your hotbar")]
        public float HotbarScale;

        [Slider]
        [Range(0.2f, 2f)]
        [DefaultValue(1)]
        [Label("UI scale")]
        [Tooltip("Change this to change the UIScale. This Window updates little wonky, close and open again after changing the value")]
        public float UIScale;

        [Label("Show Trackers")]
        [Tooltip("If Trackers are shown or not")]
        [DefaultValue(true)]
        public bool ShowTrackers;

        [Header("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nWhen you zoom out the tiles don't load any further. This is not a bug")]
        [Label("Understand?")]
        [Tooltip("Changing this doesn't do anything. This is just there to add the text")]
        [JsonIgnore]
        public bool dummy;
        public override void OnChanged()
        {
            BetterZoomPlayer.fixedscreen = Main.screenPosition;
        }
    }
}
