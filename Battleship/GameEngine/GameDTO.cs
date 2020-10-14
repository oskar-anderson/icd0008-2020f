using System.Collections.Generic;
using ConsoleGameEngineCore;

namespace GameEngine
{
    public class GameDTO
    {
        public int BoardWidth { get; set; }
        public int BoardHeight { get; set; }
        public bool AllowAdjacentPlacement { get; set; }
        public bool OppIsAi { get; set; }
        public int AiDifficulty { get; set; }

        public int Phase { get; set; }

        public int ScreenWidth { get; set; } = 45;
        public int ScreenHeight { get; set; } = 60;

        public PlayerDTO ActivePlayerDTO { get; set; }
        public PlayerDTO InactivePlayerDTO { get; set; }

        public bool NeedToSwapTurn { get; set; } = false;

        // This class has automatic parameterless constructor needed for deserialization

        public GameDTO Create()
        {
            BoardWidth = Game.BoardWidth;
            BoardHeight = Game.BoardHeight;
            AllowAdjacentPlacement = Game.AllowAdjacentPlacement;
            OppIsAi = Game.OppIsAi;
            AiDifficulty = Game.AiDifficulty;
            
            Phase = Game.Phase;
            
            ScreenWidth = Game.ScreenWidth;
            ScreenHeight = Game.ScreenHeight;

            PlayerDTO activePlayerDTO = new PlayerDTO(
                Game.ActivePlayer.Ships,
                Game.ActivePlayer.Board,
                Game.ActivePlayer.Name,
                Game.ActivePlayer.IsViewingOwnBoard,
                Game.ActivePlayer.pPlayer);
            ActivePlayerDTO = activePlayerDTO;
            PlayerDTO inactivePlayerDTO = new PlayerDTO(
                Game.InactivePlayer.Ships,
                Game.InactivePlayer.Board,
                Game.InactivePlayer.Name,
                Game.InactivePlayer.IsViewingOwnBoard,
                Game.InactivePlayer.pPlayer);
            InactivePlayerDTO = inactivePlayerDTO;

            return this;
        }

        public static Player GetDeserializedPlayer(PlayerDTO playerDTO)
        {
            return new Player(new UpdateLogic(), playerDTO.Ships, playerDTO.Board, playerDTO.pPlayer, playerDTO.Name);
        }
    }

    public class PlayerDTO
    {
        public UpdateLogic UpdateLogic { get; set; }
        public List<string?> Ships { get; set; }
        public TileValue[] Board { get; set; }
        public string Name { get; set; }
        
        // public float fScaleX { get; set; } = 1.0f;
        // public float fScaleY { get; set; } = 1.0f;

        // public float fSelectedTileX { get; set; } = 0.0f;
        // public float fSelectedTileY { get; set; } = 0.0f;

        public bool IsViewingOwnBoard { get; set; } = true;

        public Point pPlayer { get; set; }


        public PlayerDTO()
        {
            // Needed for deserialization
        }
        
        public PlayerDTO(List<string?> ships, TileValue[] board, string name, bool isViewingOwnBoard, Point _pPlayer)
        {
            Ships = ships;
            Board = board;
            Name = name;
            IsViewingOwnBoard = isViewingOwnBoard;
            pPlayer = _pPlayer;
        }
    }
}