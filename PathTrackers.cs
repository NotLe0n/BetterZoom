using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace BetterZoom
{
    class PathTrackers
    {
        public static List<PathTrackers> trackers = new List<PathTrackers> { };
        public static List<UIImage> PTrackerImgs = new List<UIImage> { };

        public StripedLine Connection;

        public Vector2 Position;
        public PathTrackers(Vector2 pos)
        {
            Position = pos;
            trackers.Add(this);

            UIImage PTrackerImg = new UIImage(ModContent.GetTexture("BetterZoom/PathTracker"));
            PTrackerImg.ImageScale = 0.5f;
            PTrackerImgs.Add(PTrackerImg);

            for (int i = 0; i < trackers.Count; i++)
            {
                Connection = new StripedLine(
                trackers[i].Position,
                trackers[i].Position);
            }
        }
    }
}
