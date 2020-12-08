using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Model;
using Game.Tile;
using RogueSharp;

namespace Game
{
    public static class BaseDraw
    { 
        private static Rectangle _mapTileBound = new Rectangle(0, 0, 0, 0);

        public static void GetDrawArea(GameData gameData, DrawLogicData drawLogicData, ref TileData.CharInfo[,] boardPixels)
        {
            _mapTileBound = new Rectangle(
                0,
                0,
                gameData.ActivePlayer.Board.GetWidth(), 
                gameData.ActivePlayer.Board.GetHeight());
            int[,] board = CombineMap(gameData, drawLogicData);

            float fWorldLeft, fWorldTop;
            UpdateLogic.ScreenToWorld(0, 0, gameData.ActivePlayer, out fWorldLeft, out fWorldTop);
            int offsetX = (int) Math.Floor(fWorldLeft);
            int offsetY = (int) Math.Floor(fWorldTop);

            MapToTiles(board, offsetX, offsetY, gameData.ActivePlayer.fScaleX, gameData.ActivePlayer.fScaleY, ref boardPixels);
        }

        private static int[,] CombineMap(GameData gameData, DrawLogicData drawLogicData)
        {
            int[,] board = new int[gameData.ActivePlayer.Board.GetHeight(), gameData.ActivePlayer.Board.GetWidth()];
         
            for (int y = 0; y < board.GetHeight(); y++)
            {
                for (int x = 0; x < board.GetWidth(); x++)
                {
                    Point coordinate = new Point(x, y);
                    int tileValue = BaseBattleship.GetBoard(gameData).Get(coordinate);
                    if (tileValue == TileData.Ship.exponent && gameData.Phase == 2)
                    {
                        tileValue = TileFunctions.GetSeaTile(coordinate);
                    }
                    board.Set(coordinate, tileValue);
                }
            }
            
            UpdateLogic.ShipPlacementStatus shipPlacementStatus = UpdateLogic.GetShipPlacementStatus(gameData);
            if (gameData.Phase == 1 
                && gameData.ActivePlayer.IsViewingOwnBoard
                && shipPlacementStatus.hitboxRect != null)
            {
                UpdateLogic.CanPlaceShipHoverStatus hoverResult = UpdateLogic.CanPlaceShipHover((Rectangle) shipPlacementStatus.hitboxRect, gameData);
                List<Player.HoverElement> hoverElements = hoverResult.boardHover.Where(hoverTile => _mapTileBound.Contains(hoverTile.Point)).ToList();
                foreach (var hoverTile in hoverElements)
                {
                    board.Set(hoverTile.Point, hoverTile.TileExponent);
                }

                if (! hoverResult.canBePlacedEntirely)
                {
                    drawLogicData.Message = "Cannot place there!";
                }
            }

            board.Set(gameData.ActivePlayer.PPlayer, drawLogicData.PlayerTileValue);
            return board;
        }

        private static void MapToTiles(int[,] board, int offsetX, int offsetY, float fZoomScaleX, float fZoomScaleY, ref TileData.CharInfo[,] boardPixels)
        {
            boardPixels.Fill(new TileData.CharInfo(' ', TileData.C.__));

            int tileWidth = TileData.GetWidth();
            int tileHeight = TileData.GetHeight();
            Rectangle pixelBound = new Rectangle(0, 0, boardPixels.GetWidth(), boardPixels.GetHeight());
            for (int y = 0; y < board.GetHeight(); y++)
            {
                for (int x = 0; x < board.GetWidth(); x++)
                {
                    Point coordinate = new Point(
                        x + offsetX / TileData.GetWidth(),
                        y + offsetY / TileData.GetHeight());
                    TileData.CharInfo[] tile = ! _mapTileBound.Contains(coordinate) 
                        ? TileData.VoidTile.charInfoArray
                        : TileFunctions.GetTile(board.Get(coordinate));
                    for (int i = 0; i < tile.Length; i++)
                    {
                        Point pixelPosition = new Point(
                            i % tileWidth + x * tileWidth - offsetX % TileData.GetWidth(), 
                            i / tileHeight + y * tileHeight - offsetY % TileData.GetHeight());
                        Point scaledCoordinate = new Point(
                            (int) Math.Floor(pixelPosition.X * fZoomScaleX),
                            (int) Math.Floor(pixelPosition.Y * fZoomScaleY));
                        if (! pixelBound.Contains(scaledCoordinate))
                        {
                            continue;
                        }

                        boardPixels.Set(scaledCoordinate, tile[i]);
                    }
                }
            }
        }
    }
}