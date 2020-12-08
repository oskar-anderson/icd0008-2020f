using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Domain;
using Domain.Model;
using Game;
using Game.Tile;
using RogueSharp;
using Point = ConsoleGameEngineCore.Point;

namespace ConsoleApp
{
   public static class ConsoleDrawLogic
   {
      private static readonly Point BoardOffset = new Point(0, 5);
      
      /// <summary>
      /// This is called when the game should draw itself.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      /// <param name="gameData">Game data</param>
      /// <param name="drawLogicData">Properties like error messages and available dialog options used in drawing functions</param>
      public static void Draw(double gameTime, GameData gameData, DrawLogicData drawLogicData)
      {
         Player player = gameData.ActivePlayer;
         ConsoleBattle.ConsoleEngine.ClearBuffer();

         // Draw transformed elements
         double dFps = 1.0d / gameTime;
         string sFps = Math.Floor(dFps).ToString(CultureInfo.InvariantCulture);
         Console.Title = "BattleShip FPS: " + sFps;
         
         TileData.CharInfo[,] map = new TileData.CharInfo[40, 40]; 
         BaseDraw.GetDrawArea(gameData, drawLogicData, ref map);
         for (int y = 0; y < map.GetHeight(); y++)
         {
            for (int x = 0; x < map.GetWidth(); x++)
            {
               TileData.CharInfo tile = map.Get(new RogueSharp.Point(x, y));
               Point screenCoord = new Point(
                  x + BoardOffset.X, 
                  y + BoardOffset.Y); 
               ConsoleBattle.ConsoleEngine.WriteText(screenCoord, tile.GetGlyphString(), tile.GetColor());
            }
         }
         // End draw transformed elements

          
          
          
         // Draw interface elements that do not scale or transform
         float fMouseScreenX, fMouseScreenY;
         UpdateLogic.ScreenToWorld(
            ConsoleBattle.ConsoleEngine.GetMousePos().X, ConsoleBattle.ConsoleEngine.GetMousePos().Y, player, 
            out fMouseScreenX, out fMouseScreenY);
         fMouseScreenX = (float) Math.Floor(fMouseScreenX);
         fMouseScreenY = (float) Math.Floor(fMouseScreenY);
         string gameWonMsg = gameData.WinningPlayer != null ? $"Game over, {gameData.WinningPlayer} won!" : "";
          
         ConsoleBattle.ConsoleEngine.Fill(new Point(0, 0), new Point(ConsoleBattle.ScreenWidth - 20, BoardOffset.Y), 6 );
         ConsoleBattle.ConsoleEngine.WriteText(new Point(10, 4), $"{drawLogicData.Message}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point((ConsoleBattle.ScreenWidth - 20 - gameWonMsg.Length) / 2, ConsoleBattle.ScreenHeight / 2),
            $"{gameWonMsg}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(new Point(2, 0), $"[ESC] MENU", 4);


         ConsoleBattle.ConsoleEngine.Fill(
            new Point(ConsoleBattle.ScreenWidth - 20, 0), 
            new Point(ConsoleBattle.ScreenWidth, ConsoleBattle.ScreenHeight), 
            4 );
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 2), 
            $"offsetX:{Math.Round(player.fOffsetX, 4)}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 3), 
            $"offsetY:{Math.Round(player.fOffsetY, 4)}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 6), 
            $"Zoom:{Math.Round(gameData.ActivePlayer.fScaleX, 3)}:" +
            $"{Math.Round(gameData.ActivePlayer.fScaleY, 3)}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 7), 
            $"W Mouse: {fMouseScreenX}:{fMouseScreenY}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 8), 
            $"S Mouse: {ConsoleBattle.ConsoleEngine.GetMousePos().X}:{ConsoleBattle.ConsoleEngine.GetMousePos().Y}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 9), 
            $"P Tile Pos: {gameData.ActivePlayer.PPlayer.X}:{gameData.ActivePlayer.PPlayer.Y}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 10), 
            $"P Pixel Pos: {gameData.ActivePlayer.PPlayer.X * TileData.GetWidth() + BoardOffset.X}:" +
            $"{gameData.ActivePlayer.PPlayer.Y * TileData.GetHeight() + BoardOffset.Y}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 16), 
            $"Rot H: {gameData.ActivePlayer.IsHorizontalPlacement}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 17), 
            $"Player: {gameData.ActivePlayer.Name}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 18), 
            $"Own Board: {gameData.ActivePlayer.IsViewingOwnBoard}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 19), 
            $"Phase: {gameData.Phase}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 20), 
            $"Frame: {gameData.FrameCount}", 4);

         for (int i = 0; i < drawLogicData.DialogOptions.Count; i++)
         {
            var dialogOption = drawLogicData.DialogOptions[i];
            ConsoleBattle.ConsoleEngine.WriteText(
               new Point(ConsoleBattle.ScreenWidth - 18, 22 + i), 
               $"[{dialogOption.key}] {dialogOption.text}", 4);
         }

         ConsoleBattle.ConsoleEngine.DisplayBuffer();
         gameData.FrameCount++;
      }

      private static bool DrawToBoard(RogueSharp.Point drawPoint, TileData.CharInfo[] tile, Player player,
         float fWorldRight, float fWorldbottom, float fWorldTop, float fWorldLeft)
      {
         int boardOffsetX = BoardOffset.X;
         int boardOffsetY = BoardOffset.Y;
          
          
         float worldX = drawPoint.X * TileData.GetWidth(), worldY = drawPoint.Y * TileData.GetHeight();

         if (fWorldRight < boardOffsetX + worldX ||
             fWorldbottom < boardOffsetY + worldY ||
             fWorldTop > boardOffsetY + worldY + TileData.GetHeight() - 1 ||
             fWorldLeft > boardOffsetX + worldX + TileData.GetWidth() - 1)
         {
            return false;
         }
         int tile_sx, tile_sy;
         UpdateLogic.WorldToScreen(worldX, worldY, player, out tile_sx, out tile_sy);
          
         for (int i = 0; i < TileData.GetHeight(); i++)
         {
            for (int j = 0; j < TileData.GetWidth(); j++)
            {
               TileData.CharInfo charInfo = tile[i * TileData.GetHeight() + j];
               Point point = new Point(boardOffsetX + tile_sx + j, boardOffsetY + tile_sy + i);
               ConsoleBattle.ConsoleEngine.WriteText(point, charInfo.GetGlyphString(), charInfo.GetColor());
            }
         }

         return true;
      }
   }
}