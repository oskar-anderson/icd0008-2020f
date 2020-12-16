using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ConsoleGameEngineCore;
using Game;

namespace ConsoleApp
{
    public class ConsoleInput : BaseC_Input, BaseI_Input
    {
        public override Dictionary<UsedKeyKeys, KeyStatus> KeyStatuses { get; set; }

        private readonly ConsoleEngine engine;
        public ConsoleInput(ConsoleEngine engine)
        {
            this.engine = engine;
            this.KeyStatuses = ResetKeyboardState();
        }
        
        public override void UpdateKeyboardState()
        {
            Dictionary<UsedKeyKeys, KeyStatus> lastTurnKeyDown = KeyStatuses.ToDictionary(entry => entry.Key, entry => entry.Value);
            UsedKeyValues isDown = new UsedKeyValues(
                GetKey(ConsoleKey.R),
                GetKey(ConsoleKey.X),
                GetKey(ConsoleKey.Escape),
                GetKey(ConsoleKey.C),
                GetKey(ConsoleKey.Z),
                GetKey(ConsoleKey.D1),
                GetKey(ConsoleKey.D2),
                GetKey(ConsoleKey.D3),
                
                GetKey(ConsoleKey.A),
                GetKey(ConsoleKey.S),
                GetKey(ConsoleKey.D),
                GetKey(ConsoleKey.W),
                
                GetKey(ConsoleKey.LeftArrow),
                GetKey(ConsoleKey.DownArrow),
                GetKey(ConsoleKey.RightArrow),
                GetKey(ConsoleKey.UpArrow),
                
                GetKey(ConsoleKey.J),
                GetKey(ConsoleKey.K),
                GetKey(ConsoleKey.L),
                GetKey(ConsoleKey.I),
                
                GetKey(ConsoleKey.OemMinus),
                GetKey(ConsoleKey.OemPlus),
                
                GetMouseLeft()
            );

            
            KeyStatuses = SetKeyboardState(
                new UsedKeyValues(
                    isDown.R && !lastTurnKeyDown[UsedKeyKeys.R].IsDown,
                    isDown.X && !lastTurnKeyDown[UsedKeyKeys.X].IsDown,
                    isDown.Escape && !lastTurnKeyDown[UsedKeyKeys.Escape].IsDown,
                    isDown.C && !lastTurnKeyDown[UsedKeyKeys.C].IsDown,
                    isDown.Z && !lastTurnKeyDown[UsedKeyKeys.Z].IsDown,
                    isDown.D1 && !lastTurnKeyDown[UsedKeyKeys.D1].IsDown,
                    isDown.D2 && !lastTurnKeyDown[UsedKeyKeys.D2].IsDown,
                    isDown.D3 && !lastTurnKeyDown[UsedKeyKeys.D3].IsDown,
                            
                    isDown.A && !lastTurnKeyDown[UsedKeyKeys.A].IsDown,
                    isDown.S && !lastTurnKeyDown[UsedKeyKeys.S].IsDown,
                    isDown.D && !lastTurnKeyDown[UsedKeyKeys.D].IsDown,
                    isDown.W && !lastTurnKeyDown[UsedKeyKeys.W].IsDown,
                            
                    isDown.LeftArrow && !lastTurnKeyDown[UsedKeyKeys.LeftArrow].IsDown,
                    isDown.DownArrow && !lastTurnKeyDown[UsedKeyKeys.DownArrow].IsDown,
                    isDown.RightArrow && !lastTurnKeyDown[UsedKeyKeys.RightArrow].IsDown,
                    isDown.UpArrow && !lastTurnKeyDown[UsedKeyKeys.UpArrow].IsDown,
                            
                    isDown.J && !lastTurnKeyDown[UsedKeyKeys.J].IsDown,
                    isDown.K && !lastTurnKeyDown[UsedKeyKeys.K].IsDown,
                    isDown.L && !lastTurnKeyDown[UsedKeyKeys.L].IsDown,
                    isDown.I && !lastTurnKeyDown[UsedKeyKeys.I].IsDown,
                            
                    isDown.OemMinus && !lastTurnKeyDown[UsedKeyKeys.OemMinus].IsDown,
                    isDown.OemPlus && !lastTurnKeyDown[UsedKeyKeys.OemPlus].IsDown,
                            
                    isDown.MouseLeft && !lastTurnKeyDown[UsedKeyKeys.MouseLeft].IsDown
                    ), 
                new UsedKeyValues(
                    isDown.R,
                    isDown.X,
                    isDown.Escape,
                    isDown.C,
                    isDown.Z,
                    isDown.D1,
                    isDown.D2,
                    isDown.D3,
                    
                    isDown.A,
                    isDown.S,
                    isDown.D,
                    isDown.W,
                    
                    isDown.LeftArrow,
                    isDown.DownArrow,
                    isDown.RightArrow,
                    isDown.UpArrow,
                    
                    isDown.J,
                    isDown.K,
                    isDown.L,
                    isDown.I,
                    
                    isDown.OemMinus,
                    isDown.OemPlus,
                    
                    isDown.MouseLeft
                    )
                );
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern short GetAsyncKeyState(int vKey);

        private bool GetKey(ConsoleKey key)
        {
            if (false)
            {
                return engine.GetKey((ConsoleKey) (int) key);
            }
            short s = GetAsyncKeyState((int) key);
            return (s & 0x8000) > 0;
        }
        
        public override RogueSharp.Point GetMousePos()
        {
            ConsoleGameEngineCore.Point p = engine.GetMousePos();
            return new RogueSharp.Point(p.X, p.Y);
        }

        public override bool GetMouseLeft()
        {
            return engine.GetMouseLeft();
        }
    }
}