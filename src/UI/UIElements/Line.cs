using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.UI;

namespace BetterZoom.src.UI.UIElements
{
    class Line : UIElement
    {
        public Vector2 StartPoint;
        public Vector2 EndPoint;
        public int LineWidth;
        public Color LineColor;
        public Rectangle Bounds;

        /// <summary>
        /// Creates a Line from A to B.
        /// </summary>
        /// <param name="start">The start position of the line. "Point A"</param>
        /// <param name="end">The end position of the line. "Point B"</param>
        /// <param name="width">The width of the line</param>
        /// <param name="color">The color of the line</param>
        public Line(Vector2 start, Vector2 end, int width, Color color)
        {
            StartPoint = start;
            EndPoint = end;
            LineWidth = width;
            LineColor = color;
            Bounds = new Rectangle((int)start.X, (int)start.Y, (int)(end - start).Length(), width);
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
            Vector2 edge = EndPoint - StartPoint;

            spriteBatch.Draw(Main.magicPixel,
                new Rectangle(          // rectangle defines shape of line and position of start of line
                    (int)StartPoint.X,  // Start
                    (int)StartPoint.Y,  // End
                    (int)edge.Length(), // sb will strech the texture to fill this rectangle
                    LineWidth),         // width of line, change this to make thicker line
                null,
                LineColor,              // colour of line
                Angle(),                // angle of line (calulated above)
                new Vector2(0, 0),      // point in line about which to rotate
                SpriteEffects.None,
                0);
        }
    }
}
