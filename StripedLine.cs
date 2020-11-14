using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;

namespace BetterZoom
{
    class StripedLine : UIElement
    {
        public Vector2 StartPoint;
        public Vector2 EndPoint;

        public StripedLine(Vector2 start, Vector2 end)
        {
            StartPoint = start - Main.screenPosition;
            EndPoint = end - Main.screenPosition;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 edge = EndPoint - StartPoint;

            // calculate angle to rotate line
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            spriteBatch.Draw(Main.magicPixel,
                new Rectangle(          // rectangle defines shape of line and position of start of line
                    (int)StartPoint.X,  // Start
                    (int)StartPoint.Y,  // End
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    5),                 //width of line, change this to make thicker line
                null,
                Color.Red,              //colour of line
                angle,                  //angle of line (calulated above)
                new Vector2(0, 0),      // point in line about which to rotate
                SpriteEffects.None,
                0);
        }
    }
}
