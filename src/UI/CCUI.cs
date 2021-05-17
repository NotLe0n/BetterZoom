using BetterZoom.src.Trackers;
using BetterZoom.src.UI.UIElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterZoom.src.UI
{
    internal class CCUI : UIState
    {
        public static byte selectedInterp = 2;

        private UITextPanel<string> playButton;
        private UIFloatRangedDataValue speed;
        private bool erasing;
        private bool moving;
        private UIToggleImage lockScreenBtn;
        private TrackerID? placing = null;
        private DragableUIPanel ConfirmPanel = new DragableUIPanel("Are you sure you want to remove all trackers?", 700, 120);

        private enum TrackerID
        {
            PathTracker,
            EntityTracker
        }

        public override void OnInitialize()
        {
            Camera.fixedscreen = Main.LocalPlayer.position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

            TabPanel Menu = new TabPanel(400, 350,
                new Tab("Better Zoom", new BZUI()),
                new Tab(" Camera Control", this)
                );
            Menu.Left.Set(DragableUIPanel.lastPos.X, 0f);
            Menu.Top.Set(DragableUIPanel.lastPos.Y, 0f);
            Menu.OnCloseBtnClicked += () => ModContent.GetInstance<BetterZoom>().userInterface.SetState(null);
            Append(Menu);

            speed = new UIFloatRangedDataValue("Tracking Speed", 1, 0.1f, 100);
            var speedSldr = new UIRange<float>(speed);
            speedSldr.Width.Set(0, 1);
            speedSldr.MarginTop = 50;
            speedSldr.MarginLeft = -20;
            Menu.Append(speedSldr);

            Menu.Append(new UIText("Control Trackers: ") { MarginTop = 130, MarginLeft = 210 });

            UIHoverImageButton PathTrackerBtn = new UIHoverImageButton("BetterZoom/Assets/PathTrackerButton", "Place Path tracker");
            PathTrackerBtn.OnClick += (evt, elm) => placing = TrackerID.PathTracker;
            PathTrackerBtn.MarginLeft = 245;
            PathTrackerBtn.MarginTop = 155;
            Menu.Append(PathTrackerBtn);

            UIHoverImageButton EraseTrackerBtn = new UIHoverImageButton("BetterZoom/Assets/EraserButton", "Erase Trackers");
            EraseTrackerBtn.OnClick += (evt, elm) => erasing = !erasing;
            EraseTrackerBtn.MarginLeft = 245;
            EraseTrackerBtn.MarginTop = 190;
            Menu.Append(EraseTrackerBtn);

            UIHoverImageButton DelBtn = new UIHoverImageButton("BetterZoom/Assets/DelButton", "Delete all Trackers");
            DelBtn.OnClick += DeleteAll;
            DelBtn.MarginLeft = 285;
            DelBtn.MarginTop = 190;
            Menu.Append(DelBtn);

            UIHoverImageButton EntityBtn = new UIHoverImageButton("BetterZoom/Assets/EntityTrackerButton", "Place Entity Tracker");
            EntityBtn.OnClick += ToggleEnityTracker;
            EntityBtn.MarginLeft = 285;
            EntityBtn.MarginTop = 155;
            Menu.Append(EntityBtn);

            UIHoverImageButton MoveBtn = new UIHoverImageButton("BetterZoom/Assets/MoveButton", "Move Path Tracker");
            MoveBtn.OnClick += (evt, elm) => moving = !moving;
            MoveBtn.MarginLeft = 325;
            MoveBtn.MarginTop = 190;
            Menu.Append(MoveBtn);

            lockScreenBtn = new UIToggleImage(TextureManager.Load("Images/UI/Settings_Toggle"), 13, 13, new Point(17, 1), new Point(1, 1));
            lockScreenBtn.MarginTop = 100;
            lockScreenBtn.MarginLeft = 250;
            lockScreenBtn.OnClick += (evt, elm) => Camera.ToggleLock();
            lockScreenBtn.Append(new UIText("Lock Screen", 0.9f) { MarginLeft = -230 });
            Menu.Append(lockScreenBtn);

            Menu.Append(new UIText("Control Screen: ") { MarginTop = 130, MarginLeft = 20 });

            // Dpad
            var Dpad = UIHelper.Dpad(60, 155);
            for (int i = 0; i < Dpad.Length; i++)
                Menu.Append(Dpad[i]);

            Dpad[0].OnMouseDown += (evt, elm) => Camera.MoveRelativeTo(new Vector2(0, -5f));
            Dpad[1].OnMouseDown += (evt, elm) => Camera.MoveRelativeTo(new Vector2(0, 5f));
            Dpad[2].OnMouseDown += (evt, elm) => Camera.MoveRelativeTo(new Vector2(-5f, 0));
            Dpad[3].OnMouseDown += (evt, elm) => Camera.MoveRelativeTo(new Vector2(5f, 0));

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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (lockScreenBtn != null)
            {
                lockScreenBtn.SetState(Camera.Locked);
            }

            Camera.speed = speed.Data * 100;

            playButton.SetText(text: Camera.Playing ? "Stop" : "Play");

            if (placing != null)
            {
                PlaceTracker((TrackerID)placing);
            }

            // Eraser
            if (erasing)
            {
                EraseTracker();
            }

            // Tracker Mover
            if (moving)
            {
                MoveTracker();
            }
        }

        private void PlaceTracker(TrackerID id)
        {
            Main.cursorOverride = 16;
            Main.LocalPlayer.mouseInterface = true;
            erasing = moving = false;

            if (Main.mouseLeft)
            {
                if (id == TrackerID.PathTracker)
                {
                    var tracker = new PathTrackers(Main.GameViewMatrix.Translation + Main.screenPosition + (Main.MouseScreen / BetterZoom.zoom));
                    ModContent.GetInstance<BetterZoom>().trackerUI.Append(tracker);
                    placing = null;
                }
                // Entity Tracker
                else if (id == TrackerID.EntityTracker)
                {
                    TrackerUI.entityTracker = new EntityTracker(Main.MouseWorld);
                    ModContent.GetInstance<BetterZoom>().trackerUI.Append(TrackerUI.entityTracker);
                    Camera.ToggleLock(TrackerUI.entityTracker.TrackedEntity.position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2));
                    placing = null;
                }
            }
        }

        private void ToggleEnityTracker(UIMouseEvent evt, UIElement listeningElement)
        {
            if (TrackerUI.entityTracker == null)
            {
                placing = TrackerID.EntityTracker;
            }
            else
            {
                TrackerUI.entityTracker.RemoveTracker();
            }
        }

        private void MoveTracker()
        {
            Main.cursorOverride = 16;
            Main.LocalPlayer.mouseInterface = true;
            placing = null;
            erasing = false;

            if (Main.mouseLeft)
            {
                for (int i = 0; i < TrackerUI.trackers.Count; i++)
                {
                    if (TrackerUI.trackers[i].IsMouseHovering)
                    {
                        TrackerUI.trackers[i].Position = Main.MouseWorld;
                    }
                }
            }
        }

        private void EraseTracker()
        {
            Main.cursorOverride = 6;
            Main.LocalPlayer.mouseInterface = true;
            moving = false;
            placing = null;

            if (Main.mouseLeft)
            {
                int ID = 0;
                for (int i = 0; i < TrackerUI.trackers.Count; i++)
                {
                    if (TrackerUI.trackers[i].IsMouseHovering)
                    {
                        if (TrackerUI.trackers[i] != null && (ID == 0 || Vector2.Distance(TrackerUI.trackers[i].Position, Main.MouseWorld) < Vector2.Distance(TrackerUI.trackers[ID].Position, Main.MouseWorld)))
                        {
                            ID = i;
                            TrackerUI.trackers[i].RemoveTracker();
                        }
                    }
                }
            }
        }

        private void DeleteAll(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!ConfirmPanel.active)
            {
                ConfirmPanel.Left.Set(1000, 0f);
                ConfirmPanel.Top.Set(500, 0f);
                ConfirmPanel.Width.Set(400, 0f);
                ConfirmPanel.Height.Set(120, 0f);
                ConfirmPanel.OnCloseBtnClicked += () => ConfirmPanel.Remove();
                Append(ConfirmPanel);

                UITextPanel<string> yep = new UITextPanel<string>("Yes");
                yep.HAlign = 0.2f;
                yep.VAlign = 0.7f;
                yep.Width.Set(100, 0f);
                yep.OnClick += (evt1, elm1) =>
                {
                    for (int i = 0; i < TrackerUI.trackers.Count; i++)
                    {
                        TrackerUI.trackers[i].RemoveTracker();
                    }
                    TrackerUI.trackers.Clear();
                    ModContent.GetInstance<BetterZoom>().trackerUI.RemoveAllChildren();
                    ConfirmPanel.Remove();
                };
                ConfirmPanel.Append(yep);

                UITextPanel<string> nop = new UITextPanel<string>("No");
                nop.HAlign = 0.8f;
                nop.VAlign = 0.7f;
                nop.Width.Set(100, 0f);
                nop.OnClick += (evt1, elm1) => ConfirmPanel.Remove();
                ConfirmPanel.Append(nop);
            }
        }

        // Drawing the cursors
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (placing != null)
            {
                Rectangle mouseRect = new Rectangle((int)(Main.MouseScreen.X - 16), (int)(Main.MouseScreen.Y - 16), 32, 32);
                if (placing == TrackerID.PathTracker)
                {
                    spriteBatch.Draw(ModContent.GetTexture("BetterZoom/Assets/PathTracker"), mouseRect, Color.White);
                }
                else if (placing == TrackerID.EntityTracker)
                {
                    spriteBatch.Draw(ModContent.GetTexture("BetterZoom/Assets/EntityTracker"), mouseRect, Color.White);
                }
            }
        }
    }
}
