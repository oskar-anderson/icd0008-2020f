using System.Collections;
using System.Collections.Generic;
using RogueSharp;
using Point = RogueSharp.Point;

namespace Game.Model
{
    public interface IGameData<TPlayer>
    {
        TPlayer ActivePlayer { get; set; }
        TPlayer InactivePlayer { get; set; }
        int BoardWidth { get; set; }
        int BoardHeight { get; set; }
        int AllowedPlacementType { get; set; }
        List<Point> ShipSizes { get; set; }
        int Phase { get; set; }
        string? WinningPlayer { get; set; }
    }
    
    public interface IPlayer
    {
        List<Rectangle> Ships { get; set; }
        Stack<ShootingHistoryItem> ShootingHistory { get; set; }
        Point PPlayer { get; set; }
        int ShipBeingPlacedIdx { get; set; }
        bool IsViewingOwnBoard { get; set; }
        bool IsHorizontalPlacement { get; set; }
        string Name { get; set; }
        int PlayerType { get; set; }
        float fOffsetY { get; set; }
        float fOffsetX { get; set; }
        float fScaleX { get; set; }
        float fScaleY { get; set; }
        float fSelectedTileX { get; set; }
        float fSelectedTileY { get; set; }
        
    }

    public struct ShootingHistoryItem
    {
        public readonly Point point;
        public readonly int prevValue;
        public readonly int currValue;
        public readonly List<ShootingHistoryItem>? allChangedPoints;
        
        public ShootingHistoryItem(Point _point, int _prevValue, int _currValue, List<ShootingHistoryItem>? _allChangedPoints)
        {
            point = _point;
            prevValue = _prevValue;
            currValue = _currValue;
            allChangedPoints = _allChangedPoints;
        }
    }
}