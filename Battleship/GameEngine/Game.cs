using System;
using System.Linq;
using System.Text;
using ConsoleGameEngineCore;
using Point = ConsoleGameEngineCore.Point;

namespace GameEngine
{
    public class Game
    {
       public static int BoardWidth { get; set; }
       public static int BoardHeight { get; set; }
       public static bool AllowAdjacentPlacement { get; set; }
       public static bool OppIsAi { get; set; }
       public static int AiDifficulty { get; set; }

       public static ConsoleEngine ConsoleEngine;
       
       public static int Phase;

       public static int ScreenWidth = 60;
       public static int ScreenHeight = 45;

       public static Player ActivePlayer;
       public static Player InactivePlayer;

       public static bool NeedToSwapTurn = false;

       public static void SwapTurns()
       {
          (ActivePlayer, InactivePlayer) = (InactivePlayer, ActivePlayer);
       }

       public static TileValue[] GetBoard()
       {
          if (ActivePlayer.IsViewingOwnBoard)
          {
             return ActivePlayer.Board;
          }
          return InactivePlayer.Board;
       }

       public Game()
       {
          
       }
       
       public Game(int boardHeight, int boardWidth, string[] ships, bool allowAdjacentPlacement, bool oppIsAi, int aiDifficulty)
       {
          Phase = 1;
          BoardHeight = boardHeight;
          BoardWidth = boardWidth;
          ActivePlayer = new Player(
             new UpdateLogic(), 
             ships.ToList(),
             Enumerable.Repeat(TileValue.EmptyTileV1, boardHeight * boardWidth).ToArray(),
             new Point(4, 4),
             "Player A");
          InactivePlayer = new Player(
             new UpdateLogic(), 
             ships.ToList(),
             Enumerable.Repeat(TileValue.EmptyTileV1, boardHeight * boardWidth).ToArray(),
             new Point(4, 4),
             "Player B");
          
          AllowAdjacentPlacement = allowAdjacentPlacement;
          OppIsAi = oppIsAi;
          AiDifficulty = aiDifficulty;
       }
       
       public void Run()
       {
          Initialize();
          DateTime startTime = DateTime.Now;
          while (true)
          {
             // if (! Helper.ApplicationIsActivated()) { continue; }
             var elapsedTime = (DateTime.Now - startTime).TotalSeconds;
             var timeCap = elapsedTime > 0.05 ? 0.05 : elapsedTime;  // 20 fps
             startTime = DateTime.Now;
             bool running = Update(timeCap);
             if (!running) { break; }
             Draw(timeCap);
          }
       }
       
       private void Initialize()
       {
          ConsoleEngine = new ConsoleEngine(ScreenWidth, ScreenHeight, 8, 8);
          Console.OutputEncoding = Encoding.Unicode;
          // https://en.wikipedia.org/wiki/Code_page_775
          //Console.SetBufferSize(ScreenWidth, ScreenHeight);
       }
       
       /// <param name="gameTime">Provides a snapshot of timing values.</param>
       private static bool Update(double gameTime)
       {
          return ActivePlayer.UpdateLogic.Update(gameTime);
       }


       /// <param name="gameTime">Provides a snapshot of timing values.</param>
       private static void Draw(double gameTime)
       {
          DrawLogic.Draw(gameTime);
       }
    }
}