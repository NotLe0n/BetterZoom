using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterZoom.src.UI.UIElements
{
    class Dpad : UIElement
    {
        private enum ArrowDir { UP, DOWN, LEFT, RIGHT }

        private readonly (UIImageButton elm, ArrowDir dir)[] buttons;

        public Dpad(float x, float y)
        {
            Width.Set(240, 0);
            Height.Set(320, 0);

            buttons = new (UIImageButton, ArrowDir)[] {
                (new UIImageButton(ModContent.Request<Texture2D>("BetterZoom/Assets/UpButton", ReLogic.Content.AssetRequestMode.ImmediateLoad))
                {
                    MarginTop = y,
                    MarginLeft = x
                }, ArrowDir.UP),

                (new UIImageButton(ModContent.Request<Texture2D>("BetterZoom/Assets/DownButton", ReLogic.Content.AssetRequestMode.ImmediateLoad))
                {
                    MarginTop = y + 40,
                    MarginLeft = x
                }, ArrowDir.DOWN),

                (new UIImageButton(ModContent.Request<Texture2D>("BetterZoom/Assets/LeftButton", ReLogic.Content.AssetRequestMode.ImmediateLoad))
                {
                    MarginTop = y + 40,
                    MarginLeft = x - 40
                }, ArrowDir.LEFT),

                (new UIImageButton(ModContent.Request<Texture2D>("BetterZoom/Assets/RightButton", ReLogic.Content.AssetRequestMode.ImmediateLoad))
                {
                    MarginTop = y + 40,
                    MarginLeft = x + 40
                }, ArrowDir.RIGHT)
            };

            foreach (var button in buttons)
            {
                Append(button.elm);
            }
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            Camera.fixedscreen = Main.screenPosition;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var button in buttons)
            {
                if (button.elm.IsMouseHovering && Main.mouseLeft)
                {
                    Clicked(button.dir);
                }
            }
        }

        private static void Clicked(ArrowDir dir)
        {
            switch (dir)
            {
                case ArrowDir.UP:
                    Camera.MoveRelativeTo(new Vector2(0, -5f));
                    break;
                case ArrowDir.DOWN:
                    Camera.MoveRelativeTo(new Vector2(0, 5f));
                    break;
                case ArrowDir.LEFT:
                    Camera.MoveRelativeTo(new Vector2(-5f, 0));
                    break;
                case ArrowDir.RIGHT:
                    Camera.MoveRelativeTo(new Vector2(5f, 0));
                    break;
            }
        }
    }
}
