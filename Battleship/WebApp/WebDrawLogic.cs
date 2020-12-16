using System;
using System.Collections.Generic;
using Domain;
using Domain.Model;
using Domain.Tile;
using Game;
using RogueSharp;

namespace WebApp
{
    public static class WebDrawLogic
    {
        public static TileData.CharInfo[,] GetDraw(GameData gameData)
        {
            gameData.FrameCount++;
            
            TileData.CharInfo[,] map = new TileData.CharInfo[40, 40];
            BaseDraw.GetDrawArea(gameData, ref map);
            return map;
        }
        public static string[,] Draw(double gameTime, GameData gameData)
        {
            gameData.FrameCount++;

            float fScreenLeft, fScreenTop, fScreenRight, fScreenBottom;
            UpdateLogic.ScreenToWorld(
                0, 0,
                1, 1,
                gameData.ActivePlayer.fCameraPixelPosX, gameData.ActivePlayer.fCameraPixelPosY,
                out fScreenLeft, out fScreenTop);
            UpdateLogic.ScreenToWorld(
                10 * TileData.Width, 10 * TileData.Height,
                1, 1,
                gameData.ActivePlayer.fCameraPixelPosX, gameData.ActivePlayer.fCameraPixelPosY,
                out fScreenRight, out fScreenBottom);

            int tilesDrawn = 0;

            string[,] worldTileValue = new string[gameData.Board2D.GetHeight(), gameData.Board2D.GetWidth()];
            Point offset = new Point((int) fScreenLeft / 4, (int) fScreenTop / 4);
            List<Point> board = new Rectangle(offset.X, offset.Y, gameData.ActivePlayer.BoardBounds.Right, gameData.ActivePlayer.BoardBounds.Bottom).ToPoints();
            foreach (var point in board)
            {
                string tileExponent;
                if (gameData.ActivePlayer.BoardBounds.Contains(point))
                {
                    tileExponent = gameData.Board2D.Get(point);
                    if (gameData.State == GameState.Shooting && tileExponent == TextureValue.IntactShip)
                    {
                        tileExponent = TileFunctions.GetSeaTile(point.GetIndex());
                    }
                }
                    
                else
                {
                    tileExponent = TextureValue.VoidTile;
                }

                bool isGood = GetTileValueToDrawToBoard(point, offset, tileExponent, fScreenRight, fScreenBottom, fScreenTop,
                    fScreenLeft, ref worldTileValue);

                if (isGood)
                {
                    tilesDrawn++;
                }
            }

            Rectangle bound = new Rectangle(
                0 + offset.X, 
                0 + offset.Y, 
                gameData.ActivePlayer.BoardBounds.Right,
                gameData.ActivePlayer.BoardBounds.Bottom);
/*
                foreach (Player.HoverElement hoverTile in gameData.ActivePlayer.BoardHover)
                {
                if (! bound.Contains(hoverTile.Point))
                {
                    continue;
                }
                bool isGood = GetTileValueToDrawToBoard(hoverTile.Point, offset, hoverTile.TileExponent, fScreenRight, fScreenBottom,
                    fScreenTop, fScreenLeft, ref worldTileValue);

                if (isGood)
                {
                    tilesDrawn++;
                }
            }
            */
            
            bool playerIsGood = GetTileValueToDrawToBoard(gameData.ActivePlayer.Sprite.Pos, offset, gameData.ActivePlayer.Sprite.Texture, fScreenRight, fScreenBottom, fScreenTop, fScreenLeft, ref worldTileValue);

            if (playerIsGood)
            {
                tilesDrawn++;
            }
            
            return worldTileValue;
        }
        
        public static bool GetTileValueToDrawToBoard(Point drawPoint, Point offSetPoint, string tileExponent, 
            float fScreenRight, float fScreenbottom, float fScreenTop, float fScreenLeft, ref string[,] result)
        {
            int worldX = drawPoint.X * TileData.Width;
            int worldY = drawPoint.Y * TileData.Height;

            if (fScreenRight < worldX ||
                fScreenbottom < worldY ||
                fScreenTop > worldY + TileData.Height - 1 ||
                fScreenLeft > worldX + TileData.Width - 1)
            {
                return false;
            }
            result.Set(drawPoint - offSetPoint, tileExponent);

            return true;
        }

        public static void GetTileValueToTileData(Point drawPoint, string tileExponent, ref TileData.CharInfo[,] result)
        {
            TileData.CharInfo[] tile = TileFunctions.GetTile(tileExponent).CharInfoArray;
            int worldX = drawPoint.X * TileData.Width;
            int worldY = drawPoint.Y * TileData.Height;
                    
            int tile_sx, tile_sy;
            UpdateLogic.WorldToScreen(worldX, worldY,
                1, 1, 
                0, 0,  
                out tile_sx, out tile_sy);
            
            for (int i = 0; i < TileData.Height; i++)
            {
                for (int j = 0; j < TileData.Width; j++)
                {
                    TileData.CharInfo charInfo = tile[i * TileData.Height + j];
                    Point point = new Point(tile_sx + j, + tile_sy + i);
                    result.Set(point, charInfo);
                }
            }
        }
    }
}