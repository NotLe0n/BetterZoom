using BetterZoom.src.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
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
            
            layers.Add(new LegacyGameInterfaceLayer(
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
