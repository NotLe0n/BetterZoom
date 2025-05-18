using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace BetterZoom;

public class FloatRange
{
	public float min;
	public float max;

	public override string ToString() => $"[{min} - {max}]";
}

public class DualRangeElement : ConfigElement<FloatRange>
{
	private FloatRange range;
	private float MaxLimit { get; set; }
	private float MinLimit { get; set; }
	
	public override void OnBind()
	{
		base.OnBind();
		range = (FloatRange)MemberInfo.GetValue(Item);
		
		if (RangeAttribute is { Min: float, Max: float }) {
			MinLimit = (float)RangeAttribute.Min;
			MaxLimit = (float)RangeAttribute.Max;
		}
		
		// Initialize proportions
		float clampedMin = MathHelper.Clamp(range.min, MinLimit, MaxLimit);
		float clampedMax = MathHelper.Clamp(range.max, MinLimit, MaxLimit);

		minProportion = (clampedMin - MinLimit) / (MaxLimit - MinLimit);
		maxProportion = (clampedMax - MinLimit) / (MaxLimit - MinLimit);
		
		TextDisplayFunction = () => $"{Label}: {range.min:P2} - {range.max:P2}";
	}

	public void DrawValueBar(SpriteBatch sb, float scale)
	{
		Texture2D colorBarTexture = TextureAssets.ColorBar.Value;
		Vector2 scaledSize = new Vector2(colorBarTexture.Width, colorBarTexture.Height) * scale;
		IngameOptions.valuePosition.X -= (int)scaledSize.X;
		
		Rectangle rectangle = new Rectangle((int)IngameOptions.valuePosition.X, (int)IngameOptions.valuePosition.Y - (int)scaledSize.Y / 2, (int)scaledSize.X, (int)scaledSize.Y);
		Rectangle destinationRectangle = rectangle;
		
		int width = 167;
		float num2 = rectangle.X + 5f * scale;
		float num3 = rectangle.Y + 4f * scale;

		sb.Draw(colorBarTexture, rectangle, Color.White);

		for (float i = 0f; i < width; i += 1f) {
			float percent = i / width;
			sb.Draw(TextureAssets.ColorBlip.Value, new Vector2(num2 + percent * width * scale, num3), null, Utils.ColorLerp_BlackToWhite(percent), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		}

		rectangle.Inflate((int)(-5f * scale), 2);

		bool hovering = rectangle.Contains(new Point(Main.mouseX, Main.mouseY));

		if (hovering) {
			sb.Draw(TextureAssets.ColorHighlight.Value, destinationRectangle, Main.OurFavoriteColor);
		}
		
		IngameOptions.inBar = hovering;
	}
	
	private bool draggingMin;
	private bool draggingMax;
	
	protected float minProportion;
	protected float maxProportion;
	
	protected override void DrawSelf(SpriteBatch spriteBatch) {
		base.DrawSelf(spriteBatch);

		CalculatedStyle dimensions = GetDimensions();
		Vector2 barPosition = new Vector2(dimensions.X + dimensions.Width - 10, dimensions.Y + 16f);

		Rectangle barRect = new Rectangle((int)barPosition.X - 167, (int)barPosition.Y - 4, 167, 8);
		IngameOptions.valuePosition = barPosition;
		float scale = 1f;

		// Draw base bar
		int num = 6;
		Vector2 vector2 = dimensions.Position();
		vector2.X += 8f;
		vector2.Y += 2f + num;
		vector2.X -= 17f;
		//TextureAssets.ColorBar.Value.Frame(1, 1, 0, 0);
		vector2 = new Vector2(dimensions.X + dimensions.Width - 10f, dimensions.Y + 10f + num);
		IngameOptions.valuePosition = vector2;
		DrawValueBar(spriteBatch, scale);

		// Draw handle sliders
		Texture2D sliderTex = TextureAssets.ColorSlider.Value;
		Vector2 sliderSize = sliderTex.Size() * 0.5f;

		Vector2 minPos = new Vector2(barRect.X + barRect.Width * minProportion, barRect.Y + barRect.Height / 2);
		Vector2 maxPos = new Vector2(barRect.X + barRect.Width * maxProportion, barRect.Y + barRect.Height / 2);

		spriteBatch.Draw(sliderTex, minPos, null, Color.White, 0f, sliderSize, scale, SpriteEffects.None, 0f);
		spriteBatch.Draw(sliderTex, maxPos, null, Color.White, 0f, sliderSize, scale, SpriteEffects.None, 0f);

		// Handle dragging logic
		Vector2 mouse = new Vector2(Main.mouseX, Main.mouseY);
		float minDistance = minPos.Distance(mouse);
		float maxDistance = maxPos.Distance(mouse);
		bool mouseDown = Main.mouseLeft;

		if (mouseDown && barRect.Contains(mouse.ToPoint())) {
			if (!draggingMin && !draggingMax) {
				if (minDistance < maxDistance) {
					draggingMin = true;
				}
				else {
					draggingMax = true;
				}
			}

			// calculate mouse position in reference to bar
			float percent = Utils.Clamp((mouse.X - barRect.X) / barRect.Width, 0, 1);

			if (draggingMin) {
				minProportion = Math.Min(percent, maxProportion); // prevent min from going over max
				range.min = LerpValue(minProportion);
				Console.WriteLine($"Setting min: {range.min}");
				SetObject(range); // update value
			} else if (draggingMax) {
				maxProportion = Math.Max(percent, minProportion); // prevent max from going over min
				range.max = LerpValue(maxProportion);
				SetObject(range); // update value
			}
		} else {
			draggingMin = false;
			draggingMax = false;
		}
	}
	
	private float LerpValue(float percent) {
		float min = MinLimit;
		float max = MaxLimit;
		float val = min + (max - min) * percent;
		return val;
	}
}