
using System;
using System.Collections.Generic;

namespace Game
{
    public interface BaseI_Input
    {
        Dictionary<ConsoleKey, bool> NewKeyDown { get; set; }
        Dictionary<ConsoleKey, bool> KeyDown { get; set; }
        RogueSharp.Point GetMousePos();
        bool GetMouseLeft();
        void UpdateKeyboardState();
    }

    public class BaseC_Input
    {
        public static Dictionary<ConsoleKey, bool> ResetKeyboardState()
        {
            return SetKeyboardState(
                new UsedKeys(
                    false, false, false, false, false, false, false, false, 
                    false, false, false, false, 
                    false, false, false, false, 
                    false, false, false, false,
                    false, false
                    )
                );
        }

        public static Dictionary<ConsoleKey, bool> SetKeyboardState(
            bool r, bool x, bool esc, bool c, bool z, bool d1, bool d2, bool d3, 
            bool a, bool s, bool d, bool w, 
            bool leftArrow, bool downArrow, bool rightArrow, bool upArrow, 
            bool j, bool k, bool l, bool i,
            bool oemMinus, bool oemPlus)
        {
            return SetKeyboardState(
                new UsedKeys(
                    r, x, esc, c, z, d1, d2, d3,
                    a, s, d, w,
                    leftArrow, downArrow, rightArrow, upArrow,
                    j, k, l, i,
                    oemMinus, oemPlus
                    )
                );
        }
        public static Dictionary<ConsoleKey, bool> SetKeyboardState(UsedKeys uk)
        {
            Dictionary<ConsoleKey, bool> currentTurnKeyDownResult = new Dictionary<ConsoleKey, bool>
            {
                [ConsoleKey.R] = uk.r,
                [ConsoleKey.X] = uk.x,
                [ConsoleKey.Escape] = uk.esc,
                [ConsoleKey.C] = uk.c,
                [ConsoleKey.Z] = uk.z,
                [ConsoleKey.D1] = uk.d1,
                [ConsoleKey.D2] = uk.d2,
                [ConsoleKey.D3] = uk.d3,
                
                
                [ConsoleKey.A] = uk.a,
                [ConsoleKey.S] = uk.s,
                [ConsoleKey.D] = uk.d,
                [ConsoleKey.W] = uk.w,
                
                [ConsoleKey.LeftArrow] = uk.leftArrow,
                [ConsoleKey.DownArrow] = uk.downArrow,
                [ConsoleKey.RightArrow] = uk.rightArrow,
                [ConsoleKey.UpArrow] = uk.upArrow,
                
                [ConsoleKey.J] = uk.j,
                [ConsoleKey.K] = uk.k,
                [ConsoleKey.L] = uk.l,
                [ConsoleKey.I] = uk.i,
                
                [ConsoleKey.OemMinus] = uk.oemMinus,
                [ConsoleKey.OemPlus] = uk.oemPlus,
            };
            return currentTurnKeyDownResult;
        }
    }

    public struct UsedKeys
    {
        public readonly bool r;
        public readonly bool x;
        public readonly bool esc;
        public readonly bool c;
        public readonly bool z;
        public readonly bool d1;
        public readonly bool d2;
        public readonly bool d3;
        
        public readonly bool a;
        public readonly bool s;
        public readonly bool d;
        public readonly bool w;
        public readonly bool leftArrow;
        public readonly bool downArrow;
        public readonly bool rightArrow;
        public readonly bool upArrow;
        public readonly bool j;
        public readonly bool k;
        public readonly bool l;
        public readonly bool i;
        
        public readonly bool oemMinus;
        public readonly bool oemPlus;

        public UsedKeys(
            bool r, bool x, bool esc, bool c, bool z, bool d1, bool d2, bool d3, 
            bool a, bool s, bool d, bool w, 
            bool leftArrow, bool downArrow, bool rightArrow, bool upArrow, 
            bool j, bool k, bool l, bool i,
            bool oemMinus, bool oemPlus)
        {
            this.r = r;
            this.x = x;
            this.esc = esc;
            this.c = c;
            this.z = z;
            this.d1 = d1;
            this.d2 = d2;
            this.d3 = d3;

            this.a = a;
            this.s = s;
            this.d = d;
            this.w = w;
            
            this.leftArrow = leftArrow;
            this.downArrow = downArrow;
            this.rightArrow = rightArrow;
            this.upArrow = upArrow;
            
            this.j = j;
            this.k = k;
            this.l = l;
            this.i = i;

            this.oemMinus = oemMinus;
            this.oemPlus = oemPlus;
        }
    }
}