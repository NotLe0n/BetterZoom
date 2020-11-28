using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterZoom.src.Trackers
{
    class EntityTracker
    {
        public static Entity TrackedEntity;
        public static Vector2 Position;
        public static EntityTracker tracker;
        public static UIImage ETrackerImg = new UIImage(ModContent.GetTexture("BetterZoom/Assets/EntityTracker")) { MarginLeft = Main.LocalPlayer.position.X - Main.screenPosition.X, MarginTop = Main.LocalPlayer.position.Y - Main.screenPosition.Y };

        public EntityTracker(Vector2 pos)
        {
            tracker = this;
            Position = pos;
            ETrackerImg.ImageScale = 0.5f;

            TrackedEntity = FindClosest(pos);
        }
        private Entity FindClosest(Vector2 pos)
        {
            int NPCID = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].Hitbox.Distance(pos) < Main.npc[NPCID].Hitbox.Distance(pos))
                {
                    NPCID = i;
                }
            }

            int PlayerID = 0;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && Main.player[i].Hitbox.Distance(pos) < Main.player[PlayerID].Hitbox.Distance(pos))
                    {
                        PlayerID = i;
                    }
                }

                return Main.player[PlayerID].Hitbox.Distance(pos) < Main.npc[NPCID].Hitbox.Distance(pos) && Main.player[PlayerID] != Main.player[Main.myPlayer]
                    ? Main.player[PlayerID]
                    : (Entity)Main.npc[NPCID];
            }
            else
            {
                return Main.npc[NPCID];
            }
        }
        public static void RemoveTracker()
        {
            Camera.locked = false;
            tracker = null;
            TrackedEntity = null;
            Position = Main.LocalPlayer.position;
            ETrackerImg.Remove();
        }
        public static void FixPosition()
        {
            // screen dimentions
            var screen = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);

            // Fix Entity Tracker Position
            if (TrackedEntity != null && screen.Contains(Position.ToPoint()) && tracker != null)
            {
                // relative to screen   // center screen   // center image               // center Entity
                ETrackerImg.MarginLeft = Position.X - screen.X + screen.Width / 2 - ETrackerImg.Width.Pixels / 2 + TrackedEntity.width / 2;
                ETrackerImg.MarginTop = Position.Y - screen.Y + screen.Height / 2 - ETrackerImg.Height.Pixels / 2 + TrackedEntity.height / 2;
            }
        }
    }
}
