using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngineCore;
using Domain.Model;
using Game;
using IrrKlang;
using RogueSharp;
using Point = RogueSharp.Point;

namespace ConsoleApp
{
    public class ConsoleBattle : BaseBattleship
    {
       public static ConsoleEngine ConsoleEngine = null!;
       

       public const int ScreenWidth = 60;
       public const int ScreenHeight = 45;
       private const int FontW = 8;
       private const int FontH = 8;

       

       public ConsoleBattle(GameData gameData) : base(gameData)
       {
          
       }

       public ConsoleBattle(int boardHeight, int boardWidth, string ships, int allowAdjacentPlacement, int startingPlayerType, int secondPlayerType)
          : base(boardHeight, boardWidth, ships, allowAdjacentPlacement, startingPlayerType, secondPlayerType)
       {
          
       }
       
       public override void Initialize()
       {
          ConsoleEngine = new ConsoleEngine(ScreenWidth, ScreenHeight, FontW, FontH);
          Console.OutputEncoding = Encoding.Unicode;
          const SoundEngineOptionFlag options = 
             SoundEngineOptionFlag.Use3DBuffers | 
             SoundEngineOptionFlag.MultiThreaded | 
             // SoundEngineOptionFlag.PrintDebugInfoIntoDebugger |
             // SoundEngineOptionFlag.PrintDebugInfoToStdOut | 
             SoundEngineOptionFlag.LoadPlugins;
          SoundEngine = new ISoundEngine(SoundOutputDriver.AutoDetect, options);
          Input = new ConsoleInput(ConsoleEngine);
          UpdateExitActionStrategy = Helper.FixConsole;
          UpdateLogic = new UpdateLogic(UpdateExitActionStrategy, Input, SoundEngine);
       }

       /// <param name="gameTime">Provides a snapshot of timing values.</param>
       /// <param name="gameData">Game data</param>
       /// <param name="drawLogicData">Properties like error messages and available dialog options used in drawing functions</param>
       public override void Draw(double gameTime, GameData gameData, DrawLogicData drawLogicData)
       {
          ConsoleDrawLogic.Draw(gameTime, gameData, drawLogicData);
       }
    }
}