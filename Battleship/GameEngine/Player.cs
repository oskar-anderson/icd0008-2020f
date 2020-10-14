using System.Collections.Generic;
using ConsoleGameEngineCore;

namespace GameEngine
{
    public class Player
    {
        public readonly UpdateLogic UpdateLogic;
        public readonly List<string?> Ships;
        public TileValue[] Board;
        public string Name;
        
        public float fScaleX = 1.0f;
        public float fScaleY = 1.0f;

        public float fSelectedTileX = 0.0f;
        public float fSelectedTileY = 0.0f;
        
        public bool IsViewingOwnBoard = true;

        public Point pPlayer;

        
        public List<(Point, TileValue)> BoardHover = new List<(Point, TileValue)>();

        public Player(UpdateLogic updateLogic, List<string?> ships, TileValue[] board, Point _pPlayer, string name)
        {
            UpdateLogic = updateLogic;
            Ships = ships;
            Board = board;
            pPlayer = _pPlayer;
            Name = name;
        }

        private void setScreenOffset()
        {
            UpdateLogic.fOffsetX = pPlayer.X * Tile.Width - 4 * Tile.Width;
            UpdateLogic.fOffsetY = pPlayer.Y * Tile.Height - 4 * Tile.Height;
        }
    }
}