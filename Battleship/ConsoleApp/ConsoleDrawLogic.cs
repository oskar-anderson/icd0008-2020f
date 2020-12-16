using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Domain;
using Domain.Model;
using Domain.Tile;
using Game;
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
      public static void Draw(double gameTime, GameData gameData)
      {
         ConsoleBattle.ConsoleEngine.ClearBuffer();
         BaseDraw.Get_UI(gameData);

         // Draw transformed elements
         double dFps = 1.0d / gameTime;
         string sFps = Math.Floor(dFps).ToString(CultureInfo.InvariantCulture);
         Console.Title = "BattleShip FPS: " + sFps;
         
         TileData.CharInfo[,] map = new TileData.CharInfo[40, 40];
         BaseDraw.GetDrawArea(gameData, ref map);
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
            ConsoleBattle.ConsoleEngine.GetMousePos().X, ConsoleBattle.ConsoleEngine.GetMousePos().Y, gameData.ActivePlayer, 
            out fMouseScreenX, out fMouseScreenY);
         fMouseScreenX = (float) Math.Floor(fMouseScreenX);
         fMouseScreenY = (float) Math.Floor(fMouseScreenY);

         ConsoleBattle.ConsoleEngine.Fill(new Point(0, 0), new Point(ConsoleBattle.ScreenWidth - 20, BoardOffset.Y), 6 );
         ConsoleBattle.ConsoleEngine.WriteText(new Point(10, 4), $"{gameData.ActivePlayer.UI_Message}", 4);
         if (gameData.State == GameState.GameOver)
         {
            UpdateLogic.IsOver(gameData, out string winner);
            string gameWonMsg = $"Game over, {winner} won!";
            ConsoleBattle.ConsoleEngine.WriteText(
                        new Point((ConsoleBattle.ScreenWidth - 20 - gameWonMsg.Length) / 2, ConsoleBattle.ScreenHeight / 2),
                        $"{gameWonMsg}", 4);
         }
         ConsoleBattle.ConsoleEngine.WriteText(new Point(2, 0), $"[ESC] MENU", 4);


         ConsoleBattle.ConsoleEngine.Fill(
            new Point(ConsoleBattle.ScreenWidth - 20, 0), 
            new Point(ConsoleBattle.ScreenWidth, ConsoleBattle.ScreenHeight), 
            4 );
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 2), 
            $"offsetX:{Math.Round(gameData.ActivePlayer.fCameraPixelPosX, 4)}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 3), 
            $"offsetY:{Math.Round(gameData.ActivePlayer.fCameraPixelPosY, 4)}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 6), 
            $"Zoom:{Math.Round(gameData.ActivePlayer.fCameraScaleX, 3)}:" +
            $"{Math.Round(gameData.ActivePlayer.fCameraScaleY, 3)}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 7), 
            $"W Mouse: {fMouseScreenX}:{fMouseScreenY}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 8), 
            $"S Mouse: {ConsoleBattle.ConsoleEngine.GetMousePos().X}:{ConsoleBattle.ConsoleEngine.GetMousePos().Y}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 9), 
            $"P Tile Pos: {gameData.ActivePlayer.Sprite.Pos.X}:{gameData.ActivePlayer.Sprite.Pos.Y}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 10), 
            $"P Pixel Pos: {gameData.ActivePlayer.Sprite.Pos.X * TileData.Width + BoardOffset.X}:" +
            $"{gameData.ActivePlayer.Sprite.Pos.Y * TileData.Height + BoardOffset.Y}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 16), 
            $"Rot H: {gameData.ActivePlayer.IsHorizontalPlacement}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 17), 
            $"Player: {gameData.ActivePlayer.Name}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 19), 
            $"Phase: {gameData.State}", 4);
         ConsoleBattle.ConsoleEngine.WriteText(
            new Point(ConsoleBattle.ScreenWidth - 18, 20), 
            $"Frame: {gameData.FrameCount}", 4);

         for (int i = 0; i < gameData.ActivePlayer.UI_DialogOptions.Count; i++)
         {
            var dialogOption = gameData.ActivePlayer.UI_DialogOptions[i];
            ConsoleBattle.ConsoleEngine.WriteText(
               new Point(ConsoleBattle.ScreenWidth - 18, 22 + i), 
               $"[{dialogOption.key}] {dialogOption.text}", 4);
         }

         ConsoleBattle.ConsoleEngine.DisplayBuffer();
         gameData.FrameCount++;
      }
   }
}