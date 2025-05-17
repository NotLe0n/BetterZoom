using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace BetterZoom.Edits;

// Some edits by DogeRoll (Steam: another_one_ayanami_rei)

/// <summary>
/// Edits to allow more tiles to render and fix other rendering issues with zooming out
/// </summary>
public class RenderEdits : ModSystem
{
	private static readonly Config Config = ModContent.GetInstance<Config>();

	public override void Load()
	{
		LoadEdits();
	}

	public override void Unload()
	{
		ReloadRenderTargets();
		base.Unload();
	}

	private static void LoadEdits()
    {
	    if (!Config.renderMoreTiles) {
		    ReloadRenderTargets();
		    return;
	    }
	    
        On_Main.GetScreenOverdrawOffset += On_Main_GetScreenOverdrawOffset;
        IL_Main.InitTargets_int_int += IL_Main_InitTargets;
        IL_Main.DrawBlack += IL_Main_DrawBlack;
        IL_Main.DoDraw += FixBackgroundRender;
        
        ReloadRenderTargets();
    }

	private static void FixBackgroundRender(ILContext il)
	{
		var c = new ILCursor(il);
		
		/*
		
				stfld		bgTopY
			[+] ldarg.0
			[+] callvirt	<delegate>
			[+] stfld		bgStartX
			[+] ldarg.0
			[+] callvirt	<delegate>
			[+] stfld		bgLoops
			[+] ldarg.0
			[+] callvirt	<delegate>
			[+] stfld		bgTopY
			...
		*/
		
		if (!c.TryGotoNext(MoveType.After, i => i.MatchStfld<Main>("bgTopY"))) {
			throw new ILEditException($"{nameof(RenderEdits)}::{nameof(FixBackgroundRender)}");
		}

		c.Index++;

		ChangeBgField("bgStartX", () =>
		{
			int visibleWidth = (int)(Main.screenWidth / Main.GameZoomTarget);
			double parallax = Main.screenPosition.X * 0.1;
			int bgWidth = Main.backgroundWidth[Main.background];
			int offset = Math.Max(0, visibleWidth - Main.screenWidth);

			return (int)(0.0 - Math.IEEERemainder(parallax, bgWidth) - bgWidth / 2d - offset / 2d);
		});
		
		ChangeBgField("bgLoops", () =>
		{
			int visibleWidth = Math.Max(Main.screenWidth, (int)(Main.screenWidth / Main.GameZoomTarget));

			return visibleWidth / Main.backgroundWidth[Main.background] + 2;
		});
		
		ChangeBgField("bgTopY", () =>
		{
			int visibleHeight = (int)(Main.screenHeight / Main.GameZoomTarget);
			int offset = Math.Max(0, visibleHeight - Main.screenHeight);

			return (int)((offset / 2f - Main.screenPosition.Y) / (Main.worldSurface * 16.0 - 600.0) * 200.0);
		});
		return;

		void ChangeBgField(string fieldName, Func<int> function)
		{
			c.EmitLdarg0();
			c.EmitDelegate(function);
			c.EmitStfld(typeof(Main).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new());
		}
	}

	public static void ReloadRenderTargets()
    { 
        /* SetResolution() call InitTargets() function, that sets offscreen drawing area and renderers */
        Main.QueueMainThreadAction(() =>
        {
            try
            {
                var sw = Main.screenWidth;
                var sh = Main.screenHeight;
                Main.SetResolution(Main.screenWidth + 1, Main.screenHeight + 1);
                Main.SetResolution(Main.screenWidth - 1, Main.screenHeight - 1);
                Main.SetResolution(sw, sh);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"{ex}: This should only happen on server initialization.");
            }
        });
    }

    private static void IL_Main_DrawBlack(ILContext il)
    {
        ILCursor c = new ILCursor(il);

        /* 
            - num3 = point.X;
            + num3 = 0;
        */
        int idx = -1;
        if (!c.TryGotoNext(MoveType.Before,
	            i => i.MatchLdloc(out _),
	            i => i.MatchLdfld<Point>("X"),
	            i => i.MatchStloc(out idx))) {
	        throw new ILEditException($"{nameof(RenderEdits)}::{nameof(IL_Main_DrawBlack)} at edit 1");
        }
        

        c.Remove();
        c.Remove();
        c.Remove();
        c.Emit(OpCodes.Ldc_I4_0);
        c.Emit(OpCodes.Stloc_S, (byte)idx);

        /* 
            - num4 = Main.maxTilesX - point.X;
            + num4 = Main.maxTilesX;
        */
        if (!c.TryGotoNext(MoveType.Before,
	            i => i.MatchLdsfld(out _),
	            i => i.MatchLdloc(out _),
	            i => i.MatchLdfld<Point>("X"),
	            i => i.MatchSub(),
	            i => i.MatchStloc(out _))) {
	        throw new ILEditException($"{nameof(RenderEdits)}::{nameof(IL_Main_DrawBlack)} at edit 2");
        }
        c.Index++;
        c.Remove();
        c.Remove();
        c.Remove();

        /* 
            - num5 = point.Y;
            + num5 = 0;
        */
        if (!c.TryGotoNext(MoveType.Before,
	            i => i.MatchLdloc(out _),
	            i => i.MatchLdfld<Point>("Y"),
	            i => i.MatchStloc(out idx))) {
	        throw new ILEditException($"{nameof(RenderEdits)}::{nameof(IL_Main_DrawBlack)} at edit 3");
        }

        c.Remove();
        c.Remove();
        c.Remove();
        c.Emit(OpCodes.Ldc_I4_0);
        c.Emit(OpCodes.Stloc_S, (byte)idx);

        /* 
            - num6 = Main.maxTilesY - point.Y;
            + num6 = Main.maxTilesY;
        */
        if (!c.TryGotoNext(MoveType.Before,
	            i => i.MatchLdsfld(out _),
	            i => i.MatchLdloc(out _),
	            i => i.MatchLdfld<Point>("Y"),
	            i => i.MatchSub(),
	            i => i.MatchStloc(out _))) {
	        throw new ILEditException($"{nameof(RenderEdits)}::{nameof(IL_Main_DrawBlack)} at edit 4");
        }
        c.Index++;
        c.Remove();
        c.Remove();
        c.Remove();

    }
    
    private static int EvalOffset(int dim) => (int)(dim * (1.0f / Math.Min(1, Config.tileRenderLimit) - 1.0f) / 2);

    private static Point On_Main_GetScreenOverdrawOffset(On_Main.orig_GetScreenOverdrawOffset orig)
    {
	    return !Config.renderMoreTiles ? orig() : new Point(0, 0);
    }

    private static void IL_Main_InitTargets(ILContext il)
    {
        /* 

            ReleaseTargets();
            offScreenRange = 192 

            + _renderTargetMaxSize = maxScreenW * 3 + 400 * Main.maxScreenW / 1920;
	        + offScreenRange = 192 + EvalOffset;

            if (width + offScreenRange * 2 > _renderTargetMaxSize)
		        offScreenRange = (_renderTargetMaxSize - width) / 2;
        
         */

        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.After, 
            i => i.MatchStsfld<Main>("offScreenRange"))) {
	        throw new ILEditException($"{nameof(RenderEdits)}::{nameof(IL_Main_InitTargets)}");
        }
        
        FieldInfo maxScreenW = typeof(Main).GetField("maxScreenW");
        
        // maxScreenW * 2
        c.Emit(OpCodes.Ldsfld, maxScreenW);
        c.Emit(OpCodes.Ldc_I4_3);
        c.Emit(OpCodes.Mul);
        // + ((400 * maxScreenW) / 1920)
        c.Emit(OpCodes.Ldc_I4, 400);
        c.Emit(OpCodes.Ldsfld, maxScreenW);
        c.Emit(OpCodes.Mul);
        c.Emit(OpCodes.Ldc_I4, 1920);
        c.Emit(OpCodes.Div);
        c.Emit(OpCodes.Add);
        // _renderTargetMaxSize = result
        c.Emit(OpCodes.Stsfld, typeof(Main).GetField("_renderTargetMaxSize", BindingFlags.NonPublic | BindingFlags.Static));
        // offScreenRange = 192 + evalOffset
        c.Emit(OpCodes.Ldc_I4, 192);
        c.Emit(OpCodes.Ldarg_1);
        c.EmitDelegate(EvalOffset);
        c.Emit(OpCodes.Add);
        c.Emit(OpCodes.Stsfld, typeof(Main).GetField("offScreenRange")); 
    }
}