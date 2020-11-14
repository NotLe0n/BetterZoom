using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace BetterZoom
{
    class EntityTracker
    {
        public static NPC TrackedNPC;
        public static Vector2 spawnPos;
        public static Vector2 currentPos;
        public static EntityTracker tracker;
        public static UIImage ETrackerImg = new UIImage(ModContent.GetTexture("BetterZoom/EntityTracker")) { MarginLeft = Main.LocalPlayer.position.X - Main.screenPosition.X, MarginTop = Main.LocalPlayer.position.Y - Main.screenPosition.Y };

        public EntityTracker(Vector2 pos)
        {
            tracker = this;
            spawnPos = currentPos = pos;
            ETrackerImg.ImageScale = 0.5f;

            int NPCID = 0;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && (NPCID == 0 || Main.npc[i].Hitbox.Distance(pos) < Main.npc[NPCID].Hitbox.Distance(pos)))
                {
                    NPCID = i;
                    TrackedNPC = Main.npc[i];
                }
            }
        }
        public static void RemoveTracker()
        {
            tracker = null;
            TrackedNPC = null;
            spawnPos = Main.LocalPlayer.position;
            currentPos = Main.LocalPlayer.position;
        }
    }
}
