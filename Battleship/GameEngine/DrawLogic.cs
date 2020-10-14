using System;
using System.Globalization;
using ConsoleGameEngineCore;
using Point = ConsoleGameEngineCore.Point;

namespace GameEngine
{
    public static class DrawLogic
    {
       private static readonly int BoardOffsetX = 0;
       private static readonly int BoardOffsetY = 5;

       public static string Message = "";

       private static int frameCount = 1;
       
       /// <summary>
       /// This is called when the game should draw itself.
       /// </summary>
       /// <param name="gameTime">Provides a snapshot of timing values.</param>
       public static void Draw(double gameTime)
       {
          Game.ConsoleEngine.ClearBuffer();
          UpdateLogic ul = Game.ActivePlayer.UpdateLogic;
          
          // Draw transformed elements
          double dFps = 1.0d / gameTime;
          string sFps = Math.Floor(dFps).ToString(CultureInfo.InvariantCulture);
          Console.Title = "BattleShip FPS: " + sFps;

          float fWorldLeft, fWorldTop, fWorldRight, fWorldbottom;
          ul.ScreenToWorld(0, 0, out fWorldLeft, out fWorldTop);
          ul.ScreenToWorld(Game.ScreenWidth, Game.ScreenHeight, out fWorldRight, out fWorldbottom);
          
/*
          for (int y = 0; y < 10; y++)
          {
             for (int x = 0; x < 10; x++)
             {
                float sx = 0.0f, sy = y;
                float ex = 10.0f, ey = y;

                int pixel_sx, pixel_sy, pixel_ex, pixel_ey;
                
                ul.WorldToScreen(sx, sy, out pixel_sx, out pixel_sy);
                ul.WorldToScreen(ex, ey, out pixel_ex, out pixel_ey);
                
                
                Game.ConsoleEngine.Line(new Point(pixel_sx, pixel_sy), new Point(pixel_ex, pixel_ey), 3);
             }
          }
          */

          int tilesDrawn = 0;

          for (int y = 0; y < Game.BoardHeight; y++)
          {
             for (int x = 0; x < Game.BoardWidth; x++)
             {
                CharInfo[] tile = Tile.GetTile(Game.GetBoard()[y * Game.BoardWidth + x]);
                bool isGood = DrawToBoard(
                   x, y, tile, 
                   new Point(BoardOffsetX, BoardOffsetY),
                   fWorldRight, fWorldbottom, fWorldTop, fWorldLeft);

                if (isGood) { tilesDrawn++;}
             }
          }

          foreach (var hoverTile in Game.ActivePlayer.BoardHover)
          {
             CharInfo[] tile = Tile.GetTile(hoverTile.Item2);
             bool isGood = DrawToBoard(
                hoverTile.Item1.X, hoverTile.Item1.Y, tile, 
                new Point(BoardOffsetX, BoardOffsetY),
                fWorldRight, fWorldbottom, fWorldTop, fWorldLeft);

             if (isGood) { tilesDrawn++;}
          }

          CharInfo[] playerTile = Tile.GetTile(TileValue.SelectedTile);
          DrawToBoard(
             Game.ActivePlayer.pPlayer.X, Game.ActivePlayer.pPlayer.Y, playerTile, 
             new Point(BoardOffsetX, BoardOffsetY), 
             fWorldRight, fWorldbottom, fWorldTop, fWorldLeft);

          int cx, cy, cr;
          ul.WorldToScreen(Game.ActivePlayer.fSelectedTileX, Game.ActivePlayer.fSelectedTileY, out cx, out cy);
          cr = (int) (5 * Game.ActivePlayer.fScaleX);
          Game.ConsoleEngine.Sector(new Point(cx, cy), cr,1, 360 , 4, ConsoleCharacter.Full);
          // End draw transformed elements

          
          
          
          // Draw interface elements that do not scale or transform
          float fMouseScreenX, fMouseScreenY;
          ul.ScreenToWorld(Game.ConsoleEngine.GetMousePos().X, Game.ConsoleEngine.GetMousePos().Y, out fMouseScreenX, out fMouseScreenY);

          Game.ConsoleEngine.Fill(new Point(0, 0), new Point(Game.ScreenWidth - 20, BoardOffsetY), 6 );
          Game.ConsoleEngine.WriteText(new Point(10, 4), $"{Message}", 4);

          
          Game.ConsoleEngine.Fill(new Point(Game.ScreenWidth - 20, 0), new Point(Game.ScreenWidth, Game.ScreenHeight), 4 );
          Game.ConsoleEngine.WriteText(new Point(Game.ScreenWidth - 18, 2), $"offsetX:{Math.Round(ul.fOffsetX, 4)}", 4);
          Game.ConsoleEngine.WriteText(new Point(Game.ScreenWidth - 18, 3), $"offsetY:{Math.Round(ul.fOffsetY, 4)}", 4);
          Game.ConsoleEngine.WriteText(new Point(Game.ScreenWidth - 18, 5), $"Tiles drawn:{tilesDrawn}", 4);
          Game.ConsoleEngine.WriteText(new Point(Game.ScreenWidth - 18, 6), $"Zoom:{Math.Round(Game.ActivePlayer.fScaleX, 3)}:{Math.Round(Game.ActivePlayer.fScaleY, 3)}", 4);
          Game.ConsoleEngine.WriteText(new Point(Game.ScreenWidth - 18, 7), $"W Mouse: {fMouseScreenX}:{fMouseScreenY}", 4);
          Game.ConsoleEngine.WriteText(new Point(Game.ScreenWidth - 18, 8), $"S Mouse: {Game.ConsoleEngine.GetMousePos().X}:{Game.ConsoleEngine.GetMousePos().Y}", 4);
          Game.ConsoleEngine.WriteText(new Point(Game.ScreenWidth - 18, 9), $"Circle Radios: {cr}", 4);
          Game.ConsoleEngine.WriteText(new Point(Game.ScreenWidth - 18, 10), $"P B Pos: {Game.ActivePlayer.pPlayer.X}:{Game.ActivePlayer.pPlayer.Y}", 4);
          Game.ConsoleEngine.WriteText(new Point(Game.ScreenWidth - 18, 11), $"P S Pos: {Game.ActivePlayer.pPlayer.X * Tile.Width + BoardOffsetX}:{Game.ActivePlayer.pPlayer.Y * Tile.Height + BoardOffsetY}", 4);
          Game.ConsoleEngine.WriteText(new Point(Game.ScreenWidth - 18, 16), $"Rot H: {UpdateLogic.IsHorizontalPlacement}", 4);
          Game.ConsoleEngine.WriteText(new Point(Game.ScreenWidth - 18, 17), $"Player: {Game.ActivePlayer.Name}", 4);
          Game.ConsoleEngine.WriteText(new Point(Game.ScreenWidth - 18, 18), $"Own Board: {Game.ActivePlayer.IsViewingOwnBoard}", 4);
          Game.ConsoleEngine.WriteText(new Point(Game.ScreenWidth - 18, 19), $"Frame: {frameCount}", 4);

          Game.ConsoleEngine.DisplayBuffer();
          frameCount++;
       }
       

       private static bool DrawToBoard(int x, int y, CharInfo[] tile, Point offset, float fWorldRight, float fWorldbottom, float fWorldTop, float fWorldLeft)
       {
          int boardOffsetX = offset.X;
          int boardOffsetY = offset.Y;
          
          
          float worldX = x * Tile.Width, worldY = y * Tile.Height;
          int tile_sx, tile_sy;
          Game.ActivePlayer.UpdateLogic.WorldToScreen(worldX, worldY, out tile_sx, out tile_sy);

          if (fWorldRight < boardOffsetX + worldX ||
              fWorldbottom < boardOffsetY + worldY ||
              fWorldTop > boardOffsetY + worldY + Tile.Height - 1 ||
              fWorldLeft > boardOffsetX + worldX + Tile.Width - 1)
          {
             return false;
          }
          for (int i = 0; i < Tile.Height; i++)
          {
             for (int j = 0; j < Tile.Width; j++)
             {
                CharInfo charInfo = tile[i * Tile.Height + j];
                Point point = new Point(boardOffsetX + tile_sx + j, boardOffsetY + tile_sy + i);
                Game.ConsoleEngine.WriteText(point, charInfo.GetGlyphString(), charInfo.GetColor());
             }
          }

          return true;
       }

       
       public static void DrawBlank()
       {
          Game.ConsoleEngine.ClearBuffer();
          Game.ConsoleEngine.Fill(new Point(0, 0), new Point(Game.ScreenWidth, Game.ScreenHeight), 0 );
          Game.ConsoleEngine.DisplayBuffer();
       }
    }
}