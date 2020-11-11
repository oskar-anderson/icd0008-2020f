using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using RogueSharp;
using Point = RogueSharp.Point;

namespace Game.Model
{
    public sealed class GameData : IGameData<Player>
    {
        public Player ActivePlayer { get; set; } = null!;
        public Player InactivePlayer { get; set; } = null!;
        public int BoardWidth { get; set; }
        public int BoardHeight { get; set; }
        public int AllowedPlacementType { get; set; }
        public List<Point> ShipSizes { get; set; } = null!;
        public int Phase { get; set; }
        public string? WinningPlayer { get; set; }

        public GameData()
        {
            // Needed for curly bracket init
        }
        public GameData(int boardWidth, int boardHeight, int allowedPlacementType, List<Point> shipSizes, int phase, Player activePlayer, Player inactivePlayer)
        {
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            AllowedPlacementType = allowedPlacementType;
            ShipSizes = shipSizes;
            Phase = phase;
            ActivePlayer = activePlayer;
            InactivePlayer = inactivePlayer;
        }
    }

    public sealed class Player : IPlayer
    {
        public List<Rectangle> Ships { get; set; } = null!;
        public int[,] Board { get; set; } = null!;
        public Stack<ShootingHistoryItem> ShootingHistory { get; set; } = new Stack<ShootingHistoryItem>();
        public Point PPlayer { get; set; }
        public int ShipBeingPlacedIdx { get; set; } = 0;
        public bool IsViewingOwnBoard { get; set; }
        public bool IsHorizontalPlacement { get; set; } = true;
        public string Name { get; set; } = null!;
        public int PlayerType { get; set; }
        public float fOffsetY { get; set; } = 0.0f;
        public float fOffsetX { get; set; } = 0.0f;
        public float fScaleX { get; set; } = 1.0f;
        public float fScaleY { get; set; } = 1.0f;
        public float fSelectedTileX { get; set; } = 0.0f;
        public float fSelectedTileY { get; set; } = 0.0f;
        
        [JsonIgnore]
        public List<HoverElement> BoardHover { get; set; } = new List<HoverElement>();

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

            ShipBeingPlacedIdx = ships.Capacity == 0 ? -1 : 0;
        }
    }
}