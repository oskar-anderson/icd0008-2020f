using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using RogueSharp;
using Point = RogueSharp.Point;

namespace Domain.Model
{
    public sealed class GameData : AbstractGameData<Player>
    {
        public override Player ActivePlayer { get; set; } = null!;
        public override Player InactivePlayer { get; set; } = null!;
        public override int AllowedPlacementType { get; set; }
        public override List<Point> ShipSizes { get; set; } = null!;
        public override int Phase { get; set; }
        public override string? WinningPlayer { get; set; }
        public override int FrameCount { get; set; }

        public GameData()
        {
            // Needed for curly bracket init
        }
        public GameData(int allowedPlacementType, List<Point> shipSizes, int phase, Player activePlayer, Player inactivePlayer)
        {
            AllowedPlacementType = allowedPlacementType;
            ShipSizes = shipSizes;
            Phase = phase;
            ActivePlayer = activePlayer;
            InactivePlayer = inactivePlayer;
        }
    }

    public sealed class Player : AbstractPlayer
    {
        public override List<Rectangle> Ships { get; set; } = null!;
        public int[,] Board { get; set; } = null!;
        public override Stack<ShootingHistoryItem> ShootingHistory { get; set; } = new Stack<ShootingHistoryItem>();
        public override Point PPlayer { get; set; }
        public override int ShipBeingPlacedIdx { get; set; } = 0;
        public override bool IsViewingOwnBoard { get; set; }
        public override bool IsHorizontalPlacement { get; set; } = true;
        public override string Name { get; set; } = null!;
        public override int PlayerType { get; set; }
        public override float fOffsetY { get; set; } = 0.0f;
        public override float fOffsetX { get; set; } = 0.0f;
        public override float fScaleX { get; set; } = 1.0f;
        public override float fScaleY { get; set; } = 1.0f;
        public override float fSelectedTileX { get; set; } = 0.0f;
        public override float fSelectedTileY { get; set; } = 0.0f;

        public struct HoverElement
        {
            public Point Point;
            public readonly int TileExponent;

            public HoverElement(Point point, int tileExponent)
            {
                Point = point;
                TileExponent = tileExponent;
            }
        }
        
        public Player()
        {
            // Needed for curly bracket init
        }
        
        public Player(List<Rectangle> ships, int[,] board, Point pPlayer, bool isViewingOwnBoard, bool isHorizontalPlacement, int playerType, string name)
        {
            Ships = ships;
            Board = board;
            PPlayer = pPlayer;
            IsViewingOwnBoard = isViewingOwnBoard;
            IsHorizontalPlacement = isHorizontalPlacement;
            PlayerType = playerType;
            Name = name;

            ShipBeingPlacedIdx = ships.Count == 0 ? 0 : -1;
        }
    }
}