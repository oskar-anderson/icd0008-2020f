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
        public Dictionary<ConsoleKey, bool> KeyDown { get; set; }
        public Dictionary<ConsoleKey, bool> NewKeyDown { get; set; }
        private Dictionary<ConsoleKey, bool> currentTurnKeyDown;
        private readonly ConsoleEngine engine;
        public ConsoleInput(ConsoleEngine engine)
        {
            this.engine = engine;
            this.currentTurnKeyDown = ResetKeyboardState();
            this.NewKeyDown = ResetKeyboardState();
            this.KeyDown = ResetKeyboardState();
        }
        
        public void UpdateKeyboardState()
        {
            Dictionary<ConsoleKey, bool> lastTurnKeyDown = currentTurnKeyDown.ToDictionary(entry => entry.Key, entry => entry.Value);
            currentTurnKeyDown = SetKeyboardState(
                new UsedKeys(
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
                    GetKey(ConsoleKey.OemPlus)
                    
                ));
            NewKeyDown = new Dictionary<ConsoleKey, bool>();
            KeyDown = new Dictionary<ConsoleKey, bool>();
            foreach (var key in lastTurnKeyDown.Keys)
            {
                NewKeyDown[key] = currentTurnKeyDown[key] && !lastTurnKeyDown[key];
                KeyDown[key] = currentTurnKeyDown[key];
            }
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
        
        public RogueSharp.Point GetMousePos()
        {
            ConsoleGameEngineCore.Point p = engine.GetMousePos();
            return new RogueSharp.Point(p.X, p.Y);
        }

        public bool GetMouseLeft()
        {
            return engine.GetMouseLeft();
        }
    }
}