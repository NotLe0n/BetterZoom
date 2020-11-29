using BetterZoom.src.Trackers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;

namespace BetterZoom.src.UI.UIElements
{
    class BezierCurve : UIElement
    {
        public Vector2 StartPoint;
        public Vector2 EndPoint;
        public int LineWidth;
        public Color LineColor;
        public Rectangle Bounds;
        public ControlPoint ControlPoint;
        public List<Vector2> PointList = new List<Vector2> { };

        /// <summary>
        /// Creates a Line from A to B.
        /// </summary>
        /// <param name="start">The start position of the line. "Point A"</param>
        /// <param name="end">The end position of the line. "Point B"</param>
        /// <param name="width">The width of the line</param>
        /// <param name="color">The color of the line</param>
        public BezierCurve(Vector2 start, Vector2 end, int width, Color color)
        {
            StartPoint = start;
            EndPoint = end;
            LineWidth = width;
            LineColor = color;
            Bounds = new Rectangle((int)start.X, (int)start.Y, (int)(end - start).Length(), width);
            ControlPoint = new ControlPoint(end);
        }
        private void CalculatePoints()
        {
            PointList.Clear();
            for (float t = 0; t <= 1; t += 0.03f)
                PointList.Add((StartPoint - 2 * ControlPoint.Position + EndPoint) * (float)Math.Pow(t, 2) + (-2 * StartPoint + 2 * ControlPoint.Position) * t + StartPoint);
        }
        /// <summary>
        /// The Length of the line.
        /// </summary>
        public float Length() => Vector2.Distance(StartPoint, EndPoint);

        /// <summary>
        /// The Angle of the Line
        /// </summary>
        public float Angle() => (float)Math.Atan2(EndPoint.Y - StartPoint.Y, EndPoint.X - StartPoint.X);

        public override void Draw(SpriteBatch spriteBatch)
        {
            CalculatePoints();
            Main.NewText(ControlPoint.Position);
            for (int i = 0; i + 1 < PointList.Count; i++)
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
