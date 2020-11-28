using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace BetterZoom.src.UI.UIElements
{
    class Tab : UITextPanel<string>
    {
        public UIState _changeStateTo;
        public static UIState lastTab = new BZUI();
        /// <summary>
        /// List of all Tabs
        /// </summary>
        public static Tab[] tabs = { };

        /// <summary>
        /// Creates a new Tab
        /// </summary>
        /// <param name="text">The text on the Tab</param>
        /// <param name="changeStateTo">What UIState should be switched to when clicking the Tab</param>
        public Tab(string text, UIState changeStateTo) : base(text)
        {
            _changeStateTo = changeStateTo;
            tabs.ToList().Add(this);
        }
        public override void OnInitialize()
        {
            SetPadding(7);
            BackgroundColor.A = 255;

            for (int i = 0; i < tabs.Length; i++)
            {
                if (i > 0 && tabs[i - 1] != null)
                {
                    tabs[i].Left.Set(tabs[i - 1].GetDimensions().Width - 16f, 0f);
                }
                Recalculate();
            }
        }
        public override void Click(UIMouseEvent evt)
        {
            lastTab = _changeStateTo;
            GetInstance<BetterZoom>().UserInterface.SetState(_changeStateTo);
            Main.PlaySound(SoundID.MenuTick);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (GetInstance<BetterZoom>().UserInterface.CurrentState == _changeStateTo)
            {
                BackgroundColor = new Color(73, 94, 171);
            }
        }
    }

    class TabPanel : UIPanel
    {
        /// <summary>
        /// Creates a new Tab Panel
        /// </summary>
        /// <param name="width">The width of the panel</param>
        /// <param name="height">The height of the panel</param>
        /// <param name="tabs">All tabs that the panel should hold</param>
        public TabPanel(float width, float height, params Tab[] tabs)
        {
            Width.Pixels = width;
            Height.Pixels = height;
            Tab.tabs = tabs;
        }

        public override void OnInitialize()
        {
            SetPadding(0);

            var header = new UIPanel();
            header.SetPadding(0);
            header.Width = Width;
            header.Height.Set(30, 0f);
            header.BackgroundColor.A = 255;
            header.OnMouseDown += Header_OnMouseDown;
            header.OnMouseUp += Header_OnMouseUp;
            Append(header);

            var closeBtn = new UITextPanel<char>('X');
            closeBtn.SetPadding(7);
            closeBtn.Width.Set(40, 0);
            closeBtn.Left.Set(0, 0.9f);
            closeBtn.BackgroundColor.A = 255;
            closeBtn.OnClick += (evt, elm) => GetInstance<BetterZoom>().UserInterface.SetState(null);
            header.Append(closeBtn);

            for (int i = Tab.tabs.Length - 1; i >= 0; i--)
                header.Append(Tab.tabs[i]);
        }
        #region Drag code yoiked from ExampleMod 

        private Vector2 offset;
        public bool dragging;
        public static Vector2 lastPos = new Vector2(600, 200);
        public void Header_OnMouseDown(UIMouseEvent evt, UIElement elm)
        {
            base.MouseDown(evt);
            offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
            dragging = true;
        }

        public void Header_OnMouseUp(UIMouseEvent evt, UIElement elm)
        {
            base.MouseUp(evt);
            dragging = false;

            Left.Set(evt.MousePosition.X - offset.X, 0f);
            Top.Set(evt.MousePosition.Y - offset.Y, 0f);
            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // don't remove.

            // Checking ContainsPoint and then setting mouseInterface to true is very common. This causes clicks on this UIElement to not cause the player to use current items. 
            if (ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;


            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0f);
                Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();

                lastPos = new Vector2(Left.Pixels, Top.Pixels);
            }

            // Here we check if the DragableUIPanel is outside the Parent UIElement rectangle. 
            // (In our example, the parent would be ExampleUI, a UIState. This means that we are checking that the DragableUIPanel is outside the whole screen)
            // By doing this and some simple math, we can snap the panel back on screen if the user resizes his window or otherwise changes resolution.
            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                // Recalculate forces the UI system to do the positioning math again.
                Recalculate();
            }
        }
        #endregion
    }
}
