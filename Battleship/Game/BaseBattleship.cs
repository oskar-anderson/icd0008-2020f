using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngineCore;
using Domain;
using Domain.Model;
using Domain.Tile;
using IrrKlang;
using RogueSharp;
using Point = RogueSharp.Point;

namespace Game
{
    public abstract class BaseBattleship
    {
       public readonly GameData GameData;
       public BaseI_Input Input { get; set; } = null!;
       public ISoundEngine SoundEngine { get; set; } = null!;

       public delegate void UpdateLogicExitEventDelegate();

       public UpdateLogicExitEventDelegate UpdateLogicExitEvent { get; set; } = null!;
       public UpdateLogic UpdateLogic { get; set; } = null!;

       private const int PlayerVerticalSeparator = 10;

       public GameResult Run()
       {
          Initialize();
          DateTime startTime = DateTime.Now;
          while (true)
          {
             double elapsedTime = (DateTime.Now - startTime).TotalSeconds;
             startTime = DateTime.Now;
             double timeCap = Math.Min(elapsedTime, 0.05);  // 20 fps
             bool running = Update(timeCap, this.GameData);
             if (!running) { break; }
             Draw(timeCap, this.GameData);
          }

          return Result();
       }
       
       protected BaseBattleship(GameData gameData)
       {
          GameData = gameData;
       }

       protected BaseBattleship(int boardHeight, int boardWidth, string ships, int allowAdjacentPlacement, int startingPlayerType, int secondPlayerType)
       {
          List<Point> shipList;
          string errorMsg = "";
          if (! Utils.ShipStringParse(ships, out shipList, ref errorMsg))
          {
             throw new Exception($"Unexpected! Failed to parse: {ships}! This should have been checked before! {errorMsg}");
          }
          if (ships == null) throw new ArgumentNullException(nameof(ships));
          if (ships.Length == 0) throw new Exception("No ships provided!");
          
          
          string[,] boardMap = TileFunctions.GetRndSeaTiles(boardWidth, boardHeight * 2 + PlayerVerticalSeparator);
          for (int y = boardHeight; y < boardHeight + PlayerVerticalSeparator; y++)
          {
             for (int x = 0; x < boardMap.GetWidth(); x++)
             {
                Point point = new Point(x, y);
                boardMap.Set(point, TextureValue.VoidTile);
             }
          }

          List<Sprite> sprites = new List<Sprite>();
          Player activePlayer = new Player(
             new Rectangle(0, 0, boardWidth, boardHeight),
             new Point(4,4),
             startingPlayerType,
             "Player A",
             Point.Zero,
             sprites);
          Player inactivePlayer = new Player(
             new Rectangle(0, boardHeight + PlayerVerticalSeparator, boardWidth, boardHeight),
             new Point(4,boardHeight + PlayerVerticalSeparator + 4),
             secondPlayerType,
             "Player B",
             new Point(0,(boardHeight + PlayerVerticalSeparator) * TileData.Height),
             sprites);
          
          GameData = new GameData(allowAdjacentPlacement, boardMap, shipList, activePlayer, inactivePlayer, sprites);
       }

       public GameResult Result()
       {
          return new GameResult(GameData);
       }

       public struct GameResult
       {
          public readonly bool IsOver;
          public readonly GameData Data;

          public GameResult(GameData data)
          {
             IsOver = UpdateLogic.IsOver(data, out string winner);
             Data = data;
          }
       }

       public abstract void Initialize();

       /// <param name="gameTime">Provides a snapshot of timing values.</param>
       /// <param name="data">Game data</param>
       public bool Update(double gameTime, GameData data)
       {
          return UpdateLogic.Update(gameTime, data);
       }


       /// <param name="gameTime">Provides a snapshot of timing values.</param>
       /// <param name="data">Game data</param>
       public abstract void Draw(double gameTime, GameData data);
    }
}