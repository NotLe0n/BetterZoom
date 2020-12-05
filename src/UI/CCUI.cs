using BetterZoom.src.Trackers;
using BetterZoom.src.UI.UIElements;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterZoom.src.UI
{
    class CCUI : UIState
    {
        UITextPanel<string> playButton;
        UIFloatRangedDataValue speed;
        byte placing;
        byte move = 0;
        bool erasing;
        bool moving;
        public static UIToggleImage lockScreenBtn;
        public static byte selectedInterp = 2;
        public override void OnInitialize()
        {
            Camera.fixedscreen = Main.LocalPlayer.position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

            UIElement Menu = new TabPanel(400, 350,
                new Tab("Better Zoom", new BZUI()),
                new Tab(" Camera Control", this)
                );
            Menu.Left.Set(TabPanel.lastPos.X, 0f);
            Menu.Top.Set(TabPanel.lastPos.Y, 0f);
            Append(Menu);

            speed = new UIFloatRangedDataValue("Tracking Speed", 1, 0.1f, 100);
            var speedSldr = new UIRange<float>(speed);
            speedSldr.Width.Set(0, 1);
            speedSldr.MarginTop = 50;
            speedSldr.MarginLeft = -20;
            Menu.Append(speedSldr);

            Menu.Append(new UIText("Control Trackers: ") { MarginTop = 130, MarginLeft = 210 });

            UIImageButton PathTrackerBtn = new UIImageButton(ModContent.GetTexture("BetterZoom/Assets/PathTrackerButton"));
            PathTrackerBtn.OnClick += (evt, elm) =>
            {
                placing = 1;
            };
            PathTrackerBtn.MarginLeft = 245;
            PathTrackerBtn.MarginTop = 155;
            Menu.Append(PathTrackerBtn);

            UIImageButton EraseTrackerBtn = new UIImageButton(ModContent.GetTexture("BetterZoom/Assets/EraserButton"));
            EraseTrackerBtn.OnClick += (evt, elm) => erasing = !erasing;
            EraseTrackerBtn.MarginLeft = 245;
            EraseTrackerBtn.MarginTop = 190;
            Menu.Append(EraseTrackerBtn);

            UIImageButton EntityBtn = new UIImageButton(ModContent.GetTexture("BetterZoom/Assets/EntityTrackerButton"));
            EntityBtn.OnClick += (evt, elm) =>
            {
                if (EntityTracker.tracker == null) placing = 2;
                else EntityTracker.RemoveTracker();
            };
            EntityBtn.MarginLeft = 285;
            EntityBtn.MarginTop = 155;
            Menu.Append(EntityBtn);

            UIImageButton MoveBtn = new UIImageButton(ModContent.GetTexture("BetterZoom/Assets/MoveButton"));
            MoveBtn.OnClick += (evt, elm) => moving = !moving;
            MoveBtn.MarginLeft = 285;
            MoveBtn.MarginTop = 190;
            Menu.Append(MoveBtn);

            lockScreenBtn = new UIToggleImage(TextureManager.Load("Images/UI/Settings_Toggle"), 13, 13, new Point(17, 1), new Point(1, 1));
            lockScreenBtn.MarginTop = 100;
            lockScreenBtn.MarginLeft = 250;
            lockScreenBtn.OnClick += (evt, elm) =>
            {
                Camera.fixedscreen = Main.screenPosition;
                Camera.locked = !Camera.locked;
            };
            lockScreenBtn.Append(new UIText("Lock Screen", 0.9f) { MarginLeft = -230 });
            Menu.Append(lockScreenBtn);

            Menu.Append(new UIText("Control Screen: ") { MarginTop = 130, MarginLeft = 20 });

            // Dpad
            var Dpad = UIHelper.Dpad(60, 155);
            for (int i = 0; i < Dpad.Length; i++)
                Menu.Append(Dpad[i]);

            Dpad[0].OnMouseDown += (evt, elm) => move = 1;
            Dpad[0].OnMouseUp += (evt, elm) => move = 0;
            Dpad[0].OnClick += (evt, elm) => Camera.locked = true;

            Dpad[1].OnMouseDown += (evt, elm) => move = 2;
            Dpad[1].OnMouseUp += (evt, elm) => move = 0;
            Dpad[1].OnClick += (evt, elm) => Camera.locked = true;

            Dpad[2].OnMouseDown += (evt, elm) => move = 3;
            Dpad[2].OnMouseUp += (evt, elm) => move = 0;
            Dpad[2].OnClick += (evt, elm) => Camera.locked = true;

            Dpad[3].OnMouseDown += (evt, elm) => move = 4;
            Dpad[3].OnMouseUp += (evt, elm) => move = 0;
            Dpad[3].OnClick += (evt, elm) => Camera.locked = true;

            var hideTrackersBtn = new UIToggleImage(TextureManager.Load("Images/UI/Settings_Toggle"), 13, 13, new Point(17, 1), new Point(1, 1));
            hideTrackersBtn.MarginTop = 250;
            hideTrackersBtn.MarginLeft = 250;
            hideTrackersBtn.OnClick += (evt, elm) => TrackerUI.hide = !TrackerUI.hide;
            hideTrackersBtn.Append(new UIText("Hide Trackers", 0.9f) { MarginLeft = -230 });
            Menu.Append(hideTrackersBtn);

            // Control Buttons
            playButton = new UITextPanel<string>("Play");
            playButton.VAlign = 0.9f;
            playButton.HAlign = 0.1f;
            playButton.OnClick += (evt, elm) => Camera.PlayStopTracking();
            Menu.Append(playButton);

            UITextPanel<string> pauseButton = new UITextPanel<string>("Pause");
            pauseButton.VAlign = 0.9f;
            pauseButton.HAlign = 0.5f;
            pauseButton.OnClick += (evt, elm) =>
            {
                Camera.PauseTracking();
                pauseButton.SetText(text: Camera.Playing ? "Pause" : "Resume");
            };
            Menu.Append(pauseButton);

            var repeatBtn = new UITextPanel<string>("Repeat");
            repeatBtn.VAlign = 0.9f;
            repeatBtn.HAlign = 0.9f;
            repeatBtn.OnClick += (evt, elm) =>
            {
                Camera.repeat = !Camera.repeat;
                repeatBtn.SetText(text: Camera.repeat ? "End" : "Repeat");
            };
            Menu.Append(repeatBtn);
        }
        UIImage placeTracker = new UIImage(ModContent.GetTexture("BetterZoom/Assets/PathTracker"));
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            lockScreenBtn.SetState(Camera.locked);
            Camera.speed = speed.Data * 100;

            playButton.SetText(text: Camera.Playing ? "Stop" : "Play");

            // Placer
            if (placing != 0)
            {
                Main.cursorOverride = 16;
                Main.LocalPlayer.mouseInterface = true;
                placeTracker.ImageScale = 0.5f;
                placeTracker.MarginLeft = Main.MouseScreen.X - placeTracker.Width.Pixels / 2;
                placeTracker.MarginTop = Main.MouseScreen.Y - placeTracker.Height.Pixels / 2;
                erasing = moving = false;

                if (placing == 1)
                {
                    placeTracker.SetImage(ModContent.GetTexture("BetterZoom/Assets/PathTracker"));
                    Append(placeTracker);

                    if (Main.mouseLeft)
                    {
                        new PathTrackers(Main.MouseWorld);
                        placeTracker.Remove();
                        placing = 0;
                    }
                }
                // Entity Tracker
                else if (placing == 2)
                {
                    placeTracker.SetImage(ModContent.GetTexture("BetterZoom/Assets/EntityTracker"));
                    Append(placeTracker);

                    if (Main.mouseLeft)
                    {
                        Camera.locked = true;
                        new EntityTracker(Main.MouseWorld);
                        placeTracker.Remove();
                        placing = 0;
                    }
                }
            }
            // Eraser
            if (erasing)
            {
                Main.cursorOverride = 6;
                Main.LocalPlayer.mouseInterface = true;
                moving = false;
                placing = 0;

                if (Main.mouseLeft)
                {
                    PathTrackers.Remove();
                }
            }

            // Tracker Mover
            if (moving)
            {
                placing = 0;
                erasing = false;
                Main.cursorOverride = 16;
                Main.LocalPlayer.mouseInterface = true;

                if (Main.mouseLeft)
                {
                    for (int i = 0; i < PathTrackers.trackers.Count; i++)
                    {
                        if (PathTrackers.trackers[i].PTrackerImg.IsMouseHovering)
                        {
                            PathTrackers.trackers[i].Position = Main.MouseWorld;
                        }
                    }
                }
            }

            // Camera Mover
            if (move != 0)
            {
                switch (move)
                {
                    case 1:
                        Camera.fixedscreen += new Vector2(0, -.1f);
                        break;
                    case 2:
                        Camera.fixedscreen += new Vector2(0, .1f);
                        break;
                    case 3:
                        Camera.fixedscreen += new Vector2(-0.1f, 0);
                        break;
                    case 4:
                        Camera.fixedscreen += new Vector2(0.1f, 0);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
