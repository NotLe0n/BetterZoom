using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace BetterZoom.src.UI.UIElements
{
    class NewUITextBox : UITextPanel<string>
    {
        internal bool focused = false;
        private int _cursor;
        private int _frameCount;
        private int _maxLength = 60;
        private string hintText;
        public event Action OnFocus;
        public event Action OnUnfocus;
        public event Action OnTextChanged;
        public event Action OnTabPressed;
        public event Action OnEnterPressed;
        public event Action OnUpPressed;
        internal bool unfocusOnEnter = true;
        internal bool unfocusOnTab = true;


        public NewUITextBox(string text, float textScale = 1, bool large = false) : base("", textScale, large)
        {
            hintText = text;
            SetPadding(4);
        }

        public override void Click(UIMouseEvent evt)
        {
            Focus();
            base.Click(evt);
        }

        public void SetUnfocusKeys(bool unfocusOnEnter, bool unfocusOnTab)
        {
            this.unfocusOnEnter = unfocusOnEnter;
            this.unfocusOnTab = unfocusOnTab;
        }

        public void Unfocus()
        {
            if (focused)
            {
                focused = false;
                Main.blockInput = false;

                OnUnfocus?.Invoke();
            }
        }

        public void Focus()
        {
            if (!focused)
            {
                Main.clrInput();
                focused = true;
                Main.blockInput = true;

                OnFocus?.Invoke();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!ContainsPoint(Main.MouseScreen) && Main.mouseLeft)
            {
                Unfocus();
            }
        }

        public void Write(string text)
        {
            SetText(Text.Insert(_cursor, text));
            _cursor += text.Length;
            _cursor = Math.Min(Text.Length, _cursor);
            Recalculate();

            OnTextChanged?.Invoke();
        }

        public void WriteAll(string text)
        {
            bool changed = text != Text;
            SetText(text);
            _cursor = text.Length;
            Recalculate();

            if (changed)
            {
                OnTextChanged?.Invoke();
            }
        }

        public override void SetText(string text, float textScale, bool large)
        {
            if (text.ToString().Length > _maxLength)
            {
                text = text.ToString().Substring(0, _maxLength);
            }
            base.SetText(text, textScale, large);

            _cursor = Math.Min(Text.Length, _cursor);

            OnTextChanged?.Invoke();
        }

        public void SetTextMaxLength(int maxLength)
        {
            _maxLength = maxLength;
        }

        public void Backspace()
        {
            if (_cursor == 0)
            {
                return;
            }
            SetText(Text.Substring(0, Text.Length - 1));
            Recalculate();
        }

        public void CursorLeft()
        {
            if (_cursor == 0)
            {
                return;
            }
            _cursor--;
        }

        public void CursorRight()
        {
            if (_cursor < Text.Length)
            {
                _cursor++;
            }
        }

        static bool JustPressed(Keys key)
        {
            return Main.inputText.IsKeyDown(key) && !Main.oldInputText.IsKeyDown(key);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle hitbox = GetDimensions().ToRectangle();

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, Color.White);


            if (focused)
            {
                Terraria.GameInput.PlayerInput.WritingText = true;
                Main.instance.HandleIME();

                WriteAll(Main.GetInputText(Text));

                if (JustPressed(Keys.Tab))
                {
                    if (unfocusOnTab) Unfocus();
                    OnTabPressed?.Invoke();
                }

                if (JustPressed(Keys.Enter))
                {
                    if (unfocusOnEnter) Unfocus();
                    OnEnterPressed?.Invoke();
                }
                if (JustPressed(Keys.Up))
                {
                    OnUpPressed?.Invoke();
                }
            }
            CalculatedStyle innerDimensions2 = GetInnerDimensions();
            Vector2 pos2 = innerDimensions2.Position();
            if (IsLarge)
            {
                pos2.Y -= 10f * TextScale * TextScale;
            }
            else
            {
                pos2.Y -= 2f * TextScale;
            }
            if (IsLarge)
            {
                Utils.DrawBorderStringBig(spriteBatch, Text, pos2, TextColor, TextScale, 0f, 0f, -1);
                return;
            }
            Utils.DrawBorderString(spriteBatch, Text, pos2, TextColor, TextScale, 0f, 0f, -1);

            _frameCount++;

            CalculatedStyle innerDimensions = GetInnerDimensions();
            Vector2 pos = innerDimensions.Position();
            DynamicSpriteFont spriteFont = IsLarge ? FontAssets.DeathText.Value : FontAssets.MouseText.Value;
            Vector2 vector = new Vector2(spriteFont.MeasureString(Text.Substring(0, _cursor)).X, IsLarge ? 32f : 16f) * TextScale;
            if (IsLarge)
            {
                pos.Y -= 8f * TextScale;
            }
            else
            {
                pos.Y -= 1f * TextScale;
            }
            if (Text.Length == 0)
            {
                Vector2 hintTextSize = new Vector2(spriteFont.MeasureString(hintText.ToString()).X, IsLarge ? 32f : 16f) * TextScale;
                pos.X += 5;
                if (IsLarge)
                {
                    Utils.DrawBorderStringBig(spriteBatch, hintText, pos, Color.Gray, TextScale, 0f, 0f, -1);
                    return;
                }
                Utils.DrawBorderString(spriteBatch, hintText, pos, Color.Gray, TextScale, 0f, 0f, -1);
                pos.X -= 5;
            }

            if (!focused) return;

            pos.X += vector.X - (IsLarge ? 8f : 4f) * TextScale + 6f;
            if ((_frameCount %= 40) > 20)
            {
                return;
            }
            if (IsLarge)
            {
                Utils.DrawBorderStringBig(spriteBatch, "|", pos, TextColor, TextScale, 0f, 0f, -1);
                return;
            }
            Utils.DrawBorderString(spriteBatch, "|", pos, TextColor, TextScale, 0f, 0f, -1);
        }
    }
}