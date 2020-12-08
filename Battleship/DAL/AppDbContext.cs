using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    // dotnet ef migrations add DbCreation01 --project DAL --startup-project ConsoleApp --context DAL.AppDbContext
    // dotnet ef database update             --project DAL --startup-project ConsoleApp
    // dotnet ef database drop               --project DAL --startup-project ConsoleApp
    // dotnet ef Migrations remove           --project DAL --startup-project ConsoleApp --context DAL.AppDbContext
    
    // cd WebApp
    // dotnet aspnet-codegenerator razorpage -m DbGameData -dc AppDbContext --useDefaultLayout -outDir Pages/GameDataCRUD --referenceScriptLibraries -f
    // dotnet aspnet-codegenerator razorpage -m DbPlayerDTO -dc AppDbContext --useDefaultLayout -outDir Pages/PlayerCRUD --referenceScriptLibraries -f

    
    public class AppDbContext : DbContext
    {
        public DbSet<DbGameData> GameData { get; set; } = default!;
        public DbSet<DbPlayerDTO> Player { get; set; } = default!;
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (true)
            {
                optionsBuilder
                    .EnableSensitiveDataLogging()
                    .UseSqlServer(@"
                        Server=barrel.itcollege.ee,1533;
                        User Id=student;
                        Password=Student.Bad.password.0;
                        Database=kaande_battle;
                        MultipleActiveResultSets=true;
                        "
                    );
            }
            else
            {
                // Directory changes on run - db path should be saved and loaded separately
                // D:\Programming\C#\2020\Project\Battleship
                // var DbPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
                //optionsBuilder.UseSqlite($"Data Source={DbPath}/Battleship.db");
                optionsBuilder.UseSqlite($"Data Source=D:\\Programming\\C#\\2020\\Project\\Battleship/Battleship.db");
            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .Where(e => !e.IsOwned())
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}