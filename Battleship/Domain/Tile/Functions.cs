using System;
using System.Linq;
using RogueSharp;
using Troschuetz.Random.Generators;

namespace Domain.Tile
{
    public static class TileFunctions
    {
        public static TileData.TileProperty GetTile(string tileValue)
        {
            TileData.TileProperty tile = TileData.Tiles[tileValue];
            if (tile.CharInfoArray.Any(x => x == null)) { throw new Exception("Tile content is messed up!");}
            if (tile.CharInfoArray.Length != TileData.Width * TileData.Height) { throw new Exception("Tile size is messed up!");}

            return tile;
        }

        public static string[,] GetRndSeaTiles(int width, int height)
        {
            string[,] board = new string[height, width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    board[j, i] = GetSeaTile(i * width + j);
                }
            }
            return board;
        }

        public static string GetSeaTile(Point seed)
        {
            return GetSeaTile(seed.GetIndex());
        }
        public static string GetSeaTile(int seed)
        {
            var xorShift = new XorShift128Generator(seed);
            var rnd = xorShift.Next();
            return TileData.SeaTiles[rnd % TileData.SeaTiles.Length];
        }
    }

}