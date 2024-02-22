using System.Reflection;
using MonoMod.Cil;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace BetterZoom.Edits;

internal static class ZoomEdits
{
	private static readonly Config config = ModContent.GetInstance<Config>();
	
	// Manual Hooks because 'On' doesn't have that one for some reason
	private delegate float orig_get_UIScaleMax(Main self);

	private static readonly MethodInfo m_UIScaleMax = typeof(Main).GetMethod("get_UIScaleMax", BindingFlags.Public | BindingFlags.Instance);

	public static void Load()
	{
		On_Main.UpdateViewZoomKeys += Main_UpdateViewZoomKeys;
		IL_Main.DoDraw += ModifyZoomBounds;
		Terraria.ModLoader.MonoModHooks.Add(m_UIScaleMax, ModifyUIScaleBounds);
	}

	private static void Main_UpdateViewZoomKeys(On_Main.orig_UpdateViewZoomKeys orig, Main self)
	{
		if (Main.inFancyUI) {
			return;
		}

		float num = 0.01f * Main.GameZoomTarget; // changed

		if (!Main.keyState.PressingShift()) { // <new />
			if (PlayerInput.Triggers.Current.ViewZoomIn) {
				Main.GameZoomTarget = Utils.Clamp(Main.GameZoomTarget + num, config.minZoom, config.maxZoom); // changed
			}

			if (PlayerInput.Triggers.Current.ViewZoomOut) {
				Main.GameZoomTarget = Utils.Clamp(Main.GameZoomTarget - num, config.minZoom, config.maxZoom); // changed
			}
		} // <new>
		else
		{
			float num1 = 0.01f * Main.UIScale;
			if (PlayerInput.Triggers.Current.ViewZoomIn) {
				Main.UIScale = Utils.Clamp(Main.UIScale + num1, config.minUIScale, config.maxUIScale);
				Main.temporaryGUIScaleSlider = Main.UIScale;
			}

			if (PlayerInput.Triggers.Current.ViewZoomOut) {
				Main.UIScale = Utils.Clamp(Main.UIScale - num1, config.minUIScale, config.maxUIScale);
				Main.temporaryGUIScaleSlider = Main.UIScale;
			} // </new>
		}
	}

	private static float ModifyUIScaleBounds(orig_get_UIScaleMax orig, Main self)
	{
		return config.maxUIScale;
	}

	private static void ModifyZoomBounds(ILContext il)
	{
		var c = new ILCursor(il);
		/*
			C# (L-48938):
				before:
					GameViewMatrix.Zoom = new Vector2(ForcedMinimumZoom * MathHelper.Clamp(GameZoomTarget, 1, 2));
				after:
					GameViewMatrix.Zoom = new Vector2(ForcedMinimumZoom * MathHelper.Clamp(GameZoomTarget, minUIScale, maxUIScale));

			IL:
				before:
					IL_0f59: nop
					IL_0f5a: ldsfld		class Terraria.Graphics.SpriteViewMatrix Terraria.Main::GameViewMatrix
					IL_0f5f: ldsfld		float32 Terraria.Main::ForcedMinimumZoom
					IL_0f64: ldsfld		float32 Terraria.Main::GameZoomTarget
					IL_0f69: ldc.r4		1
								<--- here
					IL_0f6e: ldc.r4		2
					IL_0f73: call		float32 [FNA]Microsoft.Xna.Framework.MathHelper::Clamp(float32, float32, float32)
				after:
					IL_0f64: ldsfld		float32 Terraria.Main::GameZoomTarget
				[~]	IL_0f69: ldc.r4		minUIScale
				[~]	IL_0f6e: ldc.r4		maxUIScale
					IL_0f73: call		float32 [FNA]Microsoft.Xna.Framework.MathHelper::Clamp(float32, float32, float32)
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdsfld<Main>("GameViewMatrix"),
			i => i.MatchLdsfld<Main>("ForcedMinimumZoom"),
			i => i.MatchLdsfld<Main>("GameZoomTarget"),
			i => i.MatchLdcR4(1)
		)) {
			throw new ILEditException($"BetterZoom.{nameof(ZoomEdits)}::{nameof(ModifyZoomBounds)}");
		}

		c.Prev.Operand = config.minZoom;

		c.Index++;

		/*
			IL_0f69: ldc.r4		minUIScale
			IL_0f6e: ldc.r4		2
				<--- here
		*/

		c.Prev.Operand = config.maxZoom;
	}
}
