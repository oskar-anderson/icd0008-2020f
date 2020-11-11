using System;
using System.Collections.Generic;

namespace ConsoleApp.GameMenu
{
    internal class MenuResult
    {
        internal ExitResult ExitCode { get; set; }
    }
    internal class RuleSet : MenuResult
    {
        internal int BoardHeight { get; set; }
        internal int BoardWidth { get; set; }
        internal int AllowedPlacementType { get; set; }
        internal List<RogueSharp.Point> Ships { get; set; } = null!;
    }

    public enum ExitResult
    {
        Start = 1,
        Continue = 2,
        Exit = 4,
    }
}