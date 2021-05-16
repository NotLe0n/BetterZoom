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
        public static List<PathTrackers> trackers = new List<PathTrackers>();

        public override void OnInitialize()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (entityTracker != null && entityTracker.TrackedEntity != null)
            {
                // Fix Entity Tracker Position
                entityTracker.FixPosition();

                Camera.MoveTo(entityTracker.Position);
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
