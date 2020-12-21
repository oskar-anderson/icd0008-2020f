using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.CRUDServiceTicket
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<ServiceTicket> ServiceTickets { get;set; } = default!;
        public Dictionary<string, List<ServiceStockPart>> ServiceStockParts { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ServiceTickets = await _context.ServiceTickets
                .Include(s => s.Service).ToListAsync();
            List<ServiceStockPart> serviceStockParts = await _context.ServiceStockParts
                .Include(x => x.StockPart)
                .ToListAsync();
            ServiceStockParts = new Dictionary<string, List<ServiceStockPart>>();
            foreach (var serviceTicket in ServiceTickets)
            {
                ServiceStockParts[serviceTicket.ServiceID] =
                    serviceStockParts.Where(x => x.ServiceID == serviceTicket.ServiceID).ToList();
            }
            
        }
    }
}
