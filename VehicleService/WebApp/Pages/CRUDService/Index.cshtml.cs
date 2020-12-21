using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.CRUDService
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<Service> Services { get;set; } = default!;

        public Dictionary<string, List<ServiceStockPart>> ServiceToServiceStockParts { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Services = await _context.Services.ToListAsync();
            
            List<ServiceStockPart> serviceStockParts = await _context.ServiceStockParts
                .Include(x => x.StockPart)
                .ToListAsync();
            ServiceToServiceStockParts = new Dictionary<string, List<ServiceStockPart>>();
            foreach (var service in Services)
            {
                List<ServiceStockPart> serviceToServiceStockParts = serviceStockParts
                    .Where(x => x.ServiceID == service.ID)
                    .ToList();
                ServiceToServiceStockParts[service.ID] = serviceToServiceStockParts;
            }
        }
    }
}
