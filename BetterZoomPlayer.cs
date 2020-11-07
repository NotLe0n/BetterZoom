using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace BetterZoom
{
    class BetterZoomPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Main.keyState.IsKeyDown(Keys.OemMinus))
            {
                Config.Instance.ZoomValue -= 0.01f;
            }
            if (Main.keyState.IsKeyDown(Keys.OemPlus))
            {
                Config.Instance.ZoomValue += 0.01f;
            }
        }
    }
}
