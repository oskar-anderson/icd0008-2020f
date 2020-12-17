using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Domain;
using Domain.Model;
using Domain.Tile;
using Game;
using RogueSharp;

namespace DAL
{
    [Table(nameof(AppDbContext.GameData))]
    public sealed class DbGameData : AbstractGameData<DbPlayer>
    {
        [StringLength(36)]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "Game_save_" + DateTime.Now.ToString("yyyy-MM-dd");
        public string DateCreated { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public string ActivePlayerID { get; set; } = null!;
        public string InactivePlayerID { get; set; } = null!;
        
        public override DbPlayer ActivePlayer { get; set; } = null!;
        public override DbPlayer InactivePlayer { get; set; } = null!;
        [NotMapped]
        public override List<Sprite> Sprites { get; set; } = null!;

        public string SpritesDbFriendly 
        {
            get => JsonSerializer.Serialize(Sprites);
            set
            {
                if (value != null)
                { 
                    Sprites = JsonSerializer.Deserialize<List<Sprite>>(value);                    
                }
            }
        }

        [NotMapped]
        // Cannot be serialized by System.Text.Json
        public override string[,] Board2D { get; set; } = null!;
        
        [NotMapped]
        public string[][] BoardJagged { get; set; } = null!;
        
        public string BoardDbFriendly 
        {
            get => JsonSerializer.Serialize(BoardJagged);
            set
            {
                if (value != null)
                { 
                    BoardJagged = JsonSerializer.Deserialize<string[][]>(value);                    
                }
            }
        }
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
        public override GameState State { get; set; }
        public override int FrameCount { get; set; }

        public DbGameData()
        {
            // EF uses this
        }

        
        public DbGameData(GameData gameData)
        {
            ActivePlayer = new DbPlayer(gameData.ActivePlayer);
            ActivePlayerID = ActivePlayer.ID;
            InactivePlayer = new DbPlayer(gameData.InactivePlayer);
            InactivePlayerID = InactivePlayer.ID;
            Sprites = gameData.Sprites;
            BoardJagged = gameData.Board2D.ToJaggedArray();
            AllowedPlacementType = gameData.AllowedPlacementType;
            ShipSizes = gameData.ShipSizes;
            State = gameData.State;
            FrameCount = gameData.FrameCount;
        }

        public static GameData ToGameModel(DbGameData gameData)
        {
            var game = new GameData()
            {
                ActivePlayer = DbPlayer.ToGameModel(gameData.ActivePlayer),
                InactivePlayer = DbPlayer.ToGameModel(gameData.InactivePlayer),
                Sprites = gameData.Sprites,
                Board2D = gameData.BoardJagged.To2DArray(
                    gameData.BoardJagged.Length,
                    gameData.BoardJagged[0].Length),
                AllowedPlacementType = gameData.AllowedPlacementType,
                ShipSizes = gameData.ShipSizes,
                State = gameData.State,
                FrameCount = gameData.FrameCount
            };
            return game;
        }
    }


    [Table(nameof(AppDbContext.Player))]
    public sealed class DbPlayer : AbstractPlayer
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
        public string ShootingHistoryDbFriendly 
        {
            get => JsonSerializer.Serialize(ShootingHistory);
            set
            {
                if (value != null)
                { 
                    ShootingHistory = JsonSerializer.Deserialize<List<ShootingHistoryItem>>(value);
                }
            }
        }

        public string SpriteDbFriendly
        {
            get => JsonSerializer.Serialize(Sprite);
            set
            {
                if (value != null)
                { 
                    Sprite = JsonSerializer.Deserialize<Sprite.PlayerSprite>(value);
                }
            }
        }
        
        public string BoardBoundsDbFriendly
        {
            get => JsonSerializer.Serialize(BoardBounds);
            set
            {
                if (value != null)
                { 
                    BoardBounds = JsonSerializer.Deserialize<Rectangle>(value);
                }
            }
        }


        [NotMapped] 
        public override List<Rectangle> Ships { get; set; } = null!;

        [NotMapped]
        public override List<ShootingHistoryItem> ShootingHistory { get; set; } = new List<ShootingHistoryItem>();

        [NotMapped] 
        public override Sprite.PlayerSprite Sprite { get; set; } = null!;
        
        [NotMapped]
        public override Rectangle BoardBounds { get; set; }

        public override int ShipBeingPlacedIdx { get; set; }
        public override bool IsHorizontalPlacement { get; set; } = true;
        public override string Name { get; set; } = null!;
        public override int PlayerType { get; set; }
        public override float fCameraPixelPosY { get; set; } = 0.0f;
        public override float fCameraPixelPosX { get; set; } = 0.0f;
        public override float fCameraScaleX { get; set; } = 1.0f;
        public override float fCameraScaleY { get; set; } = 1.0f;
        public override float fMouseSelectedTileX { get; set; } = 0.0f;
        public override float fMouseSelectedTileY { get; set; } = 0.0f;
        
        public DbPlayer()
        {
            // EF uses this
        }

        public DbPlayer(Player player)
        {
            Ships = player.Ships;
            ShootingHistory = player.ShootingHistory;
            Sprite = player.Sprite;
            BoardBounds = player.BoardBounds;
            ShipBeingPlacedIdx = player.ShipBeingPlacedIdx;
            IsHorizontalPlacement = player.IsHorizontalPlacement;
            Name = player.Name;
            PlayerType = player.PlayerType;
            fCameraPixelPosY = player.fCameraPixelPosY;
            fCameraPixelPosX = player.fCameraPixelPosX;
            fCameraScaleX = player.fCameraScaleX;
            fCameraScaleY = player.fCameraScaleY;
            fMouseSelectedTileX = player.fMouseSelectedTileX;
            fMouseSelectedTileY = player.fMouseSelectedTileY;
        }
        
        public static Player ToGameModel(DbPlayer player)
        {
            var result = new Player()
            {
                Ships = player.Ships,
                ShootingHistory = player.ShootingHistory,
                Sprite = player.Sprite,
                BoardBounds = player.BoardBounds,
                ShipBeingPlacedIdx = player.ShipBeingPlacedIdx,
                IsHorizontalPlacement = player.IsHorizontalPlacement,
                Name = player.Name,
                PlayerType = player.PlayerType,
                fCameraPixelPosY = player.fCameraPixelPosY,
                fCameraPixelPosX = player.fCameraPixelPosX,
                fCameraScaleX = player.fCameraScaleX,
                fCameraScaleY = player.fCameraScaleY,
                fMouseSelectedTileX = player.fMouseSelectedTileX,
                fMouseSelectedTileY = player.fMouseSelectedTileY,
            };
            return result;
        }
    }
    
}