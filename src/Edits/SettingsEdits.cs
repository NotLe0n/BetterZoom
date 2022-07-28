using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;

namespace BetterZoom.src.Edits;

internal class SettingsEdits
{
	public static void Load()
	{
		IL.Terraria.IngameOptions.Draw += IngameOptions_Draw;
	}

	private static void IngameOptions_Draw(ILContext il)
	{
		var c = new ILCursor(il);

		ModifyZoomSlider(c);

		ModifyUIScaleSlider(c);
	}

	private static void ModifyZoomSlider(ILCursor c)
	{
		/*
			C# (L-502):
				before:
					float num14 = DrawValueBar(sb, scale, Main.GameZoomTarget - 1f);
				after:
					float num14 = DrawValueBar(sb, scale, MathHelper.Clamp((Main.GameZoomTarget - MIN_GAME_ZOOM) / (MAX_GAME_ZOOM - MIN_GAME_ZOOM), 0, 1));
			IL:
				before:
					IL_1495: ldarg		1
					IL_1496: ldloc.s	6
					IL_1498: ldsfld		float32 Terraria.Main::GameZoomTarget
					IL_149d: ldc.r4		1
					IL_14a2: sub
					IL_14a3: ldc.i4		0
					IL_14a4: ldnull
				after:
					IL_1495: ldarg		1
					IL_1496: ldloc.s	6
					IL_1498: ldsfld		float32 Terraria.Main::GameZoomTarget
				[~]	IL_149d: ldc.r4		MIN_GAME_ZOOM
					IL_14a2: sub
				[+]	IL_14a3: ldc.r4		MAX_GAME_ZOOM - MIN_GAME_ZOOM
				[+]	IL_14a8: div
				[+]	IL_14a9: ldc.r4		0.0
				[+]	IL_14ae: ldc.r4		1
				[+]	IL_14b3: call		float32 [FNA]Microsoft.Xna.Framework.MathHelper::Clamp(float32, float32, float32)
					IL_14b8: ldc.i4		0
					IL_14b9: ldnull
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdarg(1),
			i => i.MatchLdloc(6),
			i => i.MatchLdsfld<Main>("GameZoomTarget"),
			i => i.MatchLdcR4(1)
			)) {
			throw new("IL edit at BetterZoom.SettingsEdits::ModifyZoomSlider failed! Please contact NotLe0n!");
		}

		c.Previous.Operand = BetterZoom.MIN_GAME_ZOOM;

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchSub()
			)) {
			throw new("IL edit at BetterZoom.SettingsEdits::ModifyZoomSlider failed! Please contact NotLe0n!");
		}

		c.Emit(OpCodes.Ldc_R4, BetterZoom.MAX_GAME_ZOOM - BetterZoom.MIN_GAME_ZOOM);
		c.Emit(OpCodes.Div);
		c.Emit(OpCodes.Ldc_R4, 0.0f);
		c.Emit(OpCodes.Ldc_R4, 1.0f);
		c.Emit(OpCodes.Call, typeof(MathHelper).GetMethod("Clamp"));
		c.Index++;

		/* 
			C# (L-506):
				before:
					Main.GameZoomTarget = num14 + 1f;
				after:
					Main.GameZoomTarget = num14 * (MAX_GAME_ZOOM - MIN_GAME_ZOOM) + MIN_GAME_ZOOM;
			IL:
				before:
					IL_14ea: ldloc.s		128
					IL_14ec: brfalse.s		IL_14fb
					IL_14ee: ldloc.s		96
					IL_14f0: ldc.r4			1f
					IL_14f5: add
					IL_14f6: stsfld			float32 Terraria.Main::GameZoomTarget
				after:
					IL_14ea: ldloc.s		128
					IL_14ec: brfalse.s		IL_14fb
					IL_1503: ldloc.s		96
				[~]	IL_1505: ldc.r4			MAX_GAME_ZOOM - MIN_GAME_ZOOM
				[+]	IL_150a: mul
				[+]	IL_150b: ldc.r4			MIN_GAME_ZOOM
					IL_1510: add
					IL_1511: stsfld			float32 Terraria.Main::GameZoomTarget
		*/

		if (!c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(1.0f))) {
			throw new("IL edit at BetterZoom.SettingsEdits::IngameOptions_Draw failed! Please contact NotLe0n!");
		}

		c.Prev.Operand = BetterZoom.MAX_GAME_ZOOM - BetterZoom.MIN_GAME_ZOOM;

		c.Emit(OpCodes.Mul);
		c.Emit(OpCodes.Ldc_R4, BetterZoom.MIN_GAME_ZOOM);
	}

	private static void ModifyUIScaleSlider(ILCursor c)
	{
		/*
			C# (L-537):
				before:
					float num15 = DrawValueBar(sb, scale, MathHelper.Clamp((Main.temporaryGUIScaleSlider - 0.5f) / 1.5f, 0f, 1f));
				after:
					float num15 = DrawValueBar(sb, scale, MathHelper.Clamp((Main.temporaryGUIScaleSlider - MIN_UI_ZOOM) / (MAX_UI_ZOOM - MIN_UI_ZOOM), 0f, 1f));
			IL:
				before:
					IL_16c0: ldarg		1
					IL_16c1: ldloc.s	6
					IL_16c3: ldsfld		float32 Terraria.Main::temporaryGUIScaleSlider
					IL_16c8: ldc.r4		0.5
					IL_16cd: sub
					IL_16ce: ldc.r4		1.5
					IL_16d3: div
					IL_16d4: ldc.r4		0.0
				after:
					IL_16c0: ldarg		1
					IL_16c1: ldloc.s	6
					IL_16c3: ldsfld		float32 Terraria.Main::temporaryGUIScaleSlider
				[~]	IL_16c8: ldc.r4		MIN_UI_ZOOM
					IL_16cd: sub
				[~]	IL_16ce: ldc.r4		MAX_UI_ZOOM - MIN_UI_ZOOM
					IL_16d3: div
					IL_16d4: ldc.r4		0.0
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdarg(1),
			i => i.MatchLdloc(6),
			i => i.MatchLdsfld<Main>("temporaryGUIScaleSlider"),
			i => i.MatchLdcR4(0.5f)
			)) {
			throw new("IL edit at BetterZoom.SettingsEdits::ModifyUIScaleSlider failed! Please contact NotLe0n!");
		}

		c.Prev.Operand = BetterZoom.MIN_UI_ZOOM;
		c.Index++;
		/* Current Position:

			IL_16c8: ldc.r4		MIN_UI_ZOOM		(0.5)
			IL_16cd: sub
				<--- here
			IL_16ce: ldc.r4		1.5
		*/

		c.Next.Operand = BetterZoom.MAX_UI_ZOOM - BetterZoom.MIN_UI_ZOOM;
		c.Index++;

		/*
			C# (L-541):
				before:
					Main.temporaryGUIScaleSlider = num15 * 1.5f + 0.5f;
				after:
					Main.temporaryGUIScaleSlider = num15 * (MAX_UI_ZOOM - MIN_UI_ZOOM) + MIN_UI_ZOOM;
			IL:
				before:
					IL_1728: stloc.s		137
					IL_172a: ldloc.s		137
					IL_172c: brfalse.s		IL_1764
					IL_172f: ldloc.s		99
					IL_1731: ldc.r4			1.5
					IL_1736: mul
					IL_1737: ldc.r4			0.5
					IL_173c: add
					IL_173d: stsfld			float32 Terraria.Main::temporaryGUIScaleSlider
				after:
					IL_1728: stloc.s		137
					IL_172a: ldloc.s		137
					IL_172c: brfalse.s		IL_1764
					IL_172f: ldloc.s		99
				[~]	IL_1731: ldc.r4			MAX_UI_ZOOM - MIN_UI_ZOOM
					IL_1736: mul
				[~]	IL_1737: ldc.r4			MIN_UI_ZOOM
					IL_173c: add
					IL_173d: stsfld			float32 Terraria.Main::temporaryGUIScaleSlider

		*/

		if (!c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(1.5f))) {
			throw new("IL edit at BetterZoom.SettingsEdits::ModifyUIScaleSlider failed! Please contact NotLe0n!");
		}

		c.Prev.Operand = BetterZoom.MAX_UI_ZOOM - BetterZoom.MIN_UI_ZOOM;

		c.Index++;

		/* Current Position

			IL_1731: ldc.r4		MAX_UI_ZOOM - MIN_UI_ZOOM		(1.5)
			IL_1736: mul
					<--- here
			IL_1737: ldc.r4		0.5
		*/
		c.Next.Operand = BetterZoom.MIN_UI_ZOOM;
	}
}
