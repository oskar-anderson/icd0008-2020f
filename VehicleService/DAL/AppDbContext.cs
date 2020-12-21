using System.Linq;
using Domain;
using Microsoft.EntityFrameworkCore;


namespace DAL
{
    // dotnet ef migrations add DbCreation01 --project DAL --startup-project WebApp --context DAL.AppDbContext
    // dotnet ef database update             --project DAL --startup-project WebApp
    // dotnet ef database drop               --project DAL --startup-project WebApp
    // dotnet ef Migrations remove           --project DAL --startup-project WebApp --context DAL.AppDbContext
    
    // cd WebApp
    // dotnet build
    // dotnet aspnet-codegenerator razorpage -m Service              -dc AppDbContext --useDefaultLayout -outDir Pages/CRUDService --referenceScriptLibraries -f
    // dotnet aspnet-codegenerator razorpage -m ServiceStockPart     -dc AppDbContext --useDefaultLayout -outDir Pages/CRUDServiceStockPart --referenceScriptLibraries -f
    // dotnet aspnet-codegenerator razorpage -m ServiceTicket        -dc AppDbContext --useDefaultLayout -outDir Pages/CRUDServiceTicket --referenceScriptLibraries -f
    // dotnet aspnet-codegenerator razorpage -m StockPart            -dc AppDbContext --useDefaultLayout -outDir Pages/CRUDStockPart --referenceScriptLibraries -f

    
    public class AppDbContext : DbContext
    {
        public DbSet<Service> Services { get; set; } = default!;
        public DbSet<ServiceStockPart> ServiceStockParts { get; set; } = default!;
        public DbSet<ServiceTicket> ServiceTickets { get; set; } = default!;
        public DbSet<StockPart> StockParts { get; set; } = default!;
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
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