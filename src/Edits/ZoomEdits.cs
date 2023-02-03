using System.Reflection;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using Terraria;
using Terraria.GameInput;

namespace BetterZoom.Edits;

internal static class ZoomEdits
{
	// Manual Hooks because 'On' doesn't have that one for some reason
	internal delegate float orig_get_UIScaleMax(Main self);
	internal delegate float hook_get_UIScaleMax(orig_get_UIScaleMax orig, Main self);

	private static readonly MethodInfo m_UIScaleMax = typeof(Main).GetMethod("get_UIScaleMax", BindingFlags.Public | BindingFlags.Instance);

	internal static event hook_get_UIScaleMax On_get_UIScaleMax {
		add => HookEndpointManager.Add(m_UIScaleMax, value);
		remove => HookEndpointManager.Remove(m_UIScaleMax, value);
	}

	public static void Load()
	{
		On.Terraria.Main.UpdateViewZoomKeys += Main_UpdateViewZoomKeys;
		IL.Terraria.Main.DoDraw += ModifyZoomBounds;
		On_get_UIScaleMax += ModifyUIScaleBounds;
	}

	private static void Main_UpdateViewZoomKeys(On.Terraria.Main.orig_UpdateViewZoomKeys orig, Main self)
	{
		if (Main.inFancyUI) {
			return;
		}

		float num = 0.01f * Main.GameZoomTarget; // changed

		if (!Main.keyState.PressingShift()) { // <new />
			if (PlayerInput.Triggers.Current.ViewZoomIn) {
				Main.GameZoomTarget = Utils.Clamp(Main.GameZoomTarget + num, BetterZoom.MinGameZoom, BetterZoom.MaxGameZoom); // changed
			}

			if (PlayerInput.Triggers.Current.ViewZoomOut) {
				Main.GameZoomTarget = Utils.Clamp(Main.GameZoomTarget - num, BetterZoom.MinGameZoom, BetterZoom.MaxGameZoom); // changed
			}
		}
		// <new>
		else {
			float num1 = 0.01f * Main.UIScale;
			if (PlayerInput.Triggers.Current.ViewZoomIn) {
				Main.UIScale = Utils.Clamp(Main.UIScale + num1, BetterZoom.MIN_UI_ZOOM, BetterZoom.MAX_UI_ZOOM);
				Main.temporaryGUIScaleSlider = Main.UIScale;
			}

			if (PlayerInput.Triggers.Current.ViewZoomOut) {
				Main.UIScale = Utils.Clamp(Main.UIScale - num1, BetterZoom.MIN_UI_ZOOM, BetterZoom.MAX_UI_ZOOM);
				Main.temporaryGUIScaleSlider = Main.UIScale;
			}
			// </new>
		}
	}

	private static float ModifyUIScaleBounds(orig_get_UIScaleMax orig, Main self)
	{
		return BetterZoom.MAX_UI_ZOOM;
	}

	private static void ModifyZoomBounds(ILContext il)
	{
		var c = new ILCursor(il);
		/*
			C# (L-48938):
				before:
					GameViewMatrix.Zoom = new Vector2(ForcedMinimumZoom * MathHelper.Clamp(GameZoomTarget, 1, 2));
				after:
					GameViewMatrix.Zoom = new Vector2(ForcedMinimumZoom * MathHelper.Clamp(GameZoomTarget, MIN_GAME_ZOOM, MAX_GAME_ZOOM));

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
				[~]	IL_0f69: ldc.r4		MIN_GAME_ZOOM
				[~]	IL_0f6e: ldc.r4		MAX_GAME_ZOOM
					IL_0f73: call		float32 [FNA]Microsoft.Xna.Framework.MathHelper::Clamp(float32, float32, float32)
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdsfld<Main>("GameViewMatrix"),
			i => i.MatchLdsfld<Main>("ForcedMinimumZoom"),
			i => i.MatchLdsfld<Main>("GameZoomTarget"),
			i => i.MatchLdcR4(1))) {
			throw new ILEditException($"BetterZoom.{nameof(ZoomEdits)}::{nameof(ModifyZoomBounds)}");
		}

		c.Prev.Operand = BetterZoom.MinGameZoom;

		c.Index++;

		/*
			IL_0f69: ldc.r4		MIN_GAME_ZOOM
			IL_0f6e: ldc.r4		2
				<--- here
		*/

		c.Prev.Operand = BetterZoom.MaxGameZoom;
	}
}
