﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Game;
using Game.Model;
using RogueSharp;

namespace DAL
{
    public sealed class DbGameData : IGameData<DbPlayerDTO>
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "Game_save_" + DateTime.Now.ToString("yyyy-MM-dd");
        public string DateCreated { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public string ActivePlayerID { get; set; } = null!;
        public string InactivePlayerID { get; set; } = null!;
        
        public DbPlayerDTO ActivePlayer { get; set; } = null!;
        public DbPlayerDTO InactivePlayer { get; set; } = null!;
        public int BoardWidth { get; set; }
        public int BoardHeight { get; set; }
        public int AllowedPlacementType { get; set; }
        [NotMapped]
        public List<Point> ShipSizes { get; set; } = null!;
        
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
        public int Phase { get; set; }
        public string? WinningPlayer { get; set; }

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
            BoardWidth = gameData.BoardWidth;
            BoardHeight = gameData.BoardHeight;
            AllowedPlacementType = gameData.AllowedPlacementType;
            ShipSizes = gameData.ShipSizes;
            Phase = gameData.Phase;
            WinningPlayer = gameData.WinningPlayer;
        }

        public static GameData ToGameModel(DbGameData gameData)
        {
            var game = new GameData()
            {
                ActivePlayer = DbPlayerDTO.ToGameModel(gameData.ActivePlayer, gameData.BoardHeight, gameData.BoardWidth),
                InactivePlayer = DbPlayerDTO.ToGameModel(gameData.InactivePlayer, gameData.BoardHeight, gameData.BoardWidth),
                BoardWidth = gameData.BoardWidth,
                BoardHeight = gameData.BoardHeight,
                AllowedPlacementType = gameData.AllowedPlacementType,
                ShipSizes = gameData.ShipSizes,
                Phase = gameData.Phase,
                WinningPlayer = gameData.WinningPlayer
            };
            return game;
        }
    }


    public sealed class DbPlayerDTO : IPlayer
    {
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
        public List<Rectangle> Ships { get; set; } = null!;
        [NotMapped]
        public int[][] Board { get; set; } = null!;

        [NotMapped]
        public Stack<ShootingHistoryItem> ShootingHistory { get; set; } = new Stack<ShootingHistoryItem>();

        [NotMapped]
        public Point PPlayer { get; set; }

        public int ShipBeingPlacedIdx { get; set; }
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
                Board = player.Board.ConvertTo2DArray(boardHeight, boardWidth),
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