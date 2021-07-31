using BetterZoom.src.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterZoom.src
{
    public class UISystem : ModSystem
    {
        // UI
        internal UserInterface userInterface;
        internal UserInterface trackerUserInterface;
        public UIState trackerUI;

        public override void Load()
        {
            // UI
            if (!Main.dedServ)
            {
                userInterface = new UserInterface();
                userInterface.SetState(null);

                trackerUI = new TrackerUI();
                trackerUI.Activate();

                trackerUserInterface = new UserInterface();
                trackerUserInterface.SetState(trackerUI);

                UI.UIElements.TabPanel.lastTab = new BZUI();
            }
        }

        public override void Unload()
        {
            // UI
            userInterface = null;
            trackerUserInterface = null;
            trackerUI = null;

            UI.UIElements.TabPanel.lastTab = null;
        }

        private GameTime _lastUpdateUiGameTime;
        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;

            userInterface?.Update(gameTime);

            if (!TrackerUI.hide)
                trackerUserInterface.Update(gameTime);
        }

        /// <summary>
        /// Add UI
        /// </summary>
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Better Zoom: UI",
                    delegate
                    {
                        userInterface?.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        return true;
                    }, InterfaceScaleType.UI));
            }

            int RulerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Ruler"));
            if (RulerIndex != -1)
            {
                layers.Insert(RulerIndex, new LegacyGameInterfaceLayer(
                    "Better Zoom: TrackerUI",
                    delegate
                    {
                        if (!TrackerUI.hide)
                            trackerUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        return true;
                    }, InterfaceScaleType.Game));
            }
        }
    }
}
