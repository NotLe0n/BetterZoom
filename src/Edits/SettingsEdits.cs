using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using System.Reflection;

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
		HookHoveringZoomText(c);
		IncreaseZoomBound(c);
	}

	// Adds a hover text and a click action which resets the game zoom to 100%
	private static void HookHoveringZoomText(ILCursor c)
	{
		/*
			C#:
				if (DrawRightSide(sb, text, num13, vector3, vector4, rightScale[num13] * 0.85f, (rightScale[num13] - num4) / (num5 - num4))) {
					if (rightLock == -1) {
							<=== here
					}
				}

			IL:
				IL_11bc: ldarg.1
				IL_11bd: ldloc.s 40
				IL_11bf: ldloc.s 36
				IL_11c1: ldloc.s 24
				IL_11c3: ldloc.s 25
				IL_11c5: ldsfld float32[] Terraria.IngameOptions::rightScale
				IL_11ca: ldloc.s 36
				IL_11cc: ldelem.r4
				IL_11cd: ldc.r4 0.85
				IL_11d2: mul
				IL_11d3: ldsfld float32[] Terraria.IngameOptions::rightScale
				IL_11d8: ldloc.s 36
				IL_11da: ldelem.r4
				IL_11db: ldloc.s 13
				IL_11dd: sub
				IL_11de: ldloc.s 14
				IL_11e0: ldloc.s 13
				IL_11e2: sub
				IL_11e3: div
				IL_11e4: ldloca.s 29
				IL_11e6: initobj [FNA]Microsoft.Xna.Framework.Color
				IL_11ec: ldloc.s 29
				IL_11ee: call bool Terraria.IngameOptions::DrawRightSide(class [FNA]Microsoft.Xna.Framework.Graphics.SpriteBatch, string, int32, valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2, float32, float32, valuetype [FNA]Microsoft.Xna.Framework.Color)
				IL_11f3: brfalse.s IL_120a

				// if (rightLock == -1)
				IL_11f5: ldsfld int32 Terraria.IngameOptions::rightLock
				IL_11fa: ldc.i4.m1
				IL_11fb: bne.un.s IL_1203
						<=== here
			[+]		   : ldstr "Click to reset to 100%"
			[+]		   : stsfld string Terraria.IngameOptions::_mouseOverText
			[+]		   : <click action>
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdarg(1),
			i => i.MatchLdloc(40),
			i => i.MatchLdloc(36),
			i => i.MatchLdloc(24),
			i => i.MatchLdloc(25),
			i => i.MatchLdsfld(typeof(IngameOptions).GetField("rightScale", BindingFlags.Public | BindingFlags.Static)),
			i => i.MatchLdloc(36),
			i => i.MatchLdelemR4(),
			i => i.MatchLdcR4(0.85f),
			i => i.MatchMul(),
			i => i.MatchLdsfld(typeof(IngameOptions).GetField("rightScale", BindingFlags.Public | BindingFlags.Static)),
			i => i.MatchLdloc(36),
			i => i.MatchLdelemR4(),
			i => i.MatchLdloc(13),
			i => i.MatchSub(),
			i => i.MatchLdloc(14),
			i => i.MatchLdloc(13),
			i => i.MatchSub(),
			i => i.MatchDiv(),
			i => i.MatchLdloca(29),
			i => i.MatchInitobj<Color>(),
			i => i.MatchLdloc(29),
			i => i.MatchCall(typeof(IngameOptions).GetMethod(nameof(IngameOptions.DrawRightSide), BindingFlags.Public | BindingFlags.Static)),
			i => i.MatchBrfalse(out _),
			i => i.MatchLdsfld(typeof(IngameOptions).GetField(nameof(IngameOptions.rightLock), BindingFlags.Public | BindingFlags.Static)),
			i => i.MatchLdcI4(-1),
			i => i.MatchBneUn(out _)
		)) {
			throw new($"IL edit at BetterZoom.SettingsEdits::HookHoveringZoomText failed! Please contact NotLe0n!");
		}

		// Add hover text
		c.Emit(OpCodes.Ldstr, "Click to reset to 100%");
		c.Emit(OpCodes.Stsfld, typeof(IngameOptions).GetField("_mouseOverText", BindingFlags.NonPublic | BindingFlags.Static));

		// Add click action
		c.EmitDelegate(() =>
		{
			if (!Main.mouseLeftRelease) {
				Main.GameZoomTarget = 1.0f;
			}
		});
	}

	// Changes the zoom slider to allow for a larger range of values
	private static void IncreaseZoomBound(ILCursor c)
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
			throw new("IL edit at BetterZoom.SettingsEdits::IncreaseZoomBound failed! Please contact NotLe0n!");
		}

		c.Previous.Operand = BetterZoom.MIN_GAME_ZOOM;

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchSub()
		    )) {
			throw new("IL edit at BetterZoom.SettingsEdits::IncreaseZoomBound failed! Please contact NotLe0n!");
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
			throw new("IL edit at BetterZoom.SettingsEdits::IncreaseZoomBound failed! Please contact NotLe0n!");
		}

		c.Prev.Operand = BetterZoom.MAX_GAME_ZOOM - BetterZoom.MIN_GAME_ZOOM;

		c.Emit(OpCodes.Mul);
		c.Emit(OpCodes.Ldc_R4, BetterZoom.MIN_GAME_ZOOM);
	}

	private static void ModifyUIScaleSlider(ILCursor c)
	{
		HookHoveringUIScaleText(c);
		IncreaseUIScaleBound(c);
	}

	// Adds a hover text and a click action which resets the game zoom to 100%
	private static void HookHoveringUIScaleText(ILCursor c)
	{
		/*
			C#:
				if (DrawRightSide(sb, text2, num13, vector6, vector7, rightScale[num13] * 0.75f, (rightScale[num13] - num4) / (num5 - num4))) {
					if (rightLock == -1) {
							<=== here
					}
				}

			IL:
				IL_13a2: ldarg.1
				IL_13a3: ldloc.s 43
				IL_13a5: ldloc.s 36
				IL_13a7: ldloc.s 24
				IL_13a9: ldloc.s 25
				IL_13ab: ldsfld float32[] Terraria.IngameOptions::rightScale
				IL_13b0: ldloc.s 36
				IL_13b2: ldelem.r4
				IL_13b3: ldc.r4 0.75
				IL_13b8: mul
				IL_13b9: ldsfld float32[] Terraria.IngameOptions::rightScale
				IL_13be: ldloc.s 36
				IL_13c0: ldelem.r4
				IL_13c1: ldloc.s 13
				IL_13c3: sub
				IL_13c4: ldloc.s 14
				IL_13c6: ldloc.s 13
				IL_13c8: sub
				IL_13c9: div
				IL_13ca: ldloca.s 29
				IL_13cc: initobj [FNA]Microsoft.Xna.Framework.Color
				IL_13d2: ldloc.s 29
				IL_13d4: call bool Terraria.IngameOptions::DrawRightSide(class [FNA]Microsoft.Xna.Framework.Graphics.SpriteBatch, string, int32, valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2, float32, float32, valuetype [FNA]Microsoft.Xna.Framework.Color)
				IL_13d9: brfalse.s IL_13f0

				// if (rightLock == -1)
				IL_13db: ldsfld int32 Terraria.IngameOptions::rightLock
				IL_13e0: ldc.i4.m1
				IL_13e1: bne.un.s IL_13e9
						<=== here
			[+]		   : ldstr "Click to reset to 100%"
			[+]		   : stsfld string Terraria.IngameOptions::_mouseOverText
			[+]		   : <click action>
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdarg(1),
			i => i.MatchLdloc(43),
			i => i.MatchLdloc(36),
			i => i.MatchLdloc(24),
			i => i.MatchLdloc(25),
			i => i.MatchLdsfld(typeof(IngameOptions).GetField("rightScale", BindingFlags.Public | BindingFlags.Static)),
			i => i.MatchLdloc(36),
			i => i.MatchLdelemR4(),
			i => i.MatchLdcR4(0.75f),
			i => i.MatchMul(),
			i => i.MatchLdsfld(typeof(IngameOptions).GetField("rightScale", BindingFlags.Public | BindingFlags.Static)),
			i => i.MatchLdloc(36),
			i => i.MatchLdelemR4(),
			i => i.MatchLdloc(13),
			i => i.MatchSub(),
			i => i.MatchLdloc(14),
			i => i.MatchLdloc(13),
			i => i.MatchSub(),
			i => i.MatchDiv(),
			i => i.MatchLdloca(29),
			i => i.MatchInitobj<Color>(),
			i => i.MatchLdloc(29),
			i => i.MatchCall(typeof(IngameOptions).GetMethod(nameof(IngameOptions.DrawRightSide), BindingFlags.Public | BindingFlags.Static)),
			i => i.MatchBrfalse(out _),
			i => i.MatchLdsfld(typeof(IngameOptions).GetField(nameof(IngameOptions.rightLock), BindingFlags.Public | BindingFlags.Static)),
			i => i.MatchLdcI4(-1),
			i => i.MatchBneUn(out _)
		)) {
			throw new($"IL edit at BetterZoom.SettingsEdits::HookHoveringUIScaleText failed! Please contact NotLe0n!");
		}

		// Add hover text
		c.Emit(OpCodes.Ldstr, "Click to reset to 100%");
		c.Emit(OpCodes.Stsfld, typeof(IngameOptions).GetField("_mouseOverText", BindingFlags.NonPublic | BindingFlags.Static));

		// Add click action
		c.EmitDelegate(() =>
		{
			if (!Main.mouseLeftRelease) {
				Main.temporaryGUIScaleSlider = 1.0f;
				Main.UIScale = 1.0f;
			}
		});
	}

	// Changes the ui scale slider to allow for a larger range of values
	private static void IncreaseUIScaleBound(ILCursor c)
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
			throw new("IL edit at BetterZoom.SettingsEdits::IncreaseUIScaleBound failed! Please contact NotLe0n!");
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
			throw new("IL edit at BetterZoom.SettingsEdits::IncreaseUIScaleBound failed! Please contact NotLe0n!");
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
