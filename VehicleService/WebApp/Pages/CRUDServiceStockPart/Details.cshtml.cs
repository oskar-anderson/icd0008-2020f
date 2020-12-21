using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.CRUDServiceStockPart
{
    public class DetailsModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DetailsModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public ServiceStockPart ServiceStockPart { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ServiceStockPart = await _context.ServiceStockParts
                .Include(s => s.Service)
                .Include(s => s.StockPart).FirstOrDefaultAsync(m => m.ID == id);

            if (ServiceStockPart == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
