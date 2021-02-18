using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

namespace BetterZoom.src
{
    public class ILEdits
    {
        public static void Load()
        {
            IL.Terraria.Main.DrawMap += HookMap;
        }
        public static void HookMap(ILContext il)
        {
            var c = new ILCursor(il);

            #region size
            // Find Main.minimapWidth = 240;
            /*
                IL_0C0C: ldsfld int32   Terraria.Main::mapStyle
                IL_0C11: ldc.i4.1
                IL_0C12: bne.un         IL_0E16
                IL_0C17: ldc.i4         240
            */
            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdsfld(typeof(Main).GetField("mapStyle")),
                i => i.MatchLdcI4(1),
                i => i.MatchBneUn(out _),
                i => i.MatchLdcI4(240)))
                return;

            // Change "Main.minimapWidth = 240;" to "Main.minimapWidth = 240 * minimapScale;"
            c.EmitDelegate<Func<int, int>>((returnvalue) =>
            {
                return (int)(240 * BetterZoom.minimapScale);
            });

            // Find Main.minimapHeight = 240;
            /*
             *   IL_0C1C: stsfld    int32 Terraria.Main::miniMapWidth
             *   IL_0C21: ldc.i4    240
            */
            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchStsfld(typeof(Main).GetField("miniMapWidth")),
                i => i.MatchLdcI4(240)))
                return;

            // Change "Main.minimapHeight = 240;" to "Main.minimapHeight = 240 * minimapScale;"
            c.EmitDelegate<Func<int, int>>((returnvalue) =>
            {
                return (int)(240 * BetterZoom.minimapScale);
            });
            #endregion

            #region border
            // Go to IL_0E01 (L326: Main.spriteBatch.Draw(Main.minimapFrame2Texture, ..., ..., ..., ..., ..., 1f))
            // Draw call for the black background
            /*
             *
             *   IL_0DF2: ldc.r4     0.0
             *   IL_0DF7: ldloca.s   V_49
             *   IL_0DF9: initobj    [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Vector2
             *   IL_0DFF: ldloc.s    V_49
             *   IL_0E01: ldc.r4     1
             *   
            */
            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdcR4(0),
                i => i.MatchLdloca(49),
                i => i.MatchInitobj(typeof(Vector2)),
                i => i.MatchLdloc(49),
                i => i.MatchLdcR4(1)))
                return;

            // change scale parameter to minimapScale
            c.EmitDelegate<Func<float, float>>((returnvalue) =>
            {
                return BetterZoom.minimapScale;
            });
            c.Index++;


            // Go to IL_21E9 (L683: Main.spriteBatch.Draw(Main.minimapFrameTexture, ..., ..., ..., ..., ..., ..., 1f))
            // Draw call for the minimap frame
            /*
            * IL_21DA: ldc.r4       0.0
            * IL_21DF: ldloca.s     V_49
            * IL_21E1: initobj      [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Vector2
            * IL_21E7: ldloc.s      V_49
            * IL_21E9: ldc.r4       1
            */
            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdcR4(0),
                i => i.MatchLdloca(49),
                i => i.MatchInitobj(typeof(Vector2)),
                i => i.MatchLdloc(49),
                i => i.MatchLdcR4(1)))
                return;

            c.EmitDelegate<Func<float, float>>((returnvalue) =>
            {
                return BetterZoom.minimapScale;
            });
            #endregion

            #region buttons

            // Go to IL_2201 (L686: float num88 = num57 + 148f + (float)(num87 * 26);)
            /*
                IL_2201: ldloc.s    num58 [84]
                IL_2203: ldc.r4     148
                IL_2208: add
                IL_2209: ldloc.s    num88 [121]
                IL_220B: ldc.i4.s   26
                IL_220D: mul
                IL_220E: conv.r4
                IL_220F: add
                IL_2210: stloc.s    num89 [122]
            */

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdloc(84),
                i => i.MatchLdcR4(148),
                i => i.MatchAdd(),
                i => i.MatchLdloc(121),
                i => i.MatchLdcI4(26),
                i => i.MatchMul(),
                i => i.MatchConvR4(),
                i => i.MatchAdd()))
                return;

            // Change button X position
            c.EmitDelegate<Func<float, float>>((returnvalue) =>
            {
                float newX = Main.miniMapX - 6;
                float offset = returnvalue - (newX + 148f);
                return newX + (148f * BetterZoom.minimapScale) + offset * BetterZoom.minimapScale;
            });

            // Go to IL_2212 (L687: float num89 = num58 + 234f;)
            /*
                IL_2212: ldloc.s    num59
                IL_2214: ldc.r4     234
                IL_2219: add
                IL_221A: stloc.s    num90
            */

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdloc(85),
                i => i.MatchLdcR4(234),
                i => i.MatchAdd()))
                return;

            // Change button Y position
            c.EmitDelegate<Func<float, float>>((returnvalue) =>
            {
                float newY = Main.miniMapY - 6;
                return newY + 234f * BetterZoom.minimapScale;
            });

            // Go to IL_2229 (L688: if((float)Main.mouseX < num88 + 22f)))
            /*
                IL_2229: ldsfld     int32 Terraria.Main::mouseX
                IL_222E: conv.r4
                IL_222F: ldloc.s    num89 [122]
                IL_2231: ldc.r4     22
                IL_2236: add
             */

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdsfld(typeof(Main).GetField("mouseX")),
                i => i.MatchConvR4(),
                i => i.MatchLdloc(122),
                i => i.MatchLdcR4(22)))
                return;

            // Change click hitbox X
            c.EmitDelegate<Func<float, float>>((returnvalue) =>
            {
                return returnvalue * BetterZoom.minimapScale;
            });

            // Go to (L688: if((float)Main.mouseY < num89 + 22f)))
            /*
                IL_2249: ldsfld     int32 Terraria.Main::mouseY
                IL_224E: conv.r4
                IL_224F: ldloc.s    num90 [123]
                IL_2251: ldc.r4     22
                IL_2256: add
             */
            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdsfld(typeof(Main).GetField("mouseY")),
                i => i.MatchConvR4(),
                i => i.MatchLdloc(123),
                i => i.MatchLdcR4(22)))
                return;

            // Change click hitbox Y
            c.EmitDelegate<Func<float, float>>((returnvalue) =>
            {
                return returnvalue * BetterZoom.minimapScale;
            });

            // Go to IL_229D (L690: Main.spriteBatch.Draw(Main.miniMapButtonTexture[i], ..., ..., ..., ..., ..., 1f))
            /*
                IL_229D: ldc.r4     0.0
                IL_22A2: ldloca.s   V_49
                IL_22A4: initobj    [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Vector2
                IL_22AA: ldloc.s    V_49
                IL_22AC: ldc.r4     1
             */
            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdcR4(0),
                i => i.MatchLdloca(49),
                i => i.MatchInitobj(typeof(Vector2)),
                i => i.MatchLdloc(49),
                i => i.MatchLdcR4(1)))
                return;

            // Change scale of Buttons
            c.EmitDelegate<Func<float, float>>((returnvalue) =>
            {
                return BetterZoom.minimapScale;
            });

            
            #endregion
        }
    }
}
