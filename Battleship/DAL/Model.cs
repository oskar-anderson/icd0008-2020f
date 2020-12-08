using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Domain;
using Domain.Model;
using Game;
using RogueSharp;

namespace DAL
{
    [Table(nameof(AppDbContext.GameData))]
    public sealed class DbGameData : AbstractGameData<DbPlayerDTO>
    {
        [StringLength(36)]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "Game_save_" + DateTime.Now.ToString("yyyy-MM-dd");
        public string DateCreated { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public string ActivePlayerID { get; set; } = null!;
        public string InactivePlayerID { get; set; } = null!;
        
        public override DbPlayerDTO ActivePlayer { get; set; } = null!;
        public override DbPlayerDTO InactivePlayer { get; set; } = null!;
        public override int AllowedPlacementType { get; set; }
        [NotMapped]
        public override List<Point> ShipSizes { get; set; } = null!;
        
        public string ShipSizesDbFriendly 
        {
            get => JsonSerializer.Serialize(ShipSizes);
            set
            {
                if (value != null)
                { 
                    ShipSizes = JsonSerializer.Deserialize<List<Point>>(value);                    
                }
            }
        }
        public override int Phase { get; set; }
        public override string? WinningPlayer { get; set; }
        public override int FrameCount { get; set; }

        public DbGameData()
        {
            // EF uses this
        }

        
        public DbGameData(GameData gameData)
        {
            ActivePlayer = new DbPlayerDTO(gameData.ActivePlayer);
            ActivePlayerID = ActivePlayer.ID;
            InactivePlayer = new DbPlayerDTO(gameData.InactivePlayer);
            InactivePlayerID = InactivePlayer.ID;
            AllowedPlacementType = gameData.AllowedPlacementType;
            ShipSizes = gameData.ShipSizes;
            Phase = gameData.Phase;
            WinningPlayer = gameData.WinningPlayer;
            FrameCount = gameData.FrameCount;
        }

        public static GameData ToGameModel(DbGameData gameData)
        {
            var game = new GameData()
            {
                ActivePlayer = DbPlayerDTO.ToGameModel(gameData.ActivePlayer, 
                    gameData.ActivePlayer.Board.Length, 
                    gameData.ActivePlayer.Board[0].Length),
                InactivePlayer = DbPlayerDTO.ToGameModel(gameData.InactivePlayer, 
                    gameData.ActivePlayer.Board.Length, 
                    gameData.ActivePlayer.Board[0].Length),
                AllowedPlacementType = gameData.AllowedPlacementType,
                ShipSizes = gameData.ShipSizes,
                Phase = gameData.Phase,
                WinningPlayer = gameData.WinningPlayer,
                FrameCount = gameData.FrameCount
            };
            return game;
        }
    }


    [Table(nameof(AppDbContext.Player))]
    public sealed class DbPlayerDTO : AbstractPlayer
    {
        [StringLength(36)]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public string ShipsDbFriendly 
        {
            get => JsonSerializer.Serialize(Ships);
            set
            {
                if (value != null)
                { 
                    Ships = JsonSerializer.Deserialize<List<Rectangle>>(value);                    
                }
            }
        }
        public string BoardDbFriendly 
        {
            get => JsonSerializer.Serialize(Board);
            set
            {
                if (value != null)
                { 
                    Board = JsonSerializer.Deserialize<int[][]>(value);                    
                }
            }
        }
        public string PPlayerDbFriendly 
        {
            get => JsonSerializer.Serialize(PPlayer);
            set
            {
                if (value != null)
                { 
                    PPlayer = JsonSerializer.Deserialize<Point>(value);                    
                }
            }
        }
        public string ShootingHistoryDbFriendly 
        {
            get => JsonSerializer.Serialize(ShootingHistory);
            set
            {
                if (value != null)
                { 
                    ShootingHistory = JsonSerializer.Deserialize<Stack<ShootingHistoryItem>>(value);
                }
            }
        }


        [NotMapped] 
        public override List<Rectangle> Ships { get; set; } = null!;
        [NotMapped]
        public int[][] Board { get; set; } = null!;

        [NotMapped]
        public override Stack<ShootingHistoryItem> ShootingHistory { get; set; } = new Stack<ShootingHistoryItem>();

        [NotMapped]
        public override Point PPlayer { get; set; }

        public override int ShipBeingPlacedIdx { get; set; }
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
        
        public DbPlayerDTO()
        {
            // EF uses this
        }

        public DbPlayerDTO(Player player)
        {
            Ships = player.Ships;
            Board = player.Board.ToJaggedArray();
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
        
        public static Player ToGameModel(DbPlayerDTO player, int boardHeight, int boardWidth)
        {
            var result = new Player()
            {
                Ships = player.Ships,
                Board = player.Board.To2DArray(boardWidth, boardHeight),
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