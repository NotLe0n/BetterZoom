using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BetterZoom.Edits;

internal static class SettingsEdits
{
	public static void Load()
	{
		IL_IngameOptions.Draw += IngameOptions_Draw;
	}

	private static void IngameOptions_Draw(ILContext il)
	{
		var c = new ILCursor(il);

		AddInputModeToggle(c);
		ModifyZoomSlider(c);
		ModifyUIScaleSlider(c);
	}
	
	private static readonly Asset<Texture2D> SliderButtonAsset = ModContent.Request<Texture2D>("BetterZoom/Assets/SliderButton");
	private static readonly Asset<Texture2D> TextInputAsset = ModContent.Request<Texture2D>("BetterZoom/Assets/TextInputButton");
	private static readonly Asset<Texture2D> ButtonHoveredAsset = ModContent.Request<Texture2D>("BetterZoom/Assets/ButtonHovered");

	private static void AddInputModeToggle(ILCursor c)
	{
		/*
			C#:
				IngameOptions.DrawRightSide(sb, Language.GetTextValue("GameUI.ZoomCategory"), num12, vector3, vector4, IngameOptions.rightScale[num12], 1f, default(Color));
				IngameOptions.skipRightSlot[num12] = true;
				
			[+] DrawInputModeToggle(sb, vector3, vector4, num12);
				
				num12++;
				vector3.X -= (float)num;
			IL:
				IL_110D: ldstr     "GameUI.ZoomCategory"
				IL_1112: call      string Terraria.Localization.Language::GetTextValue(string)
				IL_1117: ldloc.s   num12
				IL_1119: ldloc.s   vector3
				IL_111B: ldloc.s   vector4
				IL_111D: ldsfld    float32[] Terraria.IngameOptions::rightScale
				IL_1122: ldloc.s   num13
				IL_1124: ldelem.r4
				IL_1125: ldc.r4    1
				IL_112A: ldloca.s  V_29
				IL_112C: initobj   [FNA]Microsoft.Xna.Framework.Color
				IL_1132: ldloc.s   V_29
				IL_1134: call      bool Terraria.IngameOptions::DrawRightSide(class [FNA]Microsoft.Xna.Framework.Graphics.SpriteBatch, string, int32, valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2, float32, float32, valuetype [FNA]Microsoft.Xna.Framework.Color)
				IL_1139: pop
				IL_113A: ldsfld    bool[] Terraria.IngameOptions::skipRightSlot
				IL_113F: ldloc.s   num12
				IL_1141: ldc.i4.1
				IL_1142: stelem.i1
								<----- Here
				[+]	NEW: ldarg	   1
				[+]	NEW: ldloc     vector3
				[+]	NEW: ldloc     vector4
				[+]	NEW: ldloc     num12
				[+]	NEW: call	   DrawInputModeToggle
		*/

		int vector3 = 0, vector4 = 0, num12 = 0;
		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdstr("GameUI.ZoomCategory"),
			i => i.MatchCall(typeof(Language).GetMethod("GetTextValue", 0, new[] { typeof(string) })),
			i => i.MatchLdloc(out num12),
			i => i.MatchLdloc(out vector3),
			i => i.MatchLdloc(out vector4),
			i => i.MatchLdsfld(typeof(IngameOptions).GetField("rightScale")),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchLdelemR4(),
			i => i.MatchLdcR4(1),
			i => i.Match(OpCodes.Ldloca_S),
			i => i.MatchInitobj<Color>(),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchCall(typeof(IngameOptions).GetMethod("DrawRightSide")),
			i => i.MatchPop(),
			i => i.MatchLdsfld(typeof(IngameOptions).GetField("skipRightSlot")),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchLdcI4(1),
			i => i.MatchStelemI1()
		    )) 
		{
			throw new ILEditException($"BetterZoom.{nameof(SettingsEdits)}::{nameof(AddInputModeToggle)}");
		}

		c.Emit(OpCodes.Ldarg, 1);
		c.Emit(OpCodes.Ldloc, vector3);
		c.Emit(OpCodes.Ldloc, vector4);
		c.Emit(OpCodes.Ldloc, num12);
		c.EmitDelegate(DrawInputModeToggle);
	}

	private static bool _textInput;
	
	private static void DrawInputModeToggle(SpriteBatch sb, Vector2 v, Vector2 v2, int i)
	{
		var zoomTextDim = FontAssets.MouseText.Value.MeasureString(Language.GetTextValue("GameUI.ZoomCategory"));
		var pos = v + v2 * (1+i) + new Vector2(zoomTextDim.X, -zoomTextDim.Y / 2);
		var btnRect = new Rectangle((int)pos.X, (int)pos.Y, 20, 20);

		if (_textInput) {
			sb.Draw(SliderButtonAsset.Value, btnRect, Color.White);
		}
		else {
			sb.Draw(TextInputAsset.Value, btnRect, Color.White);
		}

		if (btnRect.Contains(Main.MouseScreen.ToPoint())) {
			if (!btnRect.Contains(new Point(Main.lastMouseX, Main.lastMouseY))) {
				SoundEngine.PlaySound(SoundID.MenuTick);
			}

			if (Main.mouseLeft && Main.mouseLeftRelease) {
				_textInput = !_textInput;
				SoundEngine.PlaySound(SoundID.MenuTick);
			}
				
			sb.Draw(ButtonHoveredAsset.Value, btnRect, Main.OurFavoriteColor);
		}
	}

	private static void ModifyZoomSlider(ILCursor c)
	{
		HookHoveringZoomText(c);
		ModifyZoomInput(c);
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
				IL_xxxx: [+] stloc.s V_126
				IL_xxxx: [+] ldloc.s V_126
				IL_11f3: brfalse.s IL_120a
				IL_xxxx: nop

				// if (rightLock == -1)
				IL_11f5: ldsfld int32 Terraria.IngameOptions::rightLock
				IL_11fa: ldc.i4.m1
				IL_11fb: ceq
						<=== here
			[+]		   : ldstr "Click to reset to 100%"
			[+]		   : stsfld string Terraria.IngameOptions::_mouseOverText
			[+]		   : <click action>
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdarg(1),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchLdsfld(typeof(IngameOptions).GetField("rightScale", BindingFlags.Public | BindingFlags.Static)),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchLdelemR4(),
			i => i.MatchLdcR4(0.85f),
			i => i.MatchMul(),
			i => i.MatchLdsfld(typeof(IngameOptions).GetField("rightScale", BindingFlags.Public | BindingFlags.Static)),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchLdelemR4(),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchSub(),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchSub(),
			i => i.MatchDiv(),
			i => i.Match(OpCodes.Ldloca_S),
			i => i.MatchInitobj<Color>(),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchCall(typeof(IngameOptions).GetMethod(nameof(IngameOptions.DrawRightSide), BindingFlags.Public | BindingFlags.Static)),
			i => i.Match(OpCodes.Stloc_S),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.Match(OpCodes.Brfalse_S),
			i => i.Match(OpCodes.Nop),
			i => i.MatchLdsfld(typeof(IngameOptions).GetField(nameof(IngameOptions.rightLock), BindingFlags.Public | BindingFlags.Static)),
			i => i.MatchLdcI4(-1),
			i => i.Match(OpCodes.Ceq)
		)) {
			throw new ILEditException($"BetterZoom.{nameof(SettingsEdits)}::{nameof(HookHoveringZoomText)}");
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
	private static void ModifyZoomInput(ILCursor c)
	{
		var inputBox = new ZoomInputBox(Main.GameZoomTarget);

		/*
			C# (L-516):
				before:
					float num14 = DrawValueBar(sb, scale, Main.GameZoomTarget - 1f);
				after:
					float num14;
					if (!_textInput) {
						num14 = DrawValueBar(sb, scale, MathHelper.Clamp((Main.GameZoomTarget - MIN_GAME_ZOOM) / (MAX_GAME_ZOOM - MIN_GAME_ZOOM), 0, 1));
					}
					else {
						...
						num14 = DrawInputTextBox(...);
					}
			IL (1242):
				before:
					IL_1242: ldarg		1
					IL_1243: ldloc.s	6
					IL_1245: ldsfld		float32 Terraria.Main::GameZoomTarget
					IL_124A: ldc.r4		1
					IL_124F: sub
					IL_1250: ldc.i4		0
					IL_1251: ldnull
					IL_1252: call      float32 Terraria.IngameOptions::DrawValueBar(...)
					IL_1257: stloc.s   num14
				after:
				[+] IL_####: ldsfld		bool BetterZoom.SettingsEdits::textInput
				[+] IL_####: brtrue		drawInputBoxLabel
					IL_1242: ldarg		1
					IL_1243: ldloc.s	6
					IL_1245: ldsfld		float32 Terraria.Main::GameZoomTarget
				[~]	IL_####: ldc.r4		MIN_GAME_ZOOM
					IL_124F: sub
				[+]	IL_####: ldc.r4		MAX_GAME_ZOOM - MIN_GAME_ZOOM
				[+]	IL_####: div
				[+]	IL_####: ldc.r4		0.0
				[+]	IL_####: ldc.r4		1
				[+]	IL_####: call		float32 [FNA]Microsoft.Xna.Framework.MathHelper::Clamp(float32, float32, float32)
					IL_1250: ldc.i4		0
					IL_1251: ldnull
					IL_1252: call      float32 Terraria.IngameOptions::DrawValueBar(...)
					IL_1257: stloc.s   num14
				[+] IL_####: br	       afterBlock
				[+] drawInputBoxLabel: ldarg.1
				[+] IL_####: ldloc	   6
				[+] IL_####: call	   <delegate>
				[+] afterblock:
		*/
		
		if (!c.TryGotoNext(MoveType.Before,
			    i => i.MatchLdarg(1),
			    i => i.Match(OpCodes.Ldloc_S),
			    i => i.MatchLdsfld<Main>("GameZoomTarget"),
			    i => i.MatchLdcR4(1)
		    )) {
			throw new ILEditException($"BetterZoom.{nameof(SettingsEdits)}::{nameof(ModifyZoomInput)}");
		}
		
		// #### if (!textInput) {
		var drawInputBoxLabel = c.DefineLabel();
		c.Emit(OpCodes.Ldsfld, typeof(SettingsEdits).GetField(nameof(_textInput), BindingFlags.Static | BindingFlags.NonPublic));
		c.Emit(OpCodes.Brtrue, drawInputBoxLabel);

		if (!c.TryGotoNext(MoveType.After,
			    i => i.MatchLdarg(1),
			    i => i.Match(OpCodes.Ldloc_S),
			    i => i.MatchLdsfld<Main>("GameZoomTarget"),
			    i => i.MatchLdcR4(1)
		    )) {
			throw new ILEditException($"BetterZoom.{nameof(SettingsEdits)}::{nameof(ModifyZoomInput)}");
		}

		c.Previous.Operand = BetterZoom.MinGameZoom;

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchSub()
		    )) {
			throw new ILEditException($"BetterZoom.{nameof(SettingsEdits)}::{nameof(ModifyZoomInput)}");
		}

		c.Emit(OpCodes.Ldc_R4, BetterZoom.MaxGameZoom - BetterZoom.MinGameZoom);
		c.Emit(OpCodes.Div);
		c.Emit(OpCodes.Ldc_R4, 0.0f);
		c.Emit(OpCodes.Ldc_R4, 1.0f);
		c.Emit(OpCodes.Call, typeof(MathHelper).GetMethod("Clamp"));

		if (!c.TryGotoNext(MoveType.After,
			    i => i.MatchLdcI4(0),
			    i => i.MatchLdnull(),
			    i => i.MatchCall(typeof(IngameOptions).GetMethod("DrawValueBar")),
			    i => i.Match(OpCodes.Stloc_S)
		    )) {
			throw new ILEditException($"BetterZoom.{nameof(SettingsEdits)}::{nameof(ModifyZoomInput)}");
		}

		var afterBlock = c.DefineLabel();
		c.Emit(OpCodes.Br, afterBlock);
		// #### }
		
		
		// #### else {
		c.MarkLabel(drawInputBoxLabel);

		c.Emit(OpCodes.Ldarg_1); // sb
		c.Emit(OpCodes.Ldloc, 6); // scale
		c.EmitDelegate((SpriteBatch sb, float scale) =>
		{
			if (!inputBox.Focused) {
				inputBox.ZoomValue = Main.GameZoomTarget;
			}

			inputBox.OnChange += zoomVal =>
			{
				Main.GameZoomTarget = MathHelper.Clamp(zoomVal, BetterZoom.MinGameZoom, BetterZoom.MaxGameZoom);
			};
			
			return DrawInputTextBox(inputBox, sb, scale);
		});

		c.Emit(OpCodes.Stloc, 41);

		c.MarkLabel(afterBlock);

		// ##### }
		
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
			throw new ILEditException($"BetterZoom.{nameof(SettingsEdits)}::{nameof(ModifyZoomInput)}");
		}

		c.Prev.Operand = BetterZoom.MaxGameZoom - BetterZoom.MinGameZoom;

		c.Emit(OpCodes.Mul);
		c.Emit(OpCodes.Ldc_R4, BetterZoom.MinGameZoom);
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
				IL_xxxx: stloc.s      V_134
				IL_xxxx: ldloc.s      V_134
				IL_13d9: brfalse.s	  IL_13f0	
				
				IL_xxxx: nop

				// if (rightLock == -1)
				IL_13db: ldsfld int32 Terraria.IngameOptions::rightLock
				IL_13e0: ldc.i4.m1
				IL_13e1: ceq IL_13e9
				IL_xxxx: stloc
				IL_xxxx: ldloc
				IL_xxxx: brfalse.s
				
						<=== here
			[+]		   : ldstr "Click to reset to 100%"
			[+]		   : stsfld string Terraria.IngameOptions::_mouseOverText
			[+]		   : <click action>
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdarg(1),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchLdsfld(typeof(IngameOptions).GetField("rightScale", BindingFlags.Public | BindingFlags.Static)),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchLdelemR4(),
			i => i.MatchLdcR4(0.75f),
			i => i.MatchMul(),
			i => i.MatchLdsfld(typeof(IngameOptions).GetField("rightScale", BindingFlags.Public | BindingFlags.Static)),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchLdelemR4(),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchSub(),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchSub(),
			i => i.MatchDiv(),
			i => i.Match(OpCodes.Ldloca_S),
			i => i.MatchInitobj<Color>(),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchCall(typeof(IngameOptions).GetMethod(nameof(IngameOptions.DrawRightSide), BindingFlags.Public | BindingFlags.Static)),
			i => i.Match(OpCodes.Stloc_S),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.Match(OpCodes.Brfalse_S),
			i => i.MatchNop(),
			i => i.MatchLdsfld(typeof(IngameOptions).GetField(nameof(IngameOptions.rightLock), BindingFlags.Public | BindingFlags.Static)),
			i => i.MatchLdcI4(-1),
			i => i.Match(OpCodes.Ceq),
			i => i.Match(OpCodes.Stloc_S),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.Match(OpCodes.Brfalse_S)
		)) {
			throw new ILEditException($"BetterZoom.{nameof(SettingsEdits)}::{nameof(HookHoveringUIScaleText)}");
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
		var inputBox = new ZoomInputBox(Main.UIScale);

		/*
			C# (L-537):
				before:
					float num15 = DrawValueBar(sb, scale, MathHelper.Clamp((Main.temporaryGUIScaleSlider - 0.5f) / 1.5f, 0f, 1f));
				after:
					float num15;
					if(!_textInput) {
						num15 = DrawValueBar(sb, scale, MathHelper.Clamp((Main.temporaryGUIScaleSlider - MIN_UI_ZOOM) / (MAX_UI_ZOOM - MIN_UI_ZOOM), 0f, 1f));
					}
					else {
						...
						num15 = DrawInputTextBox(...);
					}
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
				[+] IL_####: ldsfld		bool BetterZoom.SettingsEdits::textInput
				[+] IL_####: brtrue		drawInputBoxLabel
					IL_16c0: ldarg		1
					IL_16c1: ldloc.s	6
					IL_16c3: ldsfld		float32 Terraria.Main::temporaryGUIScaleSlider
				[~]	IL_16c8: ldc.r4		MIN_UI_ZOOM
					IL_16cd: sub
				[~]	IL_16ce: ldc.r4		MAX_UI_ZOOM - MIN_UI_ZOOM
					IL_16d3: div
					IL_16d4: ldc.r4		0.0
					IL_16e0: ldc.i4		0
					IL_16e1: ldnull
					IL_16e2: call      float32 Terraria.IngameOptions::DrawValueBar(...)
					IL_16e7: stloc.s   num14
				[+] IL_####: br	       afterBlock
				[+] drawInputBoxLabel: ldarg.1
				[+] IL_####: ldloc	   6
				[+] IL_####: call	   <delegate>
				[+] afterblock:
		*/

		if (!c.TryGotoNext(MoveType.Before,
			    i => i.MatchLdarg(1),
			    i => i.Match(OpCodes.Ldloc_S),
			    i => i.MatchLdsfld<Main>("temporaryGUIScaleSlider"),
			    i => i.MatchLdcR4(0.5f)
		    )) {
			throw new ILEditException($"BetterZoom.{nameof(SettingsEdits)}::{nameof(IncreaseUIScaleBound)}");
		}
		
		// #### if (!textInput) {
		var drawInputBoxLabel = c.DefineLabel();
		c.Emit(OpCodes.Ldsfld, typeof(SettingsEdits).GetField(nameof(_textInput), BindingFlags.Static | BindingFlags.NonPublic));
		c.Emit(OpCodes.Brtrue, drawInputBoxLabel);
		
		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdarg(1),
			i => i.Match(OpCodes.Ldloc_S),
			i => i.MatchLdsfld<Main>("temporaryGUIScaleSlider"),
			i => i.MatchLdcR4(0.5f)
		    )) {
			throw new ILEditException($"BetterZoom.{nameof(SettingsEdits)}::{nameof(IncreaseUIScaleBound)}");
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
		
		if (!c.TryGotoNext(MoveType.After,
			    i => i.MatchLdcI4(0),
			    i => i.MatchLdnull(),
			    i => i.MatchCall(typeof(IngameOptions).GetMethod("DrawValueBar")),
			    i => i.Match(OpCodes.Stloc_S)
		    )) {
			throw new ILEditException($"BetterZoom.{nameof(SettingsEdits)}::{nameof(ModifyZoomInput)}");
		}

		var afterBlock = c.DefineLabel();
		c.Emit(OpCodes.Br, afterBlock);
		// #### }
		
		// #### else {
		c.MarkLabel(drawInputBoxLabel);

		c.Emit(OpCodes.Ldarg_1); // sb
		c.Emit(OpCodes.Ldloc, 6); // scale
		c.EmitDelegate((SpriteBatch sb, float scale) =>
		{
			if (!inputBox.Focused) {
				inputBox.ZoomValue = Main.UIScale;
			}

			inputBox.OnChange += zoomVal =>
			{
				Main.UIScale = MathHelper.Clamp(zoomVal, BetterZoom.MIN_UI_ZOOM, BetterZoom.MAX_UI_ZOOM);
			};
			
			return DrawInputTextBox(inputBox, sb, scale);
		});

		c.Emit(OpCodes.Stloc, 44);

		c.MarkLabel(afterBlock);
		
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
			throw new ILEditException($"BetterZoom.{nameof(SettingsEdits)}::{nameof(IncreaseUIScaleBound)}");
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

	private static float DrawInputTextBox(ZoomInputBox inputBox, SpriteBatch sb, float scale)
	{
		var colorBarSize = TextureAssets.ColorBar.Size() * scale;
		IngameOptions.valuePosition.X -= colorBarSize.X;
		IngameOptions.valuePosition.Y -= colorBarSize.Y;

		var pos = new Vector2(IngameOptions.valuePosition.X, IngameOptions.valuePosition.Y);
		inputBox.Draw(sb, pos, 135, 20, scale);

		return inputBox.Focused ? 0 : 1;
	}
}
