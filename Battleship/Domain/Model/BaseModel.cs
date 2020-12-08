using System.Collections.Generic;
using RogueSharp;
using Point = RogueSharp.Point;

namespace Domain.Model
{
    public abstract class AbstractGameData<TPlayer>
    {
        public abstract TPlayer ActivePlayer { get; set; }
        public abstract TPlayer InactivePlayer { get; set; }
        public abstract int AllowedPlacementType { get; set; }
        public abstract List<Point> ShipSizes { get; set; }
        public abstract int Phase { get; set; }
        public abstract string? WinningPlayer { get; set; }
        public abstract int FrameCount { get; set; }
    }
    
    public abstract class AbstractPlayer
    {
        public abstract List<Rectangle> Ships { get; set; }
        public abstract Stack<ShootingHistoryItem> ShootingHistory { get; set; }
        public abstract Point PPlayer { get; set; }
        public abstract int ShipBeingPlacedIdx { get; set; }
        public abstract bool IsViewingOwnBoard { get; set; }
        public abstract bool IsHorizontalPlacement { get; set; }
        public abstract string Name { get; set; }
        public abstract int PlayerType { get; set; }
        public abstract float fOffsetY { get; set; }
        public abstract float fOffsetX { get; set; }
        public abstract float fScaleX { get; set; }
        public abstract float fScaleY { get; set; }
        public abstract float fSelectedTileX { get; set; }
        public abstract float fSelectedTileY { get; set; }

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