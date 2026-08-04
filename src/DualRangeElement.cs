using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
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
	// Mirrors RangeElement.rightLock: whichever element grabbed the mouse keeps it until the button is released, so only one slider reacts at a time.
	private static DualRangeElement dragLock;

	private FloatRange range;
	private float MaxLimit { get; set; }
	private float MinLimit { get; set; }

	private Rectangle sliderRect;

	private bool draggingMin;
	private bool draggingMax;

	protected float minProportion;
	protected float maxProportion;

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

	/// <param name="lockState">0 = free, 1 = this element owns the drag, 2 = another element owns it.</param>
	private void DrawValueBar(SpriteBatch sb, float scale, int lockState = 0)
	{
		Texture2D colorBarTexture = TextureAssets.ColorBar.Value;
		Vector2 scaledSize = new Vector2(colorBarTexture.Width, colorBarTexture.Height) * scale;
		IngameOptions.valuePosition.X -= (int)scaledSize.X;

		Rectangle rectangle = new Rectangle((int)IngameOptions.valuePosition.X, (int)IngameOptions.valuePosition.Y - (int)scaledSize.Y / 2, (int)scaledSize.X, (int)scaledSize.Y);
		Rectangle destinationRectangle = rectangle;

		const int SliderWidth = 167;
		float num2 = rectangle.X + 5f * scale;
		float num3 = rectangle.Y + 4f * scale;

		sb.Draw(colorBarTexture, rectangle, Color.White);

		for (float i = 0f; i < SliderWidth; i += 1f) {
			float percent = i / SliderWidth;
			sb.Draw(TextureAssets.ColorBlip.Value,
				new Vector2(num2 + percent * SliderWidth * scale, num3), null, 
				Utils.ColorLerp_BlackToWhite(percent), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f
			);
		}

		rectangle.Inflate((int)(-5f * scale), 2);
		sliderRect = new Rectangle((int)num2, rectangle.Y, (int)(SliderWidth * scale), rectangle.Height);

		bool hovering = IsMouseHovering && rectangle.Contains(new Point(Main.mouseX, Main.mouseY));
		if (lockState == 2)
			hovering = false;

		// Keep the highlight lit while we own the drag, even with the cursor off the bar.
		if (hovering || lockState == 1)
			sb.Draw(TextureAssets.ColorHighlight.Value, destinationRectangle, Main.OurFavoriteColor);

		IngameOptions.inBar = hovering;
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		base.DrawSelf(spriteBatch);

		// Release as soon as the button comes up, wherever the cursor happens to be.
		if (!Main.mouseLeft) {
			if (dragLock == this)
				dragLock = null;

			draggingMin = false;
			draggingMax = false;
		}

		int lockState = 0;

		if (dragLock == this)
			lockState = 1;
		else if (dragLock != null)
			lockState = 2;

		CalculatedStyle dimensions = GetDimensions();
		const float scale = 1f;
		const float num = 6f;

		IngameOptions.valuePosition = new Vector2(dimensions.X + dimensions.Width - 10f, dimensions.Y + 10f + num);
		DrawValueBar(spriteBatch, scale, lockState);

		Vector2 mouse = new Vector2(Main.mouseX, Main.mouseY);

		if (IngameOptions.inBar && dragLock == null && PlayerInput.Triggers.JustPressed.MouseLeft) {
			dragLock = this;

			float minDistance = GetHandlePosition(minProportion).Distance(mouse);
			float maxDistance = GetHandlePosition(maxProportion).Distance(mouse);

			if (minDistance < maxDistance)
				draggingMin = true;
			else
				draggingMax = true;
		}

		// Once we hold the lock, keep applying the value no matter where the cursor goes.
		if (dragLock == this && PlayerInput.Triggers.Current.MouseLeft) {
			float percent = Utils.Clamp((mouse.X - sliderRect.X) / sliderRect.Width, 0f, 1f);

			if (draggingMin) {
				minProportion = Math.Min(percent, maxProportion); // prevent min from going over max
				range.min = LerpValue(minProportion);
				SetObject(range); // update value
			}
			else if (draggingMax) {
				maxProportion = Math.Max(percent, minProportion); // prevent max from going under min
				range.max = LerpValue(maxProportion);
				SetObject(range); // update value
			}
		}

		Texture2D sliderTex = TextureAssets.ColorSlider.Value;
		Vector2 sliderOrigin = sliderTex.Size() * 0.5f;

		spriteBatch.Draw(sliderTex, GetHandlePosition(minProportion), null, Color.White, 0f, sliderOrigin, scale, SpriteEffects.None, 0f);
		spriteBatch.Draw(sliderTex, GetHandlePosition(maxProportion), null, Color.White, 0f, sliderOrigin, scale, SpriteEffects.None, 0f);
	}

	private Vector2 GetHandlePosition(float proportion)
	{
		return new Vector2(sliderRect.X + sliderRect.Width * proportion, sliderRect.Y + sliderRect.Height / 2f);
	}

	private float LerpValue(float percent)
	{
		float min = MinLimit;
		float max = MaxLimit;
		float val = min + (max - min) * percent;
		return val;
	}
}