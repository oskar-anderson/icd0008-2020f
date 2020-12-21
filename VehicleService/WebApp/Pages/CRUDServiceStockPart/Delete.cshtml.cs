using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.CRUDServiceStockPart
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DeleteModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ServiceStockPart = await _context.ServiceStockParts.FindAsync(id);

            if (ServiceStockPart != null)
            {
                _context.ServiceStockParts.Remove(ServiceStockPart);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
