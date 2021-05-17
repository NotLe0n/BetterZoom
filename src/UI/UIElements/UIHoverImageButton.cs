using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace BetterZoom.src.UI.UIElements
{
    internal class UIHoverImageButton : UIImageButton
    {
        internal string HoverText;
        internal string Texture;
        public UIHoverImageButton(string texture, string hoverText) : base(ModContent.GetTexture(texture))
        {
            HoverText = hoverText;
            Texture = texture;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (IsMouseHovering)
            {
                Main.hoverItemName = HoverText;
            }
            if (ContainsPoint(Main.MouseScreen)) //so you can't use items while clicking the button
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
    }
}