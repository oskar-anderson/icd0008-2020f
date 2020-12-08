using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using RogueSharp;
using Troschuetz.Random.Generators;

namespace Game.Tile
{
    public class TileFunctions
    {
        public static TileData.CharInfo[] GetTile(int tileValue)
        {
            TileData.CharInfo[] tile = TileData.Tiles[tileValue].charInfoArray;
            if (tile.Any(x => x == null)) { throw new Exception("Tile content is messed up!");}
            if (tile.Length != TileData.GetWidth() * TileData.GetHeight()) { throw new Exception("Tile size is messed up!");}

            return tile;
        }

        public static int[,] GetRndSeaTiles(int width, int height)
        {
            int[,] board = new int[height, width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    board[j, i] = GetSeaTile(i * width + j);
                }
            }
            return board;
        }

        public static int GetSeaTile(Point seed)
        {
            return GetSeaTile(seed.GetIndex());
        }
        public static int GetSeaTile(int seed)
        {
            var xorShift = new XorShift128Generator(seed);
            var rnd = xorShift.Next();
            return TileData.SeaTiles[rnd % TileData.SeaTiles.Length];
        }
    }

}