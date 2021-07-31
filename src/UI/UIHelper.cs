using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterZoom.src.UI
{
    class UIHelper
    {
        public static UIElement[] Dpad(int x, int y)
        {
            return new UIImageButton[] {
                new UIImageButton(ModContent.Request<Texture2D>("BetterZoom/Assets/UpButton"))
                {
                    MarginTop = y,
                    MarginLeft = x
                },

                new UIImageButton(ModContent.Request<Texture2D>("BetterZoom/Assets/DownButton"))
                {
                    MarginTop = y + 40,
                    MarginLeft = x
                },

                new UIImageButton(ModContent.Request<Texture2D>("BetterZoom/Assets/LeftButton"))
                {
                    MarginTop = y + 40,
                    MarginLeft = x - 40
                },

                new UIImageButton(ModContent.Request<Texture2D>("BetterZoom/Assets/RightButton"))
                {
                    MarginTop = y + 40,
                    MarginLeft = x + 40
                }
            };
        }
    }
}
