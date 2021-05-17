using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace BetterZoom.src.Trackers
{
    class ControlPoint : UIImage
    {
        /// <summary>
        /// Position in World Coordinates
        /// </summary>
        public Vector2 Position;

        public ControlPoint(Vector2 position) : base(ModContent.GetTexture("BetterZoom/Assets/ControlPoint"))
        {
            Position = position;

            OnMouseDown += (evt, elm) => dragging = true;
            OnMouseUp += (evt, elm) => dragging = false;
        }

        public bool dragging;
        public override void Update(GameTime gameTime)
        {
            // Fix Position to world
            MarginLeft = Position.X - Main.screenPosition.X - Width.Pixels / 2;
            MarginTop = Position.Y - Main.screenPosition.Y - Height.Pixels / 2;

            if (ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;

            if (dragging)
            {
                Position = Main.MouseWorld;
                Recalculate();
            }
        }
    }
}
