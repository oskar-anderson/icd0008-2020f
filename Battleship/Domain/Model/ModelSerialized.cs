using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using RogueSharp;
using Point = RogueSharp.Point;

namespace Domain.Model
{
    public sealed class GameDataSerializable : AbstractGameData<PlayerSerializable>
    {
        public override PlayerSerializable ActivePlayer { get; set; } = null!;
        public override PlayerSerializable InactivePlayer { get; set; } = null!;
        public override int AllowedPlacementType { get; set; }
        public override List<Point> ShipSizes { get; set; } = null!;
        public override int Phase { get; set; }
        public override string? WinningPlayer { get; set; }
        public override int FrameCount { get; set; }

        public GameDataSerializable()
        {
            // Serialization uses this
        }

        public GameDataSerializable(GameData gameData)
        {
            ActivePlayer = new PlayerSerializable(gameData.ActivePlayer);
            InactivePlayer = new PlayerSerializable(gameData.InactivePlayer);
            AllowedPlacementType = gameData.AllowedPlacementType;
            ShipSizes = gameData.ShipSizes;
            Phase = gameData.Phase;
            WinningPlayer = gameData.WinningPlayer;
            FrameCount = gameData.FrameCount;
        }

        public static GameData ToGameModelSerializable(GameDataSerializable gameData)
        {
            var game = new GameData()
            {
                ActivePlayer = PlayerSerializable.ToGameModelSerializable(gameData.ActivePlayer, 
                    gameData.ActivePlayer.BoardSerializationFriendly.Length, 
                    gameData.ActivePlayer.BoardSerializationFriendly[0].Length),
                InactivePlayer = PlayerSerializable.ToGameModelSerializable(gameData.InactivePlayer, 
                    gameData.ActivePlayer.BoardSerializationFriendly.Length, 
                    gameData.ActivePlayer.BoardSerializationFriendly[0].Length),
                AllowedPlacementType = gameData.AllowedPlacementType,
                ShipSizes = gameData.ShipSizes,
                Phase = gameData.Phase,
                WinningPlayer = gameData.WinningPlayer,
                FrameCount = gameData.FrameCount
            };
            return game;
        }
    }

    public sealed class PlayerSerializable : AbstractPlayer
    {
        public override List<Rectangle> Ships { get; set; } = null!;
        public int[][] BoardSerializationFriendly { get; set; } = null!;
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
        
        public PlayerSerializable()
        {
            // Serialization uses this
        }

        public PlayerSerializable(Player player)
        {
            Ships = player.Ships;
            BoardSerializationFriendly = player.Board.ToJaggedArray();
            ShootingHistory = player.ShootingHistory;
            PPlayer = player.PPlayer;
            ShipBeingPlacedIdx = player.ShipBeingPlacedIdx;
            IsViewingOwnBoard = player.IsViewingOwnBoard;
            IsHorizontalPlacement = player.IsHorizontalPlacement;
            Name = player.Name;
            PlayerType = player.PlayerType;
            fOffsetY = player.fOffsetY;
            fOffsetX = player.fOffsetX;
            fScaleX = player.fScaleX;
            fScaleY = player.fScaleY;
            fSelectedTileX = player.fSelectedTileX;
            fSelectedTileY = player.fSelectedTileY;
        }
        
        public static Player ToGameModelSerializable(PlayerSerializable player, int boardHeight, int boardWidth)
        {
            var result = new Player()
            {
                Ships = player.Ships,
                Board = player.BoardSerializationFriendly.To2DArray(boardWidth, boardHeight),
                ShootingHistory = player.ShootingHistory,
                PPlayer = player.PPlayer,
                ShipBeingPlacedIdx = player.ShipBeingPlacedIdx,
                IsViewingOwnBoard = player.IsViewingOwnBoard,
                IsHorizontalPlacement = player.IsHorizontalPlacement,
                Name = player.Name,
                PlayerType = player.PlayerType,
                fOffsetY = player.fOffsetY,
                fOffsetX = player.fOffsetX,
                fScaleX = player.fScaleX,
                fScaleY = player.fScaleY,
                fSelectedTileX = player.fSelectedTileX,
                fSelectedTileY = player.fSelectedTileY,
            };
            return result;
        }
    }
}