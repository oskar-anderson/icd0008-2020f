using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Game;
using RogueSharp;

namespace WebApp
{
    public class WebInput : BaseC_Input, BaseI_Input
    {
        public Dictionary<ConsoleKey, bool> NewKeyDown { get; set; }
        public Dictionary<ConsoleKey, bool> KeyDown { get; set; }

        public WebInput()
        {
            this.NewKeyDown = ResetKeyboardState();
            this.KeyDown = ResetKeyboardState();
        }

        public Point GetMousePos()
        {
            return Point.Zero;
        }

        public bool GetMouseLeft()
        {
            return false;
        }

        public void UpdateKeyboardState()
        {
            // do nothing
            // updating needs to be done from page
        }
    }
}