using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp.GameMenu;
using DAL;
using Game;
using Microsoft.EntityFrameworkCore;
using Point = RogueSharp.Point;

namespace ConsoleApp
{
    internal static class Program
    {
        private static void Main()
        {
            List<Menu.MenuAction> menuTree = new List<Menu.MenuAction>() { Menu.getMenuMain() };
            while (true)
            {
                Console.CursorVisible = false;
                bool isContinueActive = DbQueries.SavesAmount() != 0;
                RuleSet ruleSet = Menu.Start(menuTree, isContinueActive);
                
                GameMain game;
                switch (ruleSet.ExitCode)
                {
                    case ExitResult.Start:
                        game = new GameMain(ruleSet.BoardHeight, ruleSet.BoardWidth, ruleSet.Ships, ruleSet.AllowedPlacementType, -1, -1);
                        break;
                    case ExitResult.Continue:
                        DbQueries.TryGetGameWithIdx(0, ref GameMain.GameData);
                        game = new GameMain();
                        break;
                    case ExitResult.Exit:
                        return;
                    default:
                        throw new Exception("unexpected");
                }

                bool gameOver = game.Run();
                if (gameOver)
                {
                    return;
                }
                
                PauseMenu.PauseResult result;
                while (true)
                {
                    Console.CursorVisible = true;
                    result = PauseMenu.Run();
                    if (result == PauseMenu.PauseResult.LoadDb)
                    {
                        DbQueries.TryGetGameWithIdx(0, ref GameMain.GameData);
                    }
                    if (result == PauseMenu.PauseResult.LoadJson)
                    {
                        Game.Model.GameData? data;
                        bool isGood = DataManager.LoadGameAction(out data);
                        if (isGood)
                        {
                            if (data == null) { throw new Exception("unexpected"); }
                            GameMain.GameData = data == null ? GameMain.GameData : data;
                        }
                    }
                    if (result == PauseMenu.PauseResult.LoadDb 
                        || result == PauseMenu.PauseResult.LoadJson
                        || result == PauseMenu.PauseResult.Cont)
                    {
                        Console.CursorVisible = false;
                        gameOver = new GameMain().Run();
                        if (gameOver)
                        {
                            return;
                        }
                    }
                    else { break; }
                };
                switch (result)
                {
                    case PauseMenu.PauseResult.SaveDb:
                        DbQueries.SaveMainDb(GameMain.GameData);
                        break;
                    case PauseMenu.PauseResult.SaveJson:
                        DataManager.SaveGameAction(GameMain.GameData);
                        break;
                    case PauseMenu.PauseResult.MainMenu:
                        menuTree = new List<Menu.MenuAction>() { Menu.getMenuMain() };
                        continue;
                }
                if (result != PauseMenu.PauseResult.SaveDb &&
                    result != PauseMenu.PauseResult.SaveJson &&
                    result != PauseMenu.PauseResult.MainMenu)
                {
                    return;
                }
            }
        }
    }
}