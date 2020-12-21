using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.CRUDServiceStockPart
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<ServiceStockPart> ServiceStockParts { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ServiceStockParts = await _context.ServiceStockParts
                .Include(s => s.Service)
                .Include(s => s.StockPart).ToListAsync();
        }
    }
}
