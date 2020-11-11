using System;
using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using Troschuetz.Random.Generators;

namespace Game.Tile
{
    public static class TileFunctions
    {
        
        private static XorShift128Generator _xorShift = new XorShift128Generator();
        public static int playerTileValue = TileData.SelectedTileGreen.exponent;

        public static TileData.CharInfo[] GetTile(int tileValue)
        {
            return GetTile(tileValue, -1, 1);
        }
        
        public static TileData.CharInfo[] GetTile(int[,] board, Point p, int phase)
        {
            int tileValue = board.Get(p);
            return GetTile(tileValue, p.X + p.Y * board.GetLength(0), phase);
        }

        private static TileData.CharInfo[] GetTile(int tileValue, int tileIdx, int phase)
        {
            TileData.CharInfo[] tile;
            if (phase == 2 && tileValue == TileData.Ship.exponent)
            {
               tile = TileData.Tiles[GetSeaTile(tileIdx)].charInfoArray;
            }
            else
            {
                tile = TileData.Tiles[tileValue].charInfoArray;
            }

            if (tile.Any(x => x == null)) { throw new Exception("Tile content is messed up!");}
            if (tile.Length != TileData.GetWidth() * TileData.GetHeight()) { throw new Exception("Tile size is messed up!");}

            return tile;
        }

        public static TileData.CharInfo[] GetPlayerTile(int gamePhase)
        {
            return GetTile(playerTileValue);
        }

        public static int[,] GetRndSeaTiles(int width, int height)
        {
            int[,] board = new int[height, width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    board[j, i] = GetSeaTile(i * j);
                }
            }
            return board;
        }

        private static int GetSeaTile(int seed)
        {
            _xorShift = new XorShift128Generator(seed);
            var rnd = _xorShift.Next();
            return TileData.SeaTiles[rnd % TileData.SeaTiles.Length];
        }
    }

}