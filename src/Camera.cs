using BetterZoom.src.Trackers;
using BetterZoom.src.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BetterZoom.src
{
    class Camera : ModPlayer
    {
        public static Vector2 fixedscreen = Main.LocalPlayer.position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
        public static bool Playing = false;
        public static float k;
        public static int i = 0;
        public static float speed = 1;
        public static bool locked;
        public static bool repeat;
        public static void PlayStopTracking()
        {
            Playing = !Playing;
            k = 0;
            i = 0;
        }
        public static void PauseTracking()
        {
            fixedscreen = Playing ? Main.screenPosition
                      : Main.LocalPlayer.position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            Playing = !Playing;
            locked = !locked;
        }
        public override void ModifyScreenPosition()
        {
            if (Playing)
            {
                if (i < PathTrackers.trackers.Count && PathTrackers.trackers.Count != 1)
                {
                    Vector2 start = PathTrackers.trackers[i].Position - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
                    Vector2 end = start;
                    if (i + 1 < PathTrackers.trackers.Count)
                    {
                        end = PathTrackers.trackers[i + 1].Position - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
                    }

                    // Change screen Position
                    switch (CCUI.selectedInterp)
                    {
                        case 0:
                            if (Main.screenPosition != new Vector2(end.X, start.Y))
                            {
                                Main.screenPosition = Vector2.Lerp(start, new Vector2(end.X, start.Y), k * 2);
                                Main.NewText("going sideways");
                            }
                            else
                            {
                                Main.screenPosition = Vector2.Lerp(new Vector2(end.X, start.Y), end, k * 2);
                                Main.NewText("going up");
                            }

                            break;
                        case 1:
                            Main.screenPosition = Vector2.Lerp(start, end, k);
                            break;
                        case 2:
                            for (int j = 0; j + 1 < PathTrackers.trackers[i].Connection.PointList.Count; j++)
                                Main.screenPosition = Vector2.Lerp(PathTrackers.trackers[i].Connection.PointList[j] - new Vector2(Main.screenWidth, Main.screenHeight) / 2,
                                    PathTrackers.trackers[i].Connection.PointList[j + 1] - new Vector2(Main.screenWidth, Main.screenHeight) / 2, k);
                            break;
                    }
                    // Change Lerp amount
                    k += 0.01f / PathTrackers.trackers[i].Connection.Length() * speed;
                    // Next segment
                    if (k > 1f || Main.screenPosition == end)
                    {
                        i++;
                        k = 0;
                    }
                }
                else
                {
                    if (!repeat)
                        Playing = false;
                    i = 0;
                    k = 0;
                }
            }

            if (!Main.gameMenu)
            {
                if (locked)
                {
                    // Lock screen
                    Main.screenPosition = fixedscreen;

                    // Lock screen to Entity
                    if (EntityTracker.tracker != null)
                    {
                        EntityTracker.Position = EntityTracker.TrackedEntity.position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
                        Main.screenPosition = EntityTracker.Position;
                    }
                }
            }
        }
    }
}
