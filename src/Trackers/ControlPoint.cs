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
    class ControlPoint : UIImage
    {
        public Vector2 Position;
        public ControlPoint(Vector2 position) : base(ModContent.GetTexture("BetterZoom/Assets/PathTracker"))
        {
            Position = position;
            MarginLeft = (position - Main.screenPosition).X;
            MarginTop = (position - Main.screenPosition).X;

            OnMouseDown += ControlPoint_OnMouseDown;
            OnMouseUp += ControlPoint_OnMouseUp;
            ImageScale = 0.5f;
        }

        #region dragging code
        private Vector2 offset;
        public bool dragging;
        public void ControlPoint_OnMouseDown(UIMouseEvent evt, UIElement elm)
        {
            //Position = Main.MouseWorld;
            Recalculate();
        }

        public void ControlPoint_OnMouseUp(UIMouseEvent evt, UIElement elm)
        {
            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // don't remove.

            /*MarginLeft = Position.X - Main.screenPosition.X;
            MarginTop = Position.Y - Main.screenPosition.Y;
            */
            //Position = new Vector2(MarginLeft + Main.screenPosition.X, MarginTop + Main.screenPosition.Y);
            Recalculate();
        }
        #endregion
    }
}
