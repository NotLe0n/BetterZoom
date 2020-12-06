using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace BetterZoom.src.UI.UIElements
{
    class DragableUIPanel : UIPanel
    {
        public bool active = false;
        public DragableUIPanel(float width, float height)
        {
            Width.Set(width, 0f);
            Height.Set(height, 0f);
            SetPadding(0);

            var header = new UIPanel();
            header.SetPadding(0);
            header.Width = Width;
            header.Height.Set(30, 0f);
            header.BackgroundColor.A = 255;
            header.OnMouseDown += Header_OnMouseDown;
            header.OnMouseUp += Header_OnMouseUp;
            Append(header);

            var closeBtn = new UITextPanel<char>('X');
            closeBtn.SetPadding(7);
            closeBtn.Width.Set(40, 0);
            closeBtn.Left.Set(0, 0.9f);
            closeBtn.BackgroundColor.A = 255;
            closeBtn.OnClick += (evt, elm) => { Remove(); };
            header.Append(closeBtn);
        }
        #region Drag code yoiked from ExampleMod 
        public override void OnDeactivate()
        {
            active = false;
            RemoveAllChildren();
            Remove();
        }
        public override void OnActivate() => active = true;

        private Vector2 offset;
        public bool dragging;
        public void Header_OnMouseDown(UIMouseEvent evt, UIElement elm)
        {
            base.MouseDown(evt);
            offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
            dragging = true;
        }

        public void Header_OnMouseUp(UIMouseEvent evt, UIElement elm)
        {
            base.MouseUp(evt);
            dragging = false;

            Left.Set(evt.MousePosition.X - offset.X, 0f);
            Top.Set(evt.MousePosition.Y - offset.Y, 0f);
            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // don't remove.

            // Checking ContainsPoint and then setting mouseInterface to true is very common. This causes clicks on this UIElement to not cause the player to use current items. 
            if (ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;


            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0f);
                Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();
            }

            // Here we check if the DragableUIPanel is outside the Parent UIElement rectangle. 
            // (In our example, the parent would be ExampleUI, a UIState. This means that we are checking that the DragableUIPanel is outside the whole screen)
            // By doing this and some simple math, we can snap the panel back on screen if the user resizes his window or otherwise changes resolution.
            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                // Recalculate forces the UI system to do the positioning math again.
                Recalculate();
            }
        }
        #endregion
    }
}
