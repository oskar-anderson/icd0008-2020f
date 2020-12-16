using System.Collections.Generic;
using Domain.Tile;
using RogueSharp;
using Point = RogueSharp.Point;

namespace Domain.Model
{
    public abstract class AbstractGameData<TPlayer>
    {
        public abstract TPlayer ActivePlayer { get; set; }
        public abstract TPlayer InactivePlayer { get; set; }
        public abstract List<Sprite> Sprites { get; set; }
        public abstract string[,] Board2D { get; set; }
        public abstract int AllowedPlacementType { get; set; }
        public abstract List<Point> ShipSizes { get; set; }
        public abstract GameState State { get; set; }
        public abstract int FrameCount { get; set; }
    }

    public enum GameState
    {
        Placement,
        Shooting,
        GameOver,
    }
    
    public abstract class AbstractPlayer
    {
        public abstract List<Rectangle> Ships { get; set; }
        public abstract Stack<ShootingHistoryItem> ShootingHistory { get; set; }
        public abstract Sprite.PlayerSprite Sprite { get; set; }
        public abstract Rectangle BoardBounds { get; set; }
        public abstract int ShipBeingPlacedIdx { get; set; }
        public abstract bool IsHorizontalPlacement { get; set; }
        public abstract string Name { get; set; }
        public abstract int PlayerType { get; set; }
        public abstract float fCameraPixelPosY { get; set; }
        public abstract float fCameraPixelPosX { get; set; }
        public abstract float fCameraScaleX { get; set; }
        public abstract float fCameraScaleY { get; set; }
        public abstract float fMouseSelectedTileX { get; set; }
        public abstract float fMouseSelectedTileY { get; set; }
        

    }

    public class ShootingHistoryItem
    {
        public Point Point { get; set; } = Point.Zero;
        public string PrevValue { get; set; } = null!;
        public string CurrValue { get; set; } = null!;
        public List<ShootingHistoryItem>? AllChangedPoints { get; set; } = null;

        public ShootingHistoryItem()
        {
            
        }
        
        public ShootingHistoryItem(Point _point, string _prevValue, string _currValue, List<ShootingHistoryItem>? _allChangedPoints)
        {
            Point = _point;
            PrevValue = _prevValue;
            CurrValue = _currValue;
            AllChangedPoints = _allChangedPoints;
        }
    }
}