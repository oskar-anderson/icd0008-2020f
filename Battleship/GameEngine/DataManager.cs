using System;
using System.Linq;
using System.Text.Json;

namespace GameEngine
{
    public class DataManager
    {

        public static void LoadGameAction()
        {
            var files = System.IO.Directory.EnumerateFiles(".", "*.json").ToList();
            for (int i = 0; i < files.Count; i++)
            {
                Console.WriteLine($"{i} - {files[i]}");
            }

            var fileNo = Console.ReadLine();
            var fileName = files[int.Parse(fileNo!.Trim())];

            var jsonString = System.IO.File.ReadAllText(fileName);

            SetGameStateFromString(jsonString);
        }
        
        public static void SetGameStateFromString(string jsonString)
        {
            GameDTO gameDTO = JsonSerializer.Deserialize<GameDTO>(jsonString);

            Game.BoardWidth = gameDTO.BoardWidth;
            Game.BoardHeight = gameDTO.BoardHeight;
            Game.AllowAdjacentPlacement = gameDTO.AllowAdjacentPlacement;
            Game.OppIsAi = gameDTO.OppIsAi;
            Game.AiDifficulty = gameDTO.AiDifficulty;
            
            Game.Phase = gameDTO.Phase;
            
            Game.ScreenWidth = gameDTO.ScreenWidth;
            Game.ScreenHeight = gameDTO.ScreenHeight;
            
            Game.ActivePlayer = GameDTO.GetDeserializedPlayer(gameDTO.ActivePlayerDTO);
            Game.InactivePlayer = GameDTO.GetDeserializedPlayer(gameDTO.InactivePlayerDTO);
        }

        public static void SaveGameAction(GameDTO gameDTO)
        {
            // 2020-10-12
            var defaultName = "save_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            Console.Write($"File name ({defaultName}):");
            string customName = Console.ReadLine();
            string fileName;
            if (string.IsNullOrWhiteSpace(customName))
            {
                fileName = defaultName;
            }
            else
            {
                fileName = "save_" + customName  + ".json";
            }

            var jsonOptions = new JsonSerializerOptions() { WriteIndented = true };
            var serializedGame = JsonSerializer.Serialize(gameDTO, jsonOptions);
            
            System.IO.File.WriteAllText(fileName, serializedGame);
        }
    }
}