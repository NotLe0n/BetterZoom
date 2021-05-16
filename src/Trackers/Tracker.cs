using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;

namespace BetterZoom.src.Trackers
{
    class Tracker : UIImage
    {
        /// <summary>
        /// Position in World Coordinates
        /// </summary>
        public Vector2 Position;

        public Tracker(Vector2 position, Texture2D img) : base(img)
        {
            Position = position;
            ImageScale = 0.5f;
        }

        public virtual void FixPosition() { }

        public virtual void RemoveTracker() { }
    }
}
