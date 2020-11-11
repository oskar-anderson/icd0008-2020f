using System.Linq;
using Game.Model;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public static class DbQueries
    {
        public static void SaveMainDb(GameData gameData)
        {
            // https://stackoverflow.com/questions/729527/is-it-possible-to-assign-a-base-class-object-to-a-derived-class-reference-with-a
            // ModelGameDTO gameDataDb = JsonConvert.DeserializeObject<ModelGameDTO>(JsonConvert.SerializeObject(gameData));
            var dbGameData = new DbGameData(gameData);
            PostGame(dbGameData);
            DeleteAllButOneGame(dbGameData.ID);
        }

        public static bool TryGetGameWithIdx(int idx, ref GameData gameDataState)
        {
            if (idx < 0) { return false; }
            
            DbGameData[] saves = GetAllGames();
            if (saves.Length > idx)
            {
                var logicGameData = DbGameData.ToGameModel(saves[idx]);
                gameDataState = logicGameData;
                return true;
            }

            return false;
        }
        
        public static int SavesAmount()
        {
            using AppDbContext ctx = new AppDbContext();
            return ctx.StateSave.Count();
        }
        
        private static void PostGame(DbGameData gameData)
        {
            using AppDbContext ctx = new AppDbContext();
            ctx.StateSave.Add(gameData);
            ctx.Player.Add(gameData.ActivePlayer);
            ctx.Player.Add(gameData.InactivePlayer);
            ctx.SaveChanges();
        }
        
        private static DbGameData[] GetAllGames()
        {
            using AppDbContext ctx = new AppDbContext();
            DbGameData[] states = ctx.StateSave
                .OrderByDescending(ss => ss.DateCreated)
                .Include(ss => ss.ActivePlayer)
                .Include(ss => ss.InactivePlayer)
                .ToArray();
            return states.ToArray<DbGameData>();
            
        }

        private static void DeleteAllButOneGame(string id)
        {
            var allSaves = GetAllGames();
            using AppDbContext ctx = new AppDbContext();
            foreach (var save in allSaves)
            {
                if (save.ID != id)
                {
                    ctx.Remove(save);
                }
            }

            ctx.SaveChanges();
            
        }
    }
}