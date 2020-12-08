using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngineCore;
using Domain.Model;
using Game.Tile;
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

       public delegate void UpdateExitAction();

       public UpdateExitAction UpdateExitActionStrategy { get; set; } = null!;
       public UpdateLogic UpdateLogic { get; set; } = null!;

       public static int[,] GetBoard(GameData gameData)
       {
          int[,] board = gameData.ActivePlayer.IsViewingOwnBoard
             ? gameData.ActivePlayer.Board
             : gameData.InactivePlayer.Board;
          return board;
       }
       
       public GameResult Run()
       {
          Initialize();
          DateTime startTime = DateTime.Now;
          while (true)
          {
             double elapsedTime = (DateTime.Now - startTime).TotalSeconds;
             startTime = DateTime.Now;
             double timeCap = elapsedTime > 0.05 ? 0.05 : elapsedTime;  // 20 fps
             bool running = Update(timeCap, this.GameData, out var baseDrawLogic);
             if (!running) { break; }
             Draw(timeCap, this.GameData, baseDrawLogic);
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
          
          var activePlayerBoard = TileFunctions.GetRndSeaTiles(boardWidth, boardHeight);
          var inactivePlayerBoard = TileFunctions.GetRndSeaTiles(boardWidth, boardHeight);
          
          Player activePlayer = new Player(
             new List<Rectangle>(shipList.Count),
             activePlayerBoard,
             new Point(4,4),
             true,
             true,
             startingPlayerType,
             "Player A");
          Player inactivePlayer = new Player(
             new List<Rectangle>(shipList.Count),
             inactivePlayerBoard,
             new Point(4,4),
             true,
             true,
             secondPlayerType,
             "Player B");
          
          GameData = new GameData(allowAdjacentPlacement, shipList, 1, activePlayer, inactivePlayer);
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
             IsOver = data.WinningPlayer != null;
             Data = data;
          }
       }

       public abstract void Initialize();

       /// <param name="gameTime">Provides a snapshot of timing values.</param>
       /// <param name="data">Game data</param>
       /// <param name="drawLogicData">Properties like error messages and available dialog options used in drawing functions</param>
       public bool Update(double gameTime, GameData data, out DrawLogicData drawLogicData)
       {
          return UpdateLogic.Update(gameTime, data, out drawLogicData);
       }


       /// <param name="gameTime">Provides a snapshot of timing values.</param>
       /// <param name="data">Game data</param>
       /// <param name="drawLogicData">Properties like error messages and available dialog options used in drawing functions</param>
       public abstract void Draw(double gameTime, GameData data, DrawLogicData drawLogicData);
    }
}