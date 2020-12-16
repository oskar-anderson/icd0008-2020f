using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Model;
using Domain.Tile;
using RogueSharp;

namespace Game
{
    public static class BaseDraw
    {
        public static void Get_UI(GameData gameData)
        {
            gameData.ActivePlayer.UI_Reset();
            if (gameData.State == GameState.Shooting)
            {
                gameData.ActivePlayer.UI_DialogOptions.Add(new Player.DialogItem(true, "Z", "Shoot"));
            }
            else if (gameData.State == GameState.Placement)
            {
                UpdateLogic.ShipPlacementStatus shipPlacementStatus = UpdateLogic.GetShipPlacementStatus(gameData);
                List<Player.DialogItem> playerDialogItems = new List<Player.DialogItem>()
                {
                    new Player.DialogItem(shipPlacementStatus.isPlaceable, "Z", "Place"),
                    new Player.DialogItem(shipPlacementStatus.hitboxRect != null, "X", "Rotate"),
                    new Player.DialogItem(true, "1", "Randomize"),
                    new Player.DialogItem(true, "2", "Clear"),
                    new Player.DialogItem(shipPlacementStatus.isStartable, "3", "Start")
                };
               
                foreach (var dialogItem in playerDialogItems.Where(x => x.isActive))
                {
                    gameData.ActivePlayer.UI_DialogOptions.Add(dialogItem);
                }
            }
        }
        public static void GetDrawArea(GameData gameData, ref TileData.CharInfo[,] boardPixels)
        {
            Rectangle mapTileBound = new Rectangle(
                0,
                0,
                gameData.Board2D.GetWidth(), 
                gameData.Board2D.GetHeight());
            string[,] board = CombineMap(gameData);

            float fWorldLeft, fWorldTop;
            UpdateLogic.ScreenToWorld(0, 0, gameData.ActivePlayer, out fWorldLeft, out fWorldTop);
            int offsetX = (int) Math.Floor(fWorldLeft);
            int offsetY = (int) Math.Floor(fWorldTop);

            MapToTiles(board, offsetX, offsetY, gameData.ActivePlayer.fCameraScaleX, gameData.ActivePlayer.fCameraScaleY, mapTileBound, ref boardPixels);
        }

        private static string[,] CombineMap(GameData gameData)
        {
            string[,] board = new string[gameData.Board2D.GetHeight(), gameData.Board2D.GetWidth()];
         
            for (int y = 0; y < board.GetHeight(); y++)
            {
                for (int x = 0; x < board.GetWidth(); x++)
                {
                    Point coordinate = new Point(x, y);
                    string tileValue = gameData.Board2D.Get(coordinate);
                    if (tileValue == TextureValue.IntactShip && gameData.State == GameState.Shooting)
                    {
                        tileValue = TileFunctions.GetSeaTile(coordinate);
                    }
                    board.Set(coordinate, tileValue);
                }
            }
            
            UpdateLogic.ShipPlacementStatus shipPlacementStatus = UpdateLogic.GetShipPlacementStatus(gameData);
            if (gameData.State == GameState.Placement
                && shipPlacementStatus.hitboxRect != null)
            {
                bool canBePlacedEntirely = UpdateLogic.CanPlaceShipHover(
                    (Rectangle) shipPlacementStatus.hitboxRect, 
                    gameData, 
                    out List<Player.HoverElement> hoverElements);
                hoverElements = hoverElements.Where(hoverTile => gameData.ActivePlayer.BoardBounds.Contains(hoverTile.Point)).ToList();
                foreach (var hoverTile in hoverElements)
                {
                    board.Set(hoverTile.Point, hoverTile.TileExponent);
                }

                if (! canBePlacedEntirely)
                {
                    gameData.ActivePlayer.UI_Message = "Cannot place there!";
                }
            }
            
            board.Set(gameData.ActivePlayer.Sprite.Pos, gameData.ActivePlayer.Sprite.Texture);
            /*
            TODO Remove object reference between gameData.Sprites and gameData.(In)activePlayer.Sprite is lost on serialization load
            foreach (Sprite sprite in gameData.Sprites)
            {
                board.Set(sprite.Pos, sprite.Texture);
            }
            */
            return board;
        }

        private static void MapToTiles(string[,] board, int offsetX, int offsetY, float fZoomScaleX, float fZoomScaleY, Rectangle mapTileBound, ref TileData.CharInfo[,] boardPixels)
        {
            boardPixels.Fill(new TileData.CharInfo(' ', TileData.C.__));

            int tileWidth = TileData.Width;
            int tileHeight = TileData.Height;
            Rectangle pixelBound = new Rectangle(0, 0, boardPixels.GetWidth(), boardPixels.GetHeight());
            for (int y = 0; y < board.GetHeight(); y++)
            {
                for (int x = 0; x < board.GetWidth(); x++)
                {
                    Point coordinate = new Point(
                        x + offsetX / TileData.Width,
                        y + offsetY / TileData.Height);
                    TileData.CharInfo[] tile = ! mapTileBound.Contains(coordinate) 
                        ? TileData.VoidTile.CharInfoArray
                        : TileFunctions.GetTile(board.Get(coordinate)).CharInfoArray;
                    for (int i = 0; i < tile.Length; i++)
                    {
                        Point pixelPosition = new Point(
                            i % tileWidth + x * tileWidth - offsetX % TileData.Width, 
                            i / tileHeight + y * tileHeight - offsetY % TileData.Height);
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

        public static void CenterCamera(Player player)
        {
            int i = player.Sprite.Pos.X * TileData.Width - 4 * TileData.Width;
            player.fCameraPixelPosX = player.Sprite.Pos.X * TileData.Width - 4 * TileData.Width;
            player.fCameraPixelPosY = player.Sprite.Pos.Y * TileData.Height - 4 * TileData.Height;
        }
    }
}