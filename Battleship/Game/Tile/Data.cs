using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace Game.Tile
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class TileData
    {
        public static class TileValue
        {
            public const int EmptyTileV1 = 0;
            public const int EmptyTileV2 = 1;
            public const int SelectedTileRed = 2;
            public const int Ship = 3;
            public const int ImpossibleShip = 4;
            public const int HitShip = 5;
            public const int HitWater = 6;
            public const int SelectedTileGreen = 7;
            public const int ImpossibleShipHitbox = 8;
            public const int VoidTile = 9;
        }
        
        public static readonly int[] HitTiles = {TileValue.HitShip, TileValue.HitWater};
        public static readonly int[] SeaTiles = {TileValue.EmptyTileV1, TileValue.EmptyTileV2};

        
        public static class C
        {
            public static int DD = TileColor.Black;
            public static int DB = TileColor.DarkBlue;
            public static int DG = TileColor.DarkGreen;
            public static int DC = TileColor.DarkCyan;
            public static int DR = TileColor.DarkRed;
            public static int DM = TileColor.DarkMagenta;
            public static int DY = TileColor.DarkYellow;
            public static int _S = TileColor.Gray;
            public static int DS = TileColor.DarkGray;
            public static int _B = TileColor.Blue;
            public static int _G = TileColor.Green;
            public static int _C = TileColor.Cyan;
            public static int _R = TileColor.Red;
            public static int _M = TileColor.Magenta;
            public static int _Y = TileColor.Yellow;
            public static int __ = TileColor.White;
        }
        
        private static class TileColor
        {
            public static int Black = 0;
            public static int DarkBlue = 1;
            public static int DarkGreen = 2;
            public static int DarkCyan = 3;
            public static int DarkRed = 4;
            public static int DarkMagenta = 5;
            public static int DarkYellow = 6;
            public static int Gray = 7;
            public static int DarkGray = 8;
            public static int Blue = 9;
            public static int Green = 10;
            public static int Cyan = 11;
            public static int Red = 12;
            public static int Magenta = 13;
            public static int Yellow = 14;
            public static int White = 15;
        }

        private const int Width = 4;
        private const int Height = 4;

        public static int GetWidth() => Width;
        public static int GetHeight() => Height;

        public static readonly TileInfo VoidTile = new TileInfo(TileValue.VoidTile, new StringBuilder()
                .Append("    ")
                .Append("    ")
                .Append("    ")
                .Append("    "),
            new int[]
            {
                C.__, C.__, C.__, C.__,
                C.__, C.__, C.__, C.__,
                C.__, C.__, C.__, C.__,
                C.__, C.__, C.__, C.__
            });

        public static readonly TileInfo EmptyTileV1 = new TileInfo(TileValue.EmptyTileV1, new StringBuilder()
                .Append("~~~~")
                .Append("~##~")
                .Append("~##~")
                .Append("~~~~"),
            new int[]
            {
                C.DC, C.DC, C.DC, C.DC,
                C.DC, C.DM, C.DM, C.DC,
                C.DC, C.DM, C.DM, C.DC,
                C.DC, C.DC, C.DC, C.DC
            });

        public static readonly TileInfo EmptyTileV2 = new TileInfo(TileValue.EmptyTileV2, new StringBuilder()
                .Append("^^^^")
                .Append("^##^")
                .Append("^##^")
                .Append("^^^^"),
            new int[]
            {
                C.DC, C.DC, C.DC, C.DC,
                C.DC, C.DM, C.DM, C.DC,
                C.DC, C.DM, C.DM, C.DC,
                C.DC, C.DC, C.DC, C.DC
            });

        public static readonly TileInfo SelectedTileRed = new TileInfo(TileValue.SelectedTileRed, new StringBuilder()
                .Append("~~~~")
                .Append("~@@~")
                .Append("~@@~")
                .Append("~~~~"),
            new int[]
            {
                C.DC, C.DC, C.DC, C.DC,
                C.DC, C.DR, C.DR, C.DC,
                C.DC, C.DR, C.DR, C.DC,
                C.DC, C.DC, C.DC, C.DC
            });
        
        public static readonly TileInfo SelectedTileGreen = new TileInfo(TileValue.SelectedTileGreen, new StringBuilder()
                .Append("~~~~")
                .Append("~@@~")
                .Append("~@@~")
                .Append("~~~~"),
            new int[]
            {
                C.DC, C.DC, C.DC, C.DC,
                C.DC, C._G, C._G, C.DC,
                C.DC, C._G, C._G, C.DC,
                C.DC, C.DC, C.DC, C.DC
            });

        public static readonly TileInfo Ship = new TileInfo(TileValue.Ship, new StringBuilder()
                .Append("~~~~")
                .Append("~XX~")
                .Append("~XX~")
                .Append("~~~~"),
            new int[]
            {
                C.DC, C.DC, C.DC, C.DC,
                C.DC, C._G, C._G, C.DC,
                C.DC, C._G, C._G, C.DC,
                C.DC, C.DC, C.DC, C.DC
            });

        public static readonly TileInfo ImpossibleShip = new TileInfo(TileValue.ImpossibleShip, new StringBuilder()
                .Append("~~~~")
                .Append("~XX~")
                .Append("~XX~")
                .Append("~~~~"),
            new int[]
            {
                C.DC, C.DC, C.DC, C.DC,
                C.DC, C.DR, C.DR, C.DC,
                C.DC, C.DR, C.DR, C.DC,
                C.DC, C.DC, C.DC, C.DC
            });

        public static readonly TileInfo ImpossibleShipHitbox = new TileInfo(TileValue.ImpossibleShipHitbox, new StringBuilder()
                .Append("~~~~")
                .Append("~XX~")
                .Append("~XX~")
                .Append("~~~~"),
            new int[]
            {
                C.DC, C.DC, C.DC, C.DC,
                C.DC, C._Y, C._Y, C.DC,
                C.DC, C._Y, C._Y, C.DC,
                C.DC, C.DC, C.DC, C.DC
            });
        
        public static readonly TileInfo HitShip = new TileInfo(TileValue.HitShip, new StringBuilder()
                .Append("*~~*")
                .Append("~**~")
                .Append("~**~")
                .Append("*~~*"),
            new int[]
            {
                C.DC, C.DC, C.DC, C.DC,
                C.DC, C.DR, C.DR, C.DC,
                C.DC, C.DR, C.DR, C.DC,
                C.DC, C.DC, C.DC, C.DC
            });

        public static readonly TileInfo HitWater = new TileInfo(TileValue.HitWater, new StringBuilder()
                .Append("~~~~")
                .Append("~^^~")
                .Append("~^^~")
                .Append("~~~~"),
            new int[]
            {
                C.DC, C.DC, C.DC, C.DC,
                C.DC, C._Y, C._Y, C.DC,
                C.DC, C._Y, C._Y, C.DC,
                C.DC, C.DC, C.DC, C.DC
            });

        public static readonly Dictionary<int, TileInfo> Tiles = new Dictionary<int, TileInfo>()
        {
            {EmptyTileV1.exponent, EmptyTileV1},
            {EmptyTileV2.exponent, EmptyTileV2},
            {SelectedTileRed.exponent, SelectedTileRed},
            {SelectedTileGreen.exponent, SelectedTileGreen},
            {Ship.exponent, Ship},
            {ImpossibleShipHitbox.exponent, ImpossibleShipHitbox},
            {ImpossibleShip.exponent, ImpossibleShip},
            {HitShip.exponent, HitShip},
            {HitWater.exponent, HitWater},
            {VoidTile.exponent, VoidTile},
        };


        public class TileInfo
        {
            public readonly int power;
            public readonly int exponent;
            public readonly CharInfo[] charInfoArray;

            public TileInfo(int exponent, StringBuilder sTileSymbols, int[] fgColors)
            {
                this.power = 1 << exponent;
                this.exponent = exponent;
                charInfoArray = new CharInfo[GetHeight() * GetWidth()];
                for (int i = 0; i < GetHeight() * GetWidth(); i++)
                {
                    charInfoArray[i] = new CharInfo(sTileSymbols[i], fgColors[i]);
                }
            }
        }
        
        

        public class CharInfo
        {
            [Obsolete("This needs to be public for serialization. No set, only get!")]
            public char Glyph { get; set; }
            [Obsolete("This needs to be public for serialization. No set, only get!")]
            public int Color { get; set; }
            
            public CharInfo()
            {
                // needed for deserialization
            }

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
    }
}