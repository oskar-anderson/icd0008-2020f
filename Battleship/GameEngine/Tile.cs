using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public enum TileValue
    {
        EmptyTileV1 = 1,
        EmptyTileV2 = 2,
        SelectedTile = 3,
        Ship = 4,
        ImpossibleShip = 5,
        HitShip = 6,
        HitWater = 7,
    }
    public static class Tile
    {
        public static readonly TileValue[] HitTiles = {TileValue.HitShip, TileValue.HitWater};
        public static readonly TileValue[] SeaTiles = {TileValue.EmptyTileV1, TileValue.EmptyTileV2};

        private static Random random = new Random();
        
        public static int Width = 4;
        public static int Height = 4;

        public static readonly Dictionary<char, CharInfo> charToCharInfoMap = new Dictionary<char, CharInfo>()
        {
            { '~', new CharInfo('~', 3) },
            { '^', new CharInfo('^', 3) },
            { '#', new CharInfo('#', 5) },
            { '@', new CharInfo('@', 4) },
            { 'X', new CharInfo('X', 10) },
            { 'Ä', new CharInfo('X', 4) },
            { '*', new CharInfo('*', 4) },
        };
        
        public static CharInfo[] GetTile(TileValue tileValue)
        {
            CharInfo[] tile;
            StringBuilder sbTile = new StringBuilder();
            
            switch (tileValue)
            {
                case TileValue.EmptyTileV1:
                    sbTile
                        .Append("~~~~")
                        .Append("~##~")
                        .Append("~##~")
                        .Append("~~~~");
                    tile = sbTile.ToString().ToCharInfoArray();
                    break;
                case TileValue.EmptyTileV2:
                    sbTile
                        .Append("^^^^")
                        .Append("^##^")
                        .Append("^##^")
                        .Append("^^^^");
                    tile = sbTile.ToString().ToCharInfoArray();
                    break;
                case TileValue.SelectedTile:
                    sbTile
                        .Append("~~~~")
                        .Append("~@@~")
                        .Append("~@@~")
                        .Append("~~~~");
                    tile = sbTile.ToString().ToCharInfoArray();
                    break;
                case TileValue.Ship:
                    sbTile
                        .Append("~~~~")
                        .Append("~XX~")
                        .Append("~XX~")
                        .Append("~~~~");
                    tile = sbTile.ToString().ToCharInfoArray();
                    break;
                case TileValue.ImpossibleShip:
                    sbTile
                        .Append("~~~~")
                        .Append("~ÄÄ~")
                        .Append("~ÄÄ~")
                        .Append("~~~~");
                    tile = sbTile.ToString().ToCharInfoArray();
                    break;
                case TileValue.HitShip:
                    sbTile
                        .Append("*~~*")
                        .Append("~**~")
                        .Append("~**~")
                        .Append("*~~*");
                    tile = sbTile.ToString().ToCharInfoArray();
                    break;
                case TileValue.HitWater:
                    sbTile
                        .Append("~~~~")
                        .Append("~^^~")
                        .Append("~^^~")
                        .Append("~~~~");
                    tile = sbTile.ToString().ToCharInfoArray();
                    break;
                default:
                    throw new Exception("Unknown value: " + tileValue);
            }

            if (tile.Any(x => x == null)) { throw new Exception("Tile content is messed up!");}
            if (tile.Length != Width * Height) { throw new Exception("Tile size is messed up!");}
            return tile;
        }


        public static TileValue GetSeaTile()
        {
            int rnd = random.Next();
            return SeaTiles[rnd % SeaTiles.Length];
        }
    }

    public class CharInfo
    {
        private char Glyph;
        private int Color;

        public CharInfo(char glyph, int color)
        {
            this.Glyph = glyph;
            this.Color = color;
        }

        public char GetGlyphChar() => Glyph;
        public string GetGlyphString() => Glyph.ToString();
        public int GetColor() => Color;

        public override string ToString() => GetGlyphString();
    }

    public static class Extensions
    {
        public static CharInfo[] ToCharInfoArray(this string text)
        {
            CharInfo[] charInfoArray = new CharInfo[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                bool isGood = Tile.charToCharInfoMap.TryGetValue(text[i], out CharInfo charInfo);
                if (isGood) { charInfoArray[i] = charInfo; }
                else { throw new Exception($"Tile is messed up. Unrecognized character: {text[i]}"); }
            }

            return charInfoArray;
        }
    }
}