using ConsoleApp.GameMenu;
using static System.Environment;
using GameEngine;

namespace ConsoleApp
{
    internal static class Program
    {
        private static void Main()
        {
            RuleSet ruleSet = (RuleSet) Menu.Start();
            if (ruleSet.ExitCode == "StartGame")
            {
                var game = new BattleshipBrain(ruleSet.BoardHeight, ruleSet.BoardWidth, ruleSet.AdjacentTilesAllowed,
                    false, -1);
            }

            Exit(1);
        }
    }
}