using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp.GameMenu;
using DAL;
using Domain;
using Domain.Model;
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
                bool isContinueActive;
                try
                {
                    isContinueActive = DbQueries.SavesAmount() != 0;
                }
                catch
                {
                    isContinueActive = false;
                }

                RuleSet ruleSet = Menu.Start(menuTree, isContinueActive);
                
                ConsoleBattle game;
                switch (ruleSet.ExitCode)
                {
                    case ExitResult.Start:
                        game = new ConsoleBattle(ruleSet.BoardHeight, ruleSet.BoardWidth, ruleSet.Ships, ruleSet.AllowedPlacementType, -1, -1);
                        break;
                    case ExitResult.Continue:
                        DbQueries.TryGetGameWithIdx(0, out GameData? gameDataTemp);
                        if (gameDataTemp == null) { throw new Exception("unexpected!");}
                        game = new ConsoleBattle(gameDataTemp);
                        break;
                    case ExitResult.Exit:
                        return;
                    default:
                        throw new Exception("unexpected");
                }

                BaseBattleship.GameResult gameData = game.Run();
                if (gameData.IsOver)
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
                        DbQueries.TryGetGameWithIdx(0, out GameData? gameDataTemp);
                        if (gameDataTemp == null) { throw new Exception("unexpected!");}
                        game = new ConsoleBattle(gameDataTemp);
                    }
                    if (result == PauseMenu.PauseResult.LoadJson)
                    {
                        bool isGood = DataManager.LoadGameAction(out GameData? gameDataTemp);
                        if (isGood)
                        {
                            if (gameDataTemp == null) { throw new Exception("unexpected"); }
                            game = new ConsoleBattle(gameDataTemp);
                        }
                    }
                    if (result == PauseMenu.PauseResult.Cont)
                    {
                        game = new ConsoleBattle(gameData.Data);
                    }
                    if (result == PauseMenu.PauseResult.LoadDb 
                        || result == PauseMenu.PauseResult.LoadJson
                        || result == PauseMenu.PauseResult.Cont)
                    {
                        Console.CursorVisible = false;
                        gameData = game.Run();
                        if (gameData.IsOver)
                        {
                            return;
                        }
                    }
                    else { break; }
                }
                switch (result)
                {
                    case PauseMenu.PauseResult.SaveDb:
                        DbQueries.SaveAndDeleteOthers(gameData.Data);
                        break;
                    case PauseMenu.PauseResult.SaveJson:
                        DataManager.SaveGameAction(gameData.Data);
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