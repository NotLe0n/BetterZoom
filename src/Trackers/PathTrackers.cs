using BetterZoom.src.UI;
using BetterZoom.src.UI.UIElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BetterZoom.src.Trackers
{
    class PathTrackers : Tracker
    {
        public BezierCurve Connection;

        public PathTrackers(Vector2 pos) : base(pos, ModContent.Request<Texture2D>("BetterZoom/Assets/PathTracker").Value)
        {
            TrackerUI.trackers.Add(this);

            CreateCurve();
        }
        public void CreateCurve() // Broken
        {
            Connection = new BezierCurve(
                Position,
                Position,
                5, Color.Red);
        }
        public override void FixPosition()
        {
            MarginLeft = Position.X - Main.screenPosition.X - Width.Pixels / 2;
            MarginTop = Position.Y - Main.screenPosition.Y - Height.Pixels / 2;
        }
        public static void FixLinePosition()
        {
            // Fix Line Position
            for (int i = 0; i < TrackerUI.trackers.Count; i++)
            {
                if (TrackerUI.trackers[i].Connection != null && TrackerUI.trackers[i] != null)
                {
                    TrackerUI.trackers[i].Connection.StartPoint = (TrackerUI.trackers[i].Position - Main.screenPosition);

                    if (i + 1 < TrackerUI.trackers.Count)
                    {
                        TrackerUI.trackers[i].Connection.EndPoint = (TrackerUI.trackers[i + 1].Position - Main.screenPosition);
                    }
                    else
                    {
                        TrackerUI.trackers[i].Connection.Remove();
                    }
                }
            }
        }
        public override void RemoveTracker()
        {
            Connection?.Remove();
            Connection.ControlPoint?.Remove();
            Remove();
            TrackerUI.trackers.Remove(this);
        }
    }
}
