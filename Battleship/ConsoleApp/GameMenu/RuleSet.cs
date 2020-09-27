namespace ConsoleApp.GameMenu
{
    internal class MenuResult
    {
        internal string? ExitCode { get; set; }
    }
    internal class RuleSet : MenuResult
    {
        internal int BoardHeight { get; set; }
        internal int BoardWidth { get; set; }
        internal bool AdjacentTilesAllowed { get; set; }
    }
}