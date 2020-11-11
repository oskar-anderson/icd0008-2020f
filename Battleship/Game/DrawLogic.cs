using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ConsoleGameEngineCore;
using Game.Model;
using Game.Tile;
using Point = ConsoleGameEngineCore.Point;

namespace Game
{
    public static class DrawLogic
    {
       private static readonly Point BoardOffset = new Point(0, 5);

       public static string Message = "";
       public static readonly List<string> DialogOptions = new List<string>();

       private static int _frameCount = 1;
       
       /// <summary>
       /// This is called when the game should draw itself.
       /// </summary>
       /// <param name="gameTime">Provides a snapshot of timing values.</param>
       public static void Draw(double gameTime)
       {
          GameMain.ConsoleEngine.ClearBuffer();

          // Draw transformed elements
          double dFps = 1.0d / gameTime;
          string sFps = Math.Floor(dFps).ToString(CultureInfo.InvariantCulture);
          Console.Title = "BattleShip FPS: " + sFps;

          float fWorldLeft, fWorldTop, fWorldRight, fWorldbottom;
          UpdateLogic.ScreenToWorld(0, 0, out fWorldLeft, out fWorldTop);
          UpdateLogic.ScreenToWorld(GameMain.ScreenWidth, GameMain.ScreenHeight, out fWorldRight, out fWorldbottom);
          
          int tilesDrawn = 0;

          for (int y = 0; y < GameMain.GameData.BoardHeight; y++)
          {
             for (int x = 0; x < GameMain.GameData.BoardWidth; x++)
             {
                TileData.CharInfo[] tile = TileFunctions.GetTile(GameMain.GetBoard(), new RogueSharp.Point(x, y), GameMain.GameData.Phase);
                
                bool isGood = DrawToBoard(new RogueSharp.Point(x, y), tile, BoardOffset, fWorldRight, fWorldbottom, fWorldTop, fWorldLeft);

                if (isGood) { tilesDrawn++;}
             }
          }

          foreach (var hoverTile in GameMain.GameData.ActivePlayer.BoardHover)
          {
             TileData.CharInfo[] tile = TileFunctions.GetTile(hoverTile.TileExponent);
             bool isGood = DrawToBoard(hoverTile.Point, tile, BoardOffset, fWorldRight, fWorldbottom, fWorldTop, fWorldLeft);

             if (isGood) { tilesDrawn++;}
          }

          
          TileData.CharInfo[] playerTile = TileFunctions.GetPlayerTile(GameMain.GameData.Phase);
          DrawToBoard(GameMain.GameData.ActivePlayer.PPlayer, playerTile, BoardOffset, fWorldRight, fWorldbottom, fWorldTop, fWorldLeft);

          // End draw transformed elements

          
          
          
          // Draw interface elements that do not scale or transform
          float fMouseScreenX, fMouseScreenY;
          UpdateLogic.ScreenToWorld(
             GameMain.ConsoleEngine.GetMousePos().X, GameMain.ConsoleEngine.GetMousePos().Y, 
             out fMouseScreenX, out fMouseScreenY);
          string gameWonMsg = GameMain.GameData.WinningPlayer != null ? $"Game over, {GameMain.GameData.WinningPlayer} won!" : "";
          
          GameMain.ConsoleEngine.Fill(new Point(0, 0), new Point(GameMain.ScreenWidth - 20, BoardOffset.Y), 6 );
          GameMain.ConsoleEngine.WriteText(new Point(10, 4), $"{Message}", 4);
          GameMain.ConsoleEngine.WriteText(
             new Point((GameMain.ScreenWidth - 20 - gameWonMsg.Length) / 2, GameMain.ScreenHeight / 2),
             $"{gameWonMsg}", 4);
          GameMain.ConsoleEngine.WriteText(new Point(2, 0), $"[ESC] MENU", 4);
          GameMain.ConsoleEngine.WriteText(new Point(14, 0), $"[R] Reset", GameMain.GameData.Phase == 1 ? 4 : 3);

          
          GameMain.ConsoleEngine.Fill(
             new Point(GameMain.ScreenWidth - 20, 0), 
             new Point(GameMain.ScreenWidth, GameMain.ScreenHeight), 
             4 );
          GameMain.ConsoleEngine.WriteText(
             new Point(GameMain.ScreenWidth - 18, 2), 
             $"offsetX:{Math.Round(UpdateLogic.fOffsetX, 4)}", 4);
          GameMain.ConsoleEngine.WriteText(
             new Point(GameMain.ScreenWidth - 18, 3), 
             $"offsetY:{Math.Round(UpdateLogic.fOffsetY, 4)}", 4);
          GameMain.ConsoleEngine.WriteText(
             new Point(GameMain.ScreenWidth - 18, 5), 
             $"Tiles drawn:{tilesDrawn}", 4);
          GameMain.ConsoleEngine.WriteText(
             new Point(GameMain.ScreenWidth - 18, 6), 
             $"Zoom:{Math.Round(GameMain.GameData.ActivePlayer.fScaleX, 3)}:" +
             $"{Math.Round(GameMain.GameData.ActivePlayer.fScaleY, 3)}", 4);
          GameMain.ConsoleEngine.WriteText(
             new Point(GameMain.ScreenWidth - 18, 7), 
             $"W Mouse: {fMouseScreenX}:{fMouseScreenY}", 4);
          GameMain.ConsoleEngine.WriteText(
             new Point(GameMain.ScreenWidth - 18, 8), 
             $"S Mouse: {GameMain.ConsoleEngine.GetMousePos().X}:{GameMain.ConsoleEngine.GetMousePos().Y}", 4);
          GameMain.ConsoleEngine.WriteText(
             new Point(GameMain.ScreenWidth - 18, 9), 
             $"P B Pos: {GameMain.GameData.ActivePlayer.PPlayer.X}:{GameMain.GameData.ActivePlayer.PPlayer.Y}", 4);
          GameMain.ConsoleEngine.WriteText(
             new Point(GameMain.ScreenWidth - 18, 10), 
             $"P S Pos: {GameMain.GameData.ActivePlayer.PPlayer.X * TileData.GetWidth() + BoardOffset.X}:" +
             $"{GameMain.GameData.ActivePlayer.PPlayer.Y * TileData.GetHeight() + BoardOffset.Y}", 4);
          GameMain.ConsoleEngine.WriteText(
             new Point(GameMain.ScreenWidth - 18, 16), 
             $"Rot H: {GameMain.GameData.ActivePlayer.IsHorizontalPlacement}", 4);
          GameMain.ConsoleEngine.WriteText(
             new Point(GameMain.ScreenWidth - 18, 17), 
             $"Player: {GameMain.GameData.ActivePlayer.Name}", 4);
          GameMain.ConsoleEngine.WriteText(
             new Point(GameMain.ScreenWidth - 18, 18), 
             $"Own Board: {GameMain.GameData.ActivePlayer.IsViewingOwnBoard}", 4);
          GameMain.ConsoleEngine.WriteText(
             new Point(GameMain.ScreenWidth - 18, 19), 
             $"Phase: {GameMain.GameData.Phase}", 4);
          GameMain.ConsoleEngine.WriteText(
             new Point(GameMain.ScreenWidth - 18, 20), 
             $"Frame: {_frameCount}", 4);
          
          foreach (var dialogOption in DialogOptions.Select((value, i) => new { value, i }))
          {
             GameMain.ConsoleEngine.WriteText(
                new Point(GameMain.ScreenWidth - 18, 22 + dialogOption.i), 
                $"[{dialogOption.i + 1}] {dialogOption.value}", 4);
          }

          GameMain.ConsoleEngine.DisplayBuffer();
          _frameCount++;
       }
       

       private static bool DrawToBoard(RogueSharp.Point drawPoint, TileData.CharInfo[] tile, Point offset, 
          float fWorldRight, float fWorldbottom, float fWorldTop, float fWorldLeft)
       {
          int boardOffsetX = offset.X;
          int boardOffsetY = offset.Y;
          
          
          float worldX = drawPoint.X * TileData.GetWidth(), worldY = drawPoint.Y * TileData.GetHeight();

          if (fWorldRight < boardOffsetX + worldX ||
              fWorldbottom < boardOffsetY + worldY ||
              fWorldTop > boardOffsetY + worldY + TileData.GetHeight() - 1 ||
              fWorldLeft > boardOffsetX + worldX + TileData.GetWidth() - 1)
          {
             return false;
          }
          int tile_sx, tile_sy;
          UpdateLogic.WorldToScreen(worldX, worldY, out tile_sx, out tile_sy);
          
          for (int i = 0; i < TileData.GetHeight(); i++)
          {
             for (int j = 0; j < TileData.GetWidth(); j++)
             {
                TileData.CharInfo charInfo = tile[i * TileData.GetHeight() + j];
                Point point = new Point(boardOffsetX + tile_sx + j, boardOffsetY + tile_sy + i);
                GameMain.ConsoleEngine.WriteText(point, charInfo.GetGlyphString(), charInfo.GetColor());
             }
          }

          return true;
       }
    }
}