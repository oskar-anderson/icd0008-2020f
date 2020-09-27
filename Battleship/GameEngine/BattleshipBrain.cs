using System;

namespace GameEngine
{
    public class BattleshipBrain
    {
        public string[,] Board { get; }

        public int BoardWidth { get; }
        public int BoardHeight { get; }
        public bool AllowAdjacentPlacement { get; }
        public bool OppIsAi { get; }
        public int AiDifficulty { get; }
        
        public string ActivePlayerName = "";

        public BattleshipBrain(int boardHeight, int boardWidth, bool allowAdjacentPlacement, bool oppIsAi, int aiDifficulty)
        {
            BoardHeight = boardHeight;
            BoardWidth = boardWidth;
            AllowAdjacentPlacement = allowAdjacentPlacement;
            OppIsAi = oppIsAi;
            AiDifficulty = aiDifficulty;
            
            // initialize the board
            Board = new string[boardHeight, boardWidth];
            
            Console.WriteLine("Hello from Battleship brain!");
        }

    }
}