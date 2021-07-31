using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterZoom.src.UI.UIElements
{
    class Tab : UITextPanel<string>
    {
        public UIState _changeStateTo;

        /// <summary>
        /// Creates a new Tab
        /// </summary>
        /// <param name="text">The text on the Tab</param>
        /// <param name="changeStateTo">What UIState should be switched to when clicking the Tab</param>
        public Tab(string text, UIState changeStateTo) : base(text)
        {
            _changeStateTo = changeStateTo;
        }

        public override void OnInitialize()
        {
            SetPadding(7);
            BackgroundColor.A = 255; // solid color
        }

        public override void Click(UIMouseEvent evt)
        {
            // update Last tab
            TabPanel.lastTab = _changeStateTo;

            // change UIState and play click sound
            ModContent.GetInstance<UISystem>().userInterface.SetState(_changeStateTo);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void Update(GameTime gameTime)
        {
            // Highlight
            if (ModContent.GetInstance<UISystem>().userInterface.CurrentState == _changeStateTo)
            {
                BackgroundColor = new Color(73, 94, 171);
            }
        }
    }

    class TabPanel : DragableUIPanel
    {
        /// <summary>
        /// List of all Tabs
        /// </summary>
        public Tab[] Tabs;

        public static UIState lastTab = new BZUI();

        /// <summary>
        /// Creates a new Tab Panel
        /// </summary>
        /// <param name="width">The width of the panel</param>
        /// <param name="height">The height of the panel</param>
        /// <param name="tabs">All tabs that the panel should hold</param>
        public TabPanel(float width, float height, params Tab[] tabs) : base("", width, height)
        {
            Width.Pixels = width;
            Height.Pixels = height;
            Tabs = tabs;
        }

        public override void OnInitialize()
        {
            // set correct position for all tabs
            for (int i = 0; i < Tabs.Length; i++)
            {
                if (i > 0 && Tabs[i - 1] != null)
                {
                    Tabs[i].Left.Set(Tabs[i - 1].MinWidth.Pixels - 24, 0f);
                }
            }

            // append all tabs
            for (int i = Tabs.Length - 1; i >= 0; i--)
                header.Append(Tabs[i]);
        }
    }
}
