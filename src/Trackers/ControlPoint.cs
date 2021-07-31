using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public new bool IsMouseHovering => new Rectangle((int)Position.X - (int)(8 * BetterZoom.zoom), (int)Position.Y - (int)(8 * BetterZoom.zoom), (int)(16 * BetterZoom.zoom), (int)(16 * BetterZoom.zoom)).Contains(BetterZoom.RealMouseWorld.ToPoint());

        public ControlPoint(Vector2 position) : base(ModContent.Request<Texture2D>("BetterZoom/Assets/ControlPoint"))
        {
            Position = position;
        }

        public bool dragging;
        public override void Update(GameTime gameTime)
        {
            // Fix Position to world
            MarginLeft = Position.X - Main.screenPosition.X - Width.Pixels / 2;
            MarginTop = Position.Y - Main.screenPosition.Y - Height.Pixels / 2;

            if (IsMouseHovering && Main.mouseLeft)
            {
                dragging = true;
            }
            if (Main.mouseLeftRelease)
            {
                dragging = false;
            }

            if (ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;

            if (dragging)
            {
                Position = BetterZoom.RealMouseWorld;
                Recalculate();
            }
        }
    }
}
