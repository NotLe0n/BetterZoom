using System.Reflection;
using MonoMod.Cil;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace BetterZoom.Edits;

internal class ZoomEdits : ModSystem
{
	private static Config Config => ModContent.GetInstance<Config>();

	// Manual Hooks because 'On' doesn't have that one for some reason
	private delegate float orig_get_UIScaleMax(Main self);

	private static readonly MethodInfo UIScaleMax = typeof(Main).GetMethod("get_UIScaleMax", BindingFlags.Public | BindingFlags.Instance);

	public override void Load()
	{
		On_Main.UpdateViewZoomKeys += Main_UpdateViewZoomKeys;
		// fixes race condition which crashes the game
		Main.RunOnMainThread(() => { IL_Main.DoDraw += ModifyZoomBounds; });

		MonoModHooks.Add(UIScaleMax, ModifyUIScaleBounds);
	}

	private static void Main_UpdateViewZoomKeys(On_Main.orig_UpdateViewZoomKeys orig, Main self)
	{
		if (Main.inFancyUI) {
			return;
		}

		float num = Config.zoomSpeed / 100 * Main.GameZoomTarget; // changed

		if (!Main.keyState.PressingShift()) { // <new />
			if (PlayerInput.Triggers.Current.ViewZoomIn) {
				Main.GameZoomTarget = Utils.Clamp(Main.GameZoomTarget + num, Config.zoomRange.min, Config.zoomRange.max); // changed
			}

			if (PlayerInput.Triggers.Current.ViewZoomOut) {
				Main.GameZoomTarget = Utils.Clamp(Main.GameZoomTarget - num, Config.zoomRange.min, Config.zoomRange.max); // changed
			}
		} // <new>
		else if (!Config.disableUIZoomHotkey) {
			float num1 = Config.zoomSpeed / 100 * Main.UIScale;
			if (PlayerInput.Triggers.Current.ViewZoomIn) {
				Main.UIScale = Utils.Clamp(Main.UIScale + num1, Config.UIScaleRange.min, Config.UIScaleRange.max);
				Main.temporaryGUIScaleSlider = Main.UIScale;
			}

			if (PlayerInput.Triggers.Current.ViewZoomOut) {
				Main.UIScale = Utils.Clamp(Main.UIScale - num1, Config.UIScaleRange.min, Config.UIScaleRange.max);
				Main.temporaryGUIScaleSlider = Main.UIScale;
			} // </new>
		}
	}

	private static float ModifyUIScaleBounds(orig_get_UIScaleMax orig, Main self)
	{
		return Config.UIScaleRange.max;
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
				[~]	IL_0f69: callvirt 	() => minUIScale
				[~]	IL_0f6e: callvirt	() => maxUIScale
					IL_0f73: call		float32 [FNA]Microsoft.Xna.Framework.MathHelper::Clamp(float32, float32, float32)
		*/

		if (!c.TryGotoNext(MoveType.After,
			    i => i.MatchLdsfld<Main>("GameViewMatrix"),
			    i => i.MatchLdsfld<Main>("ForcedMinimumZoom"),
			    i => i.MatchLdsfld<Main>("GameZoomTarget")
		    )) {
			throw new ILEditException($"{nameof(ZoomEdits)}::{nameof(ModifyZoomBounds)}");
		}

		c.Remove();
		c.EmitDelegate(() => Config.zoomRange.min);


		/*
			IL_0f69: callvirt	() => minUIScale
					<--- here
			IL_0f6e: ldc.r4		2
		*/

		c.Remove();
		c.EmitDelegate(() => Config.zoomRange.max);
	}
}