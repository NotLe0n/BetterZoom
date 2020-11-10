using Microsoft.Xna.Framework;
using Terraria;

namespace BetterZoom
{
    class EntityTracker
    {
        public static NPC TrackedNPC;
        public static Vector2 spawnPos;
        public static Vector2 currentPos;
        public static EntityTracker tracker;
        public EntityTracker(Vector2 pos)
        {
            tracker = this;
            spawnPos = currentPos = pos;

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
