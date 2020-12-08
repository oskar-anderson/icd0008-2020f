using System;
using System.Collections.Generic;
using Domain;
using Domain.Model;
using Game;
using Game.Tile;
using RogueSharp;

namespace WebApp
{
    public static class WebDrawLogic
    {
        public static int[,] Draw(double gameTime, GameData gameData, DrawLogicData drawLogicData)
        {
            gameData.FrameCount++;

            float fScreenLeft, fScreenTop, fScreenRight, fScreenBottom;
            UpdateLogic.ScreenToWorld(
                0, 0,
                1, 1,
                gameData.ActivePlayer.fOffsetX, gameData.ActivePlayer.fOffsetY,
                out fScreenLeft, out fScreenTop);
            UpdateLogic.ScreenToWorld(
                10 * TileData.GetWidth(), 10 * TileData.GetHeight(),
                1, 1,
                gameData.ActivePlayer.fOffsetX, gameData.ActivePlayer.fOffsetY,
                out fScreenRight, out fScreenBottom);

            int tilesDrawn = 0;

            int[,] worldTileValue = new int[gameData.ActivePlayer.Board.GetHeight(), gameData.ActivePlayer.Board.GetWidth()];
            Point offset = new Point((int) fScreenLeft / 4, (int) fScreenTop / 4);
            List<Point> board = new Rectangle(offset.X, offset.Y, gameData.ActivePlayer.Board.GetWidth(), gameData.ActivePlayer.Board.GetHeight()).ToPoints();
            foreach (var point in board)
            {
                int tileExponent;
                if (new Rectangle(0, 0, gameData.ActivePlayer.Board.GetWidth(), gameData.ActivePlayer.Board.GetHeight()).Contains(point))
                {
                    tileExponent = BaseBattleship.GetBoard(gameData).Get(point);
                    if (gameData.Phase == 2 && tileExponent == TileData.Ship.exponent)
                    {
                        tileExponent = TileFunctions.GetSeaTile(point.GetIndex());
                    }
                }
                    
                else
                {
                    tileExponent = TileData.VoidTile.exponent;
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
                gameData.ActivePlayer.Board.GetWidth(),
                gameData.ActivePlayer.Board.GetHeight());
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
            
            bool playerIsGood = GetTileValueToDrawToBoard(gameData.ActivePlayer.PPlayer, offset, drawLogicData.PlayerTileValue, fScreenRight, fScreenBottom, fScreenTop, fScreenLeft, ref worldTileValue);

            if (playerIsGood)
            {
                tilesDrawn++;
            }
            
            return worldTileValue;
        }
        
        public static bool GetTileValueToDrawToBoard(Point drawPoint, Point offSetPoint, int tileExponent, 
            float fScreenRight, float fScreenbottom, float fScreenTop, float fScreenLeft, ref int[,] result)
        {
            int worldX = drawPoint.X * TileData.GetWidth();
            int worldY = drawPoint.Y * TileData.GetHeight();

            if (fScreenRight < worldX ||
                fScreenbottom < worldY ||
                fScreenTop > worldY + TileData.GetHeight() - 1 ||
                fScreenLeft > worldX + TileData.GetWidth() - 1)
            {
                return false;
            }
            result.Set(drawPoint - offSetPoint, tileExponent);

            return true;
        }

        public static void GetTileValueToTileData(Point drawPoint, int tileExponent, ref TileData.CharInfo[,] result)
        {
            TileData.CharInfo[] tile = TileFunctions.GetTile(tileExponent);
            int worldX = drawPoint.X * TileData.GetWidth();
            int worldY = drawPoint.Y * TileData.GetHeight();
                    
            int tile_sx, tile_sy;
            UpdateLogic.WorldToScreen(worldX, worldY,
                1, 1, 
                0, 0,  
                out tile_sx, out tile_sy);
            
            for (int i = 0; i < TileData.GetHeight(); i++)
            {
                for (int j = 0; j < TileData.GetWidth(); j++)
                {
                    TileData.CharInfo charInfo = tile[i * TileData.GetHeight() + j];
                    Point point = new Point(tile_sx + j, + tile_sy + i);
                    result.Set(point, charInfo);
                }
            }
        }
    }
}