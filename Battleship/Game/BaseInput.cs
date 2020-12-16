
using System;
using System.Collections.Generic;
using RogueSharp;

namespace Game
{
    public interface BaseI_Input
    {
        Dictionary<UsedKeyKeys, KeyStatus> KeyStatuses { get; set; }
        RogueSharp.Point GetMousePos();
        bool GetMouseLeft();
        void UpdateKeyboardState();
    }

    public class KeyStatus
    {
        public readonly bool IsPressed;
        public readonly bool IsDown;
        public KeyStatus(bool isPressed, bool isDown)
        {
            this.IsPressed = isPressed;
            this.IsDown = isDown;
        }

        public override string ToString()
        {
            return $"{nameof(IsPressed)}:{IsPressed}; " +
                   $"{nameof(IsDown)}:{IsDown};";
        }
    }
    
    public abstract class BaseC_Input : BaseI_Input
    {
        public static Dictionary<UsedKeyKeys, KeyStatus> ResetKeyboardState()
        {
            return SetKeyboardState(
                new UsedKeyValues(), 
                new UsedKeyValues()
                );
        }


        public static Dictionary<UsedKeyKeys, KeyStatus> SetKeyboardState(
            UsedKeyValues isPressed, UsedKeyValues isDown)
        {
            Dictionary<UsedKeyKeys, KeyStatus> currentTurnKeyDownResult = new Dictionary<UsedKeyKeys, KeyStatus>
            {
                [UsedKeyKeys.R] = new KeyStatus(isPressed.R, isDown.R),
                [UsedKeyKeys.X] = new KeyStatus(isPressed.X, isDown.X),
                [UsedKeyKeys.Escape] = new KeyStatus(isPressed.Escape, isDown.Escape),
                [UsedKeyKeys.C] = new KeyStatus(isPressed.C, isDown.C),
                [UsedKeyKeys.Z] = new KeyStatus(isPressed.Z, isDown.Z),
                [UsedKeyKeys.D1] = new KeyStatus(isPressed.D1, isDown.D1),
                [UsedKeyKeys.D2] = new KeyStatus(isPressed.D2, isDown.D2),
                [UsedKeyKeys.D3] = new KeyStatus(isPressed.D3, isDown.D3),
                
                [UsedKeyKeys.A] = new KeyStatus(isPressed.A, isDown.A),
                [UsedKeyKeys.S] = new KeyStatus(isPressed.S, isDown.S),
                [UsedKeyKeys.D] = new KeyStatus(isPressed.D, isDown.D),
                [UsedKeyKeys.W] = new KeyStatus(isPressed.W, isDown.W),
                
                [UsedKeyKeys.LeftArrow] = new KeyStatus(isPressed.LeftArrow, isDown.LeftArrow),
                [UsedKeyKeys.DownArrow] = new KeyStatus(isPressed.DownArrow, isDown.DownArrow),
                [UsedKeyKeys.RightArrow] = new KeyStatus(isPressed.RightArrow, isDown.RightArrow),
                [UsedKeyKeys.UpArrow] = new KeyStatus(isPressed.UpArrow, isDown.UpArrow),
                
                [UsedKeyKeys.J] = new KeyStatus(isPressed.J, isDown.J),
                [UsedKeyKeys.K] = new KeyStatus(isPressed.K, isDown.K),
                [UsedKeyKeys.L] = new KeyStatus(isPressed.L, isDown.L),
                [UsedKeyKeys.I] = new KeyStatus(isPressed.I, isDown.I),
                
                [UsedKeyKeys.OemMinus] = new KeyStatus(isPressed.OemMinus, isDown.OemMinus),
                [UsedKeyKeys.OemPlus] = new KeyStatus(isPressed.OemPlus, isDown.OemPlus),
                
                [UsedKeyKeys.MouseLeft] = new KeyStatus(isPressed.MouseLeft, isDown.MouseLeft),
            };
            return currentTurnKeyDownResult;
        }
        public abstract Dictionary<UsedKeyKeys, KeyStatus> KeyStatuses { get; set; }
        public abstract Point GetMousePos();
        public abstract bool GetMouseLeft();
        public abstract void UpdateKeyboardState();
    }
    
    public class UsedKeyKeys
    {
        // https://stackoverflow.com/questions/757684/enum-inheritance/4042826#4042826
        
        public static readonly UsedKeyKeys R = new UsedKeyKeys("R");
        public static readonly UsedKeyKeys X = new UsedKeyKeys("X");
        public static readonly UsedKeyKeys Escape = new UsedKeyKeys("Escape");
        public static readonly UsedKeyKeys C = new UsedKeyKeys("C");
        public static readonly UsedKeyKeys Z = new UsedKeyKeys("Z");
        
        public static readonly UsedKeyKeys D1 = new UsedKeyKeys("D1");
        public static readonly UsedKeyKeys D2 = new UsedKeyKeys("D2");
        public static readonly UsedKeyKeys D3 = new UsedKeyKeys("D3");
        
        public static readonly UsedKeyKeys A = new UsedKeyKeys("A");
        public static readonly UsedKeyKeys S = new UsedKeyKeys("S");
        public static readonly UsedKeyKeys D = new UsedKeyKeys("D");
        public static readonly UsedKeyKeys W = new UsedKeyKeys("W");
        
        public static readonly UsedKeyKeys LeftArrow = new UsedKeyKeys("LeftArrow");
        public static readonly UsedKeyKeys DownArrow = new UsedKeyKeys("DownArrow");
        public static readonly UsedKeyKeys RightArrow = new UsedKeyKeys("RightArrow");
        public static readonly UsedKeyKeys UpArrow = new UsedKeyKeys("UpArrow");
        
        public static readonly UsedKeyKeys J = new UsedKeyKeys("J");
        public static readonly UsedKeyKeys K = new UsedKeyKeys("K");
        public static readonly UsedKeyKeys L = new UsedKeyKeys("L");
        public static readonly UsedKeyKeys I = new UsedKeyKeys("I");
        
        public static readonly UsedKeyKeys OemMinus = new UsedKeyKeys("OemMinus");
        public static readonly UsedKeyKeys OemPlus = new UsedKeyKeys("OemPlus");
        
        public static readonly UsedKeyKeys MouseLeft = new UsedKeyKeys("MouseLeft");

        private readonly string name;
        private UsedKeyKeys(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return this.name;
        }
    }
    
    public struct UsedKeyValues
    {
        public readonly bool R;
        public readonly bool X;
        public readonly bool Escape;
        public readonly bool C;
        public readonly bool Z;
        public readonly bool D1;
        public readonly bool D2;
        public readonly bool D3;
        
        public readonly bool A;
        public readonly bool S;
        public readonly bool D;
        public readonly bool W;
        public readonly bool LeftArrow;
        public readonly bool DownArrow;
        public readonly bool RightArrow;
        public readonly bool UpArrow;
        public readonly bool J;
        public readonly bool K;
        public readonly bool L;
        public readonly bool I;
        
        public readonly bool OemMinus;
        public readonly bool OemPlus;
        
        public readonly bool MouseLeft;

        public UsedKeyValues(
            bool r, bool x, bool escape, bool c, bool z, bool d1, bool d2, bool d3, 
            bool a, bool s, bool d, bool w, 
            bool leftArrow, bool downArrow, bool rightArrow, bool upArrow, 
            bool j, bool k, bool l, bool i,
            bool oemMinus, bool oemPlus, bool mouseLeft)
        {
            this.R = r;
            this.X = x;
            this.Escape = escape;
            this.C = c;
            this.Z = z;
            this.D1 = d1;
            this.D2 = d2;
            this.D3 = d3;

            this.A = a;
            this.S = s;
            this.D = d;
            this.W = w;
            
            this.LeftArrow = leftArrow;
            this.DownArrow = downArrow;
            this.RightArrow = rightArrow;
            this.UpArrow = upArrow;
            
            this.J = j;
            this.K = k;
            this.L = l;
            this.I = i;

            this.OemMinus = oemMinus;
            this.OemPlus = oemPlus;

            this.MouseLeft = mouseLeft;
        }
    }
}