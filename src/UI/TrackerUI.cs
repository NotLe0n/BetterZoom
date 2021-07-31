using BetterZoom.src.Trackers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.UI;

namespace BetterZoom.src.UI
{
    class TrackerUI : UIState
    {
        public static bool hide;
        public static EntityTracker entityTracker;
        public static List<PathTrackers> trackers = new();

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Fix Entity Tracker Position
            if (entityTracker != null && entityTracker.TrackedEntity != null)
            {
                entityTracker.FixPosition();
            }

            for (int i = 0; i < trackers.Count; i++)
            {
                trackers[i].FixPosition();

                // Append Lines
                if (trackers[i].Connection != null)
                    Append(trackers[i].Connection);

                // Append Control Points
                if (i + 1 < trackers.Count && CCUI.selectedInterp == 2 && trackers[i].Connection != null)
                {
                    Append(trackers[i].Connection.ControlPoint);
                }
            }

            // Fix Line Position
            PathTrackers.FixLinePosition();
        }
    }
}
