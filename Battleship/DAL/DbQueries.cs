using System.Linq;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public static class DbQueries
    {
        public static void SaveAndDeleteOthers(GameData gameData)
        {
            // https://stackoverflow.com/questions/729527/is-it-possible-to-assign-a-base-class-object-to-a-derived-class-reference-with-a
            // ModelGameDTO gameDataDb = JsonConvert.DeserializeObject<ModelGameDTO>(JsonConvert.SerializeObject(gameData));
            var dbGameData = new DbGameData(gameData);
            PostGame(dbGameData);
            DeleteAllButOneGame(dbGameData.ID);
        }
        
        public static string Save(GameData gameData)
        {
            var dbGameData = new DbGameData(gameData);
            return PostGame(dbGameData);
        }
        
        public static void Delete(string id)
        {
            using AppDbContext ctx = new AppDbContext();
            DbGameData? gameData = ctx.GameData
                .Include(ss => ss.ActivePlayer)
                .Include(ss => ss.InactivePlayer)
                .FirstOrDefault(x => x.ID == id);
            if (gameData != null)
            {
                ctx.Remove(gameData);
                ctx.Remove(gameData.ActivePlayer);
                ctx.Remove(gameData.InactivePlayer);
            }
            ctx.SaveChanges();
        }

        public static bool TryGetGameWithIdx(int idx, out GameData? gameDataState)
        {
            if (idx < 0)
            {
                gameDataState = null;
                return false;
            }
            
            DbGameData[] saves = GetAllGames();
            if (saves.Length > idx)
            {
                var logicGameData = DbGameData.ToGameModel(saves[idx]);
                gameDataState = logicGameData;
                return true;
            }
            gameDataState = null;
            return false;
        }
        
        public static int SavesAmount()
        {
            using AppDbContext ctx = new AppDbContext();
            return ctx.GameData.Count();
        }
        
        public static string PostGame(DbGameData gameData)
        {
            using AppDbContext ctx = new AppDbContext();
            ctx.GameData.Add(gameData);
            ctx.Player.Add(gameData.ActivePlayer);
            ctx.Player.Add(gameData.InactivePlayer);
            ctx.SaveChanges();
            return gameData.ID;
        }
        
        public static DbGameData[] GetAllGames()
        {
            using AppDbContext ctx = new AppDbContext();
            DbGameData[] states = ctx.GameData
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
                    ctx.Remove(save.ActivePlayer);
                    ctx.Remove(save.InactivePlayer);
                }
            }

            ctx.SaveChanges();
            
        }

        public static bool DeleteGame(string id)
        {
            using AppDbContext ctx = new AppDbContext();
            DbGameData dbGameData = ctx.GameData
                .Include(d => d.ActivePlayer)
                .Include(d => d.InactivePlayer).FirstOrDefault(x => x.ID == id);
            
            if (dbGameData != null)
            {
                ctx.GameData.Remove(dbGameData);
                ctx.Player.Remove(dbGameData.ActivePlayer);
                ctx.Player.Remove(dbGameData.InactivePlayer);
                ctx.SaveChanges();
                return true;
            }

            return false;
        }
    }
}