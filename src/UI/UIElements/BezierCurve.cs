using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BetterZoom.src.Trackers;

namespace BetterZoom.src.UI.UIElements
{
    class BezierCurve : Line
    {
        public ControlPoint ControlPoint;
        public List<Vector2> PointList = new List<Vector2> { };

        public BezierCurve(Vector2 start, Vector2 end, int width, Color color) : base(start, end, width, color)
        {
            ControlPoint = new ControlPoint(end / 1.5f);
        }
        private void CalculatePoints()
        {
            PointList.Clear();
            for (float t = 0; t <= 1; t += 0.03f)
                PointList.Add((StartPoint - 2 * ControlPoint.Position + EndPoint) * (float)Math.Pow(t, 2) + (-2 * StartPoint + 2 * ControlPoint.Position) * t + StartPoint);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            CalculatePoints();

            for (int i = 0; i + 1 < PointList.Count; i++)
            {
                if (i + 1 < PathTrackers.trackers.Count)
                {
                    float angle = (float)Math.Atan2(PointList[i + 1].Y - PointList[i].Y, PointList[i + 1].X - PointList[i].X);
                    Vector2 edge = PointList[i + 1] - PointList[i];

                    spriteBatch.Draw(Main.magicPixel,
                        new Rectangle(          // rectangle defines shape of line and position of start of line
                            (int)PointList[i].X,  // Start
                            (int)PointList[i].Y,  // End
                            (int)edge.Length(), // sb will strech the texture to fill this rectangle
                            LineWidth),         // width of line, change this to make thicker line
                        null,
                        LineColor,              // colour of line
                        angle,                // angle of line (calulated above)
                        new Vector2(0, 0),      // point in line about which to rotate
                        SpriteEffects.None,
                        0);
                }
            }
        }
    }
}
