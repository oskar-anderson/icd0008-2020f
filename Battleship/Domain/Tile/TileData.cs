using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Domain.Tile
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class TileData
    {
        public static readonly string[] SeaTiles = {TextureValue.WaterV1, TextureValue.WaterV2};

        
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

        public const int Width = 4;
        public const int Height = 4;


        public static readonly TileProperty VoidTile = new TileProperty(TextureValue.VoidTile, new StringBuilder()
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
            }, true
            );

        public static readonly TileProperty EmptyTileV1 = new TileProperty(TextureValue.WaterV1, new StringBuilder()
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
            }, false
            );

        public static readonly TileProperty EmptyTileV2 = new TileProperty(TextureValue.WaterV2, new StringBuilder()
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
            }, false
            );

        public static readonly TileProperty ImpossibleShip = new TileProperty(TextureValue.ImpossibleShip, new StringBuilder()
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
            }, false
            );

        public static readonly TileProperty ImpossibleShipHitbox = new TileProperty(TextureValue.ImpossibleShipHitbox, new StringBuilder()
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
            }, false
            );

        public static readonly TileProperty HitWater = new TileProperty(TextureValue.HitWater, new StringBuilder()
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
            }, false
            );

        public static readonly TileProperty HitShip = new TileProperty(TextureValue.HitShip, new StringBuilder()
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
            }, false
        );
            
        public static readonly TileProperty IntactShip = new TileProperty(TextureValue.IntactShip, new StringBuilder()
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
            }, false
        );

        public static readonly Dictionary<string, TileProperty> Tiles = new Dictionary<string, TileProperty>()
        {
            { VoidTile.Value, VoidTile },
            { EmptyTileV1.Value, EmptyTileV1 },
            { EmptyTileV2.Value, EmptyTileV2 },
            { ImpossibleShip.Value, ImpossibleShip },
            { ImpossibleShipHitbox.Value, ImpossibleShipHitbox },
            { HitWater.Value, HitWater },
            { HitShip.Value, HitShip },
            { IntactShip.Value, IntactShip },
            { Sprite.PlayerSprite.SelectedTileGreen.Value, Sprite.PlayerSprite.SelectedTileGreen },
            { Sprite.PlayerSprite.SelectedTileRed.Value, Sprite.PlayerSprite.SelectedTileRed },
        };


        public class TileProperty
        {
            public readonly string Value;
            public readonly CharInfo[] CharInfoArray;
            public readonly bool HasCollision;

            public TileProperty(string value, StringBuilder sbTileSymbols, int[] fgColors, bool hasCollision)
            {
                this.HasCollision = hasCollision;
                this.Value = value;
                this.CharInfoArray = new CharInfo[Height * Width];
                for (int i = 0; i < Height * Width; i++)
                {
                    CharInfoArray[i] = new CharInfo(sbTileSymbols[i], fgColors[i]);
                }
                if (CharInfoArray.Any(x => x == null)) { throw new Exception("Tile content is messed up!");}
                if (CharInfoArray.Length != Width * Height) { throw new Exception("Tile size is messed up!");}
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