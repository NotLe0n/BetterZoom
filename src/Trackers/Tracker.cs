using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace BetterZoom.src.Trackers
{
    abstract class Tracker : UIImage
    {
        /// <summary>
        /// Position in World Coordinates
        /// </summary>
        public Vector2 Position;
        public new bool IsMouseHovering => new Rectangle((int)Position.X - (int)(16 * BetterZoom.Zoom), (int)Position.Y - (int)(16 * BetterZoom.Zoom), (int)(32 * BetterZoom.Zoom), (int)(32 * BetterZoom.Zoom)).Contains(BetterZoom.RealMouseWorld.ToPoint());

        public Tracker(Vector2 position, Texture2D img) : base(img)
        {
            Position = position;
            ImageScale = 0.5f;
        }

        public virtual void FixPosition() { }

        public virtual void RemoveTracker() { }
    }
}
