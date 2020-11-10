using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterZoom
{
    class UI : UIState
    {
        List<UIImage> PTrackerImgs = new List<UIImage> { };
        UIImage ETrackerImg = new UIImage(ModContent.GetTexture("BetterZoom/EntityTracker")) { MarginLeft = Main.LocalPlayer.position.X - Main.screenPosition.X, MarginTop = Main.LocalPlayer.position.Y - Main.screenPosition.Y };
        public override void OnInitialize()
        {
            ETrackerImg.ImageScale = 0.5f;
            Append(ETrackerImg);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var screen = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);

            if (EntityTracker.TrackedNPC != null && screen.Contains(EntityTracker.currentPos.ToPoint()) && EntityTracker.tracker != null)
            {
                                                         // relative to screen   // center screen   // center image               // center Entity
                ETrackerImg.MarginLeft = EntityTracker.currentPos.X - screen.X + screen.Width / 2 - ETrackerImg.Width.Pixels / 2 + EntityTracker.TrackedNPC.width / 2;
                ETrackerImg.MarginTop = EntityTracker.currentPos.Y - screen.Y + screen.Height / 2 - ETrackerImg.Width.Pixels / 2 + EntityTracker.TrackedNPC.height / 2;
            }
            for (int i = 0; i < PathTrackers.trackers.Count; i++)
            {
                if (PTrackerImgs.Count <= PathTrackers.trackers.Count)
                {
                    UIImage PTrackerImg = new UIImage(ModContent.GetTexture("BetterZoom/PathTracker"))
                    {
                        MarginLeft = PathTrackers.trackers[i].Position.X - Main.screenPosition.X - Width.Pixels / 2,
                        MarginTop = PathTrackers.trackers[i].Position.Y - Main.screenPosition.Y - Height.Pixels / 2,
                        ImageScale = 0.5f
                    };
                    Append(PTrackerImg);
                    PTrackerImgs.Add(PTrackerImg);
                }
            }
        }
    }
}
