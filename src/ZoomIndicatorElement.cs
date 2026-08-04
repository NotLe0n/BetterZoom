using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.Config.UI;

namespace BetterZoom;

public class ZoomIndicatorElement : FloatElement
{
	private static readonly Asset<Texture2D> Background = ModContent.Request<Texture2D>("BetterZoom/Assets/ZoomElementBackground");
	private Func<string> tooltipFunction;
	private const int ElementHeight = 400;

	public ZoomIndicatorElement()
	{
		Height.Set(ElementHeight, 0);
	}
	
	public override void OnBind()
	{
		base.OnBind();
		tooltipFunction = TooltipFunction; // initial TooltipFunction to reset to
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		base.DrawSelf(spriteBatch);

		var rect = GetDimensions().ToRectangle();
		const int YOffset = 35;
		const int EdgeOffset = 10;

		var backgroundRect = new Rectangle(
			rect.X + EdgeOffset,
			rect.Y + YOffset,
			rect.Width - EdgeOffset * 2,
			ElementHeight - YOffset - EdgeOffset
		);

		TooltipFunction = backgroundRect.Contains(Main.MouseScreen.ToPoint()) ? null : tooltipFunction; // hide tooltip when hovering background

		float v = GetValue();
		spriteBatch.Draw(Background.Value, backgroundRect, Color.White);

		const float BackgroundZoom = 0.35f; // zoom value that was used for the background

		var rec100 = Shrink(backgroundRect, BackgroundZoom / 1f);
		DrawBorder(rec100, spriteBatch, 1, Color.White);
		spriteBatch.DrawString(FontAssets.MouseText.Value, "100%", rec100.BottomLeft(), Color.White, 0, Vector2.Zero, new Vector2(0.5f), SpriteEffects.None, 0);

		var rec50 = Shrink(backgroundRect, BackgroundZoom / 0.5f);
		DrawBorder(rec50, spriteBatch, 1, Color.DimGray);
		spriteBatch.DrawString(FontAssets.MouseText.Value, "50%", rec50.BottomLeft(), Color.White, 0, Vector2.Zero, new Vector2(0.5f), SpriteEffects.None, 0);

		if (v >= BackgroundZoom - 0.001) {
			var recV = Shrink(backgroundRect, BackgroundZoom / v);
			DrawBorder(recV, spriteBatch, 1, Color.Firebrick);
			spriteBatch.DrawString(FontAssets.MouseText.Value, $"{v:P0}", recV.BottomLeft(), Color.White, 0, Vector2.Zero, new Vector2(0.5f), SpriteEffects.None, 0);
		}
	}

	private static void DrawBorder(Rectangle rect, SpriteBatch spriteBatch, int borderWidth, Color color)
	{
		var pixel = TextureAssets.MagicPixel.Value;

		// Top
		spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, borderWidth), color);
		// Bottom
		spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - borderWidth, rect.Width, borderWidth), color);
		// Left
		spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y + borderWidth, borderWidth, rect.Height - borderWidth * 2), color);
		// Right
		spriteBatch.Draw(pixel, new Rectangle(rect.Right - borderWidth, rect.Y + borderWidth, borderWidth, rect.Height - borderWidth * 2), color);
	}

	private static Rectangle Shrink(Rectangle rect, float multiplier)
	{
		float w = rect.Width * multiplier;
		float h = rect.Height * multiplier;
		return new Rectangle(
			(int)(rect.X + (rect.Width - w) / 2),
			(int)(rect.Y + (rect.Height - h) / 2),
			(int)w,
			(int)h);
	}
}