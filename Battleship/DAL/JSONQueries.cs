using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Game;
using Game.Model;

namespace DAL
{
    public static class DataManager
    {
        public static bool LoadGameAction(out Game.Model.GameData? data)
        {
            List<string> files = System.IO.Directory.EnumerateFiles(".", "*.json").ToList();
            files = files.Select(f => f.Remove(0, 2)).ToList();
            files = files.Where(f => (f + string.Concat(Enumerable.Repeat(" ", 4))).Substring(0, 4) == "save").ToList();
            for (int i = 0; i < files.Count; i++)
            {
                Console.WriteLine($"{i} - {files[i]}");
            }

            if (files.Count == 0)
            {
                Console.WriteLine("No saves! Press enter.");
            }

            string fileName;
            try
            { 
                var fileNo = Console.ReadLine();
                fileName = files[int.Parse(fileNo!.Trim())];
            }
            catch (Exception)
            {
                data = null;
                return false;
            }


            var jsonString = System.IO.File.ReadAllText(fileName);

            data = GetGameStateFromString(jsonString);
            return true;
        }
        
        public static GameData GetGameStateFromString(string jsonString)
        {
            var dbGameData = JsonSerializer.Deserialize<DbGameData>(jsonString);
            return DbGameData.ToGameModel(dbGameData);
        }

        public static void SaveGameAction(GameData gameData)
        {
            // 2020-10-12
            var defaultName = "save_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            Console.Write($"File name ({defaultName}):");
            string customName = Console.ReadLine();
            string fileName = string.IsNullOrWhiteSpace(customName) ? 
                defaultName : 
                "save_" + customName  + ".json";
            var jsonOptions = new JsonSerializerOptions() { WriteIndented = true };
            DbGameData data = new DbGameData(gameData);
            var serializedGame = JsonSerializer.Serialize(data, jsonOptions);
            
            System.IO.File.WriteAllText(fileName, serializedGame);
        }
    }
}