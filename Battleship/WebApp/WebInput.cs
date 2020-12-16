using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Game;
using RogueSharp;

namespace WebApp
{
    public class WebInput : BaseC_Input, BaseI_Input
    {
        public override Dictionary<UsedKeyKeys, KeyStatus> KeyStatuses { get; set; }

        public WebInput()
        {
            this.KeyStatuses = ResetKeyboardState();
        }

        public override Point GetMousePos()
        {
            return Point.Zero;
        }

        public override bool GetMouseLeft()
        {
            return false;
        }

        public override void UpdateKeyboardState()
        {
            // do nothing
            // updating needs to be done from page
        }
    }
}