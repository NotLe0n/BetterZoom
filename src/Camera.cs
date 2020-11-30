using BetterZoom.src.Trackers;
using BetterZoom.src.UI;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace BetterZoom.src
{
    class Camera : ModPlayer
    {
        public static Vector2 fixedscreen = Main.LocalPlayer.position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
        public static bool Playing, locked, repeat;
        public static float k;
        public static int segment = 0;
        public static float speed = 1;
        public static void PlayStopTracking()
        {
            Playing = !Playing;
            k = 0;
            segment = 0;
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
                if (segment < PathTrackers.trackers.Count && PathTrackers.trackers.Count != 1)
                {
                    var start = PathTrackers.trackers[segment].Position - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
                    var end = start;
                    if (segment + 1 < PathTrackers.trackers.Count)
                        end = PathTrackers.trackers[segment + 1].Position - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
                    var control = PathTrackers.trackers[segment].Connection.ControlPoint.Position - new Vector2(Main.screenWidth, Main.screenHeight) / 2;

                    // Change screen Position
                    switch (CCUI.selectedInterp)
                    {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            Main.screenPosition = (start - 2 * control + end) * (float)Math.Pow(k, 2) + (-2 * start + 2 * control) * k + start;
                            break;
                    }
                    // Change Lerp amount
                    k += 0.01f / PathTrackers.trackers[segment].Connection.Length() * speed;
                    // Next segment
                    if (k > 1f || Main.screenPosition == end)
                    {
                        segment++;
                        k = 0;
                    }
                }
                else
                {
                    if (!repeat)
                        Playing = false;
                    segment = 0;
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
