using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace BetterZoom.Edits;

internal sealed class ZoomInputBox
{
	private int textBoxCounter;
	private string zoomString;
	public float ZoomValue { get; set; }
	public bool Focused { get; private set; }
	public event Action<float> OnChange;

	public ZoomInputBox(float initialValue)
	{
		ZoomValue = initialValue;
		zoomString = $"{initialValue:p2}";
	}
	
	public void Draw(SpriteBatch sb, Vector2 pos, int width, int height, float scale1)
	{
		const int edgeWidth = 5;

		Utils.DrawSplicedPanel(sb, TextureAssets.TextBack.Value, (int)pos.X, (int)pos.Y, width, height, edgeWidth, edgeWidth, edgeWidth, edgeWidth, Color.White);
		
		if (Main.mouseLeft) {
			if (new Rectangle((int)pos.X, (int)pos.Y, width, height).Contains(Main.MouseScreen.ToPoint())) {
				if (!Focused) {
					Focused = true;
					zoomString = zoomString[..^1];
					SoundEngine.PlaySound(SoundID.MenuTick);
				}
			}
			else {
				Unfocus();
			}
		}

		if (Focused) {
			// Handle input
			Terraria.GameInput.PlayerInput.WritingText = true;
			Main.instance.HandleIME();
			
			string input = Main.GetInputText(zoomString);
			
			// only allow floats beween 0 and 1000
			if ((float.TryParse(input, out float num) || input == "") && num / 1000f is <= 1 and >= 0) {
				zoomString = input;
			}

			if (Main.inputText.IsKeyDown(Keys.Enter) && !Main.oldInputText.IsKeyDown(Keys.Enter)) {
				Unfocus();
			}

			// Text box caret
			textBoxCounter = ++textBoxCounter % 61;
			if (textBoxCounter < 30) {
				var textSize = FontAssets.MouseText.Value.MeasureString(zoomString) * scale1;
				Utils.DrawBorderString(sb, "I", pos + new Vector2(textSize.X + 5, 2), Color.White, scale1);
			}
		}
		else {
			zoomString = $"{ZoomValue:p2}";
		}

		// Draw string
		Utils.DrawBorderString(sb, zoomString, pos + new Vector2(5, 2), Color.White, scale1);
	}

	private void Unfocus()
	{
		Focused = false;
		textBoxCounter = 0;
		
		if (float.TryParse(zoomString, out float newZoom)) {
			ZoomValue = newZoom / 100f;
			OnChange?.Invoke(ZoomValue);

			SoundEngine.PlaySound(SoundID.MenuTick);
		}
	}
}
