using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using static BetterZoom.PathTrackers;
using static BetterZoom.EntityTracker;

namespace BetterZoom
{
    class UI : UIState
    {
        public override void OnInitialize()
        {
            Append(ETrackerImg);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // screen dimentions
            var screen = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);

            // Fix Entity Tracker Position
            if (TrackedNPC != null && screen.Contains(currentPos.ToPoint()) && tracker != null)
            {
                                             // relative to screen   // center screen   // center image               // center Entity
                ETrackerImg.MarginLeft = currentPos.X - screen.X + screen.Width / 2 - ETrackerImg.Width.Pixels / 2 + TrackedNPC.width / 2;
                ETrackerImg.MarginTop = currentPos.Y - screen.Y + screen.Height / 2 - ETrackerImg.Height.Pixels / 2 + TrackedNPC.height / 2;
            }
            // Fix Path Tracker Position
            for (int i = 0; i < PTrackerImgs.Count; i++)
            {
                PTrackerImgs[i].MarginLeft = trackers[i].Position.X - screen.X - PTrackerImgs[i].Width.Pixels / 2;
                PTrackerImgs[i].MarginTop = trackers[i].Position.Y - screen.Y - PTrackerImgs[i].Height.Pixels / 2;
                Append(PTrackerImgs[i]);
            }
            // Fix Line Position
            for (int i = 0; i < trackers.Count; i++)
            {
                if (trackers[i].Connection != null)
                {
                    trackers[i].Connection.StartPoint = trackers[i].Position - Main.screenPosition;

                    if (i + 1 < trackers.Count)
                    {
                        trackers[i].Connection.EndPoint = trackers[i + 1].Position - Main.screenPosition;
                    }
                    else
                    {
                        trackers[i].Connection.EndPoint = trackers[i].Connection.StartPoint;
                    }

                    Append(trackers[i].Connection);
                }
            }
        }
    }
}
