using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterZoom
{
    class PathTrackers
    {
        public static List<PathTrackers> trackers = new List<PathTrackers> { };
        public Vector2 Position;
        public PathTrackers(Vector2 pos)
        {
            Position = pos;
            trackers.Add(this);
        }
    }
}
