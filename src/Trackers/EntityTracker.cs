using BetterZoom.src.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterZoom.src.Trackers
{
    class EntityTracker : Tracker
    {
        public Entity TrackedEntity;

        public EntityTracker(Vector2 pos) : base(pos, ModContent.Request<Texture2D>("BetterZoom/Assets/EntityTracker").Value)
        {
            MarginLeft = Main.LocalPlayer.position.X - Main.screenPosition.X;
            MarginTop = Main.LocalPlayer.position.Y - Main.screenPosition.Y;

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

        public override void RemoveTracker()
        {
            Camera.Locked = false;
            TrackedEntity = null;
            Position = Main.LocalPlayer.position;
            TrackerUI.entityTracker = null;
            Remove();
        }

        public override void FixPosition()
        {
            // screen dimentions
            var screen = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);

            // Fix Entity Tracker Position
            if (TrackedEntity != null && screen.Contains(Position.ToPoint()))
            {
                // relative to screen   // center screen   // center image               // center Entity
                MarginLeft = Position.X - screen.X + screen.Width / 2 - Width.Pixels / 2 + TrackedEntity.width / 2;
                MarginTop = Position.Y - screen.Y + screen.Height / 2 - Height.Pixels / 2 + TrackedEntity.height / 2;
            }
        }
    }
}
