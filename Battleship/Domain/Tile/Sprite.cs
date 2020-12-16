using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueSharp;
using static Domain.Tile.TileData;

namespace Domain.Tile
{
    public class Sprite
    {
        public bool HasCollision { get; set; }
        public string Texture { get; set; } = null!;
        public string Type { get; set; } = null!;
        public Point Pos { get; set; }
        
        public Sprite()
        {
            // Serialization requirement
        }
        
        
        private static class SpriteTextureValue
        {
            public const string SelectedTileRed =       "pVae";
            public const string SelectedTileGreen =     "ZLzo";
        }
        
        public sealed class PlayerSprite : Sprite
        {
            public PlayerSprite()
            {
                // Serialization requirement
            }
            
            public PlayerSprite(Point pos)
            {
                HasCollision = false;
                Texture = SelectedTileGreen.Value;
                Type = nameof(PlayerSprite);
                Pos = pos;
            }

            public void SetSpriteToSelectedTileRed() { Texture = SelectedTileRed.Value; }
            public void SetSpriteToSelectedTileGreen() { Texture = SelectedTileGreen.Value; }
            
            public static readonly TileProperty SelectedTileRed = new TileProperty(SpriteTextureValue.SelectedTileRed, new StringBuilder()
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
                }, false
                );
            
            public static readonly TileProperty SelectedTileGreen = new TileProperty(SpriteTextureValue.SelectedTileGreen, new StringBuilder()
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
                }, false
                );
        }
        
    }
}