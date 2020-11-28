using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterZoom.src.Trackers
{
    class ControlPoint : UIElement
    {
        public Vector2 Position;
        public UIImage image = new UIImage(ModContent.GetTexture("BetterZoom/Assets/PathTracker"));
        public ControlPoint(Vector2 position)
        {
            Position = position;
            image.OnMouseDown += ControlPoint_OnMouseDown;
            image.OnMouseUp += ControlPoint_OnMouseUp;
            image.ImageScale = 0.5f;
        }
        public static void FixPosition()
        {
            for (int i = 0; i < PathTrackers.trackers.Count; i++)
            {
                PathTrackers.trackers[i].Connection.ControlPoint.image.MarginLeft = PathTrackers.trackers[i].Connection.ControlPoint.Position.X - Main.screenPosition.X - PathTrackers.trackers[i].Connection.ControlPoint.image.Width.Pixels / 2;
                PathTrackers.trackers[i].Connection.ControlPoint.image.MarginTop = PathTrackers.trackers[i].Connection.ControlPoint.Position.Y - Main.screenPosition.Y - PathTrackers.trackers[i].Connection.ControlPoint.image.Height.Pixels / 2;
            }
        }

        #region dragging code
        private Vector2 offset;
        public bool dragging;
        public static Vector2 lastPos = new Vector2(600, 200);
        public void ControlPoint_OnMouseDown(UIMouseEvent evt, UIElement elm)
        {
            offset = new Vector2(evt.MousePosition.X - image.Left.Pixels, evt.MousePosition.Y - image.Top.Pixels);
            dragging = true;
        }

        public void ControlPoint_OnMouseUp(UIMouseEvent evt, UIElement elm)
        {
            dragging = false;

            image.Left.Set(evt.MousePosition.X - offset.X, 0f);
            image.Top.Set(evt.MousePosition.Y - offset.Y, 0f);
            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // don't remove.
            Position = new Vector2(image.Left.Pixels, image.Top.Pixels);
            // Checking ContainsPoint and then setting mouseInterface to true is very common. This causes clicks on this UIElement to not cause the player to use current items. 
            if (image.ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;


            if (dragging)
            {
                image.Left.Set(Main.mouseX - offset.X, 0f);
                image.Top.Set(Main.mouseY - offset.Y, 0f);
                image.Recalculate();

                lastPos = new Vector2(image.Left.Pixels, image.Top.Pixels);
            }

            // Here we check if the DragableUIPanel is outside the Parent UIElement rectangle. 
            // (In our example, the parent would be ExampleUI, a UIState. This means that we are checking that the DragableUIPanel is outside the whole screen)
            // By doing this and some simple math, we can snap the panel back on screen if the user resizes his window or otherwise changes resolution.
            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!image.GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                image.Left.Pixels = Utils.Clamp(image.Left.Pixels, 0, parentSpace.Right - image.Width.Pixels);
                image.Top.Pixels = Utils.Clamp(image.Top.Pixels, 0, parentSpace.Bottom - image.Height.Pixels);
                // Recalculate forces the UI system to do the positioning math again.
                image.Recalculate();
            }
        }
        #endregion
    }
}
