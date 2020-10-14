using System;
using System.Collections.Generic;
using ConsoleApp.GameMenu;
using GameEngine;

namespace ConsoleApp
{
    internal static class Program
    {
        private static void Main()
        {
            Console.CursorVisible = false;
            RuleSet ruleSet = (RuleSet) Menu.Start();
            if (ruleSet.ExitCode == "StartGame")
            {
                string[] ships = new[] {"xxxxx", "xxxx", "xxxx", "xxx", "xxx", "xxx", "xx", "xx", "xx", "xx"};
                var game = new Game(ruleSet.BoardHeight, ruleSet.BoardWidth, ships, ruleSet.AdjacentTilesAllowed,
                    false, -1);
                game.Run();

                while (true)
                {
                    var menuTree = new List<Menu.MenuAction>() {Menu.getMenuMain(), Menu.getMenuStart()};
                    Menu.Start(menuTree);
                    Console.CursorVisible = true;
                    string result = QuickMenu();
                    if (result == "1")
                    {
                        DataManager.LoadGameAction();
                        new Game().Run();
                    } 
                    else if (result == "2")
                    {
                        GameDTO gameState = new GameDTO().Create();
                        DataManager.SaveGameAction(gameState);
                    }
                    else if (result == "3")
                    {
                        Console.CursorVisible = false;
                        new Game().Run();

                    }
                    else if (result == "4")
                    {
                        return;
                    }
                }
            }
        }

        private static string QuickMenu()
        {
            Console.WriteLine("1. LoadGameAction");
            Console.WriteLine("2. SaveGameAction");
            Console.WriteLine("3. Continue");
            Console.WriteLine("4. Quit");
            while (true)
            {
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        return "1";
                    case "2":
                        return "2";
                    case "3":
                        return "3";
                    case "4":
                        return "4";
                    default:
                        Console.WriteLine($"Unknown command: {input}");
                        continue;
                }
            }
        }
    }
}