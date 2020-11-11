using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleGameEngineCore;
using Game.Model;
using Game.Tile;
using IrrKlang;
using RogueSharp;
using Point = RogueSharp.Point;

namespace Game
{
    public class GameMain
    {
       public static ConsoleEngine ConsoleEngine = null!;
       public static ISoundEngine SoundEngine = null!;

       public const int ScreenWidth = 60;
       public const int ScreenHeight = 45;

       
       public static GameData GameData = null!;

       public static int[,] GetBoard()
       {
          int[,] board = GameData.ActivePlayer.IsViewingOwnBoard
             ? GameData.ActivePlayer.Board
             : GameData.InactivePlayer.Board;
          return board;
       }

       public GameMain()
       {
          if (GameData == null)
          {
             throw new Exception("Wrong constructor!");
          }
       }
       
       public GameMain(int boardHeight, int boardWidth, List<Point> ships, int allowAdjacentPlacement, int startingPlayerType, int secondPlayerType)
       {
          if (ships == null) throw new ArgumentNullException(nameof(ships));
          
          var activePlayerBoard = TileFunctions.GetRndSeaTiles(boardWidth, boardHeight);
          var inactivePlayerBoard = TileFunctions.GetRndSeaTiles(boardWidth, boardHeight);
          
          Player activePlayer = new Player(
             new List<Rectangle>(ships.Count),
             activePlayerBoard,
             new Point(4,4),
             true,
             true,
             startingPlayerType,
             "Player A");
          Player inactivePlayer = new Player(
             new List<Rectangle>(ships.Count),
             inactivePlayerBoard,
             new Point(4,4),
             true,
             true,
             secondPlayerType,
             "Player B");
          
          GameData = new GameData(boardWidth, boardHeight, allowAdjacentPlacement, new List<Point>(ships), 1, activePlayer, inactivePlayer);
       }
       
       public bool Run()
       {
          Initialize();
          DateTime startTime = DateTime.Now;
          while (true)
          {
             // if (! Helper.ApplicationIsActivated()) { continue; }
             double elapsedTime = (DateTime.Now - startTime).TotalSeconds;
             startTime = DateTime.Now;
             double timeCap = elapsedTime > 0.05 ? 0.05 : elapsedTime;  // 20 fps
             bool running = Update(timeCap);
             if (!running) { break; }
             Draw(timeCap);
          }
          return GameData.WinningPlayer != null;
       }
       
       private static void Initialize()
       {
          ConsoleEngine = new ConsoleEngine(ScreenWidth, ScreenHeight, 8, 8);
          Console.OutputEncoding = Encoding.Unicode;
          const SoundEngineOptionFlag options = 
             SoundEngineOptionFlag.Use3DBuffers | 
             SoundEngineOptionFlag.MultiThreaded | 
             // SoundEngineOptionFlag.PrintDebugInfoIntoDebugger |
             // SoundEngineOptionFlag.PrintDebugInfoToStdOut | 
            SoundEngineOptionFlag.LoadPlugins;
          SoundEngine = new ISoundEngine(SoundOutputDriver.AutoDetect, options);
       }
       
       /// <param name="gameTime">Provides a snapshot of timing values.</param>
       private static bool Update(double gameTime)
       {
          return UpdateLogic.Update(gameTime);
       }


       /// <param name="gameTime">Provides a snapshot of timing values.</param>
       private static void Draw(double gameTime)
       {
          DrawLogic.Draw(gameTime);
       }
    }
}