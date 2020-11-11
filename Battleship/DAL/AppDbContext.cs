using Microsoft.EntityFrameworkCore;

namespace DAL
{
    // dotnet ef migrations add DbCreation01 --project DAL --startup-project ConsoleApp --context DAL.AppDbContext
    // dotnet ef database update             --project DAL --startup-project ConsoleApp
    // dotnet ef database drop               --project DAL --startup-project ConsoleApp
    // dotnet ef Migrations remove           --project DAL --startup-project ConsoleApp --context DAL.AppDbContext
    public class AppDbContext : DbContext
    {
        public DbSet<DbGameData> StateSave { get; set; } = default!;
        public DbSet<DbPlayerDTO> Player { get; set; } = default!;
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (false)
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
    }
}