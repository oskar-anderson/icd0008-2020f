using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Tile;
using RogueSharp;
using Point = RogueSharp.Point;

namespace Domain.Model
{
    public sealed class GameData : AbstractGameData<Player>
    {
        public override Player ActivePlayer { get; set; } = null!;
        public override Player InactivePlayer { get; set; } = null!;
        public override string[,] Board2D { get; set; } = null!;
        public override List<Sprite> Sprites { get; set; } = null!;
        public override int AllowedPlacementType { get; set; }
        public override List<Point> ShipSizes { get; set; } = null!;
        public override GameState State { get; set; } = GameState.Placement;
        public override int FrameCount { get; set; }

        public GameData()
        {
            // Needed for curly bracket init
        }
        public GameData(
            int allowedPlacementType, string[,] board, List<Point> shipSizes, 
            Player activePlayer, Player inactivePlayer, List<Sprite> sprites)
        {
            AllowedPlacementType = allowedPlacementType;
            Board2D = board;
            ShipSizes = shipSizes;
            ActivePlayer = activePlayer;
            InactivePlayer = inactivePlayer;
            Sprites = sprites;
        }
    }

    public sealed class Player : AbstractPlayer
    {
        public override List<Rectangle> Ships { get; set; } = new List<Rectangle>();
        public override Stack<ShootingHistoryItem> ShootingHistory { get; set; } = new Stack<ShootingHistoryItem>();
        public override Sprite.PlayerSprite Sprite { get; set; } = null!;
        public override Rectangle BoardBounds { get; set; }
        public override int ShipBeingPlacedIdx { get; set; } = 0;
        public override bool IsHorizontalPlacement { get; set; } = true;
        public override string Name { get; set; } = null!;
        public override int PlayerType { get; set; }
        public override float fCameraPixelPosY { get; set; }
        public override float fCameraPixelPosX { get; set; }
        public override float fCameraScaleX { get; set; } = 1.0f;
        public override float fCameraScaleY { get; set; } = 1.0f;
        public override float fMouseSelectedTileX { get; set; } = 0.0f;
        public override float fMouseSelectedTileY { get; set; } = 0.0f;

        // Things not important enough to save
        public Point pMouseStartPixelPan { get; set; }
        public float fKeyboardMoveTimeout { get; set; } = -1f;
        public string UI_Message { get; set; } = string.Empty;
        public List<DialogItem> UI_DialogOptions { get; set; } = new List<DialogItem>();
        
        public struct DialogItem
        {
            public bool isActive;
            public readonly string key;
            public readonly string text;

            public DialogItem(bool isActive, string key, string text)
            {
                this.isActive = isActive;
                this.key = key;
                this.text = text;
            }
        }

        public void UI_Reset()
        {
            this.UI_Message = String.Empty;
            this.UI_DialogOptions = new List<DialogItem>();;
        }
        
        public void UI_Swap(Player player)
        {
            player.UI_Message = UI_Message;
            player.UI_DialogOptions = UI_DialogOptions;
        }

        public struct HoverElement
        {
            public Point Point;
            public readonly string TileExponent;

            public HoverElement(Point point, string tileExponent)
            {
                Point = point;
                TileExponent = tileExponent;
            }
        }
        
        public Player()
        {
            // Needed for curly bracket init
        }
        
        public Player(Rectangle boardBounds, Point pPlayer, int playerType, string name, Point cameraPoint, List<Sprite> sprites)
        {
            BoardBounds = boardBounds;
            PlayerType = playerType;
            Name = name;
            fCameraPixelPosX = cameraPoint.X;
            fCameraPixelPosY = cameraPoint.Y;
            
            Sprite = new Sprite.PlayerSprite(pPlayer);
            sprites.Add(Sprite);
        }
    }
}