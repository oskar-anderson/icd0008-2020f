using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.CRUDServiceStockPart
{
    public class EditModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public EditModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ServiceStockPart ServiceStockPart { get; set; } = default!;
        public SelectList StockParts { get; set; } = default!;
        public SelectList Services { get; set; } = default!;
        
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
            Services = new SelectList(
                _context.Services, 
                nameof(ServiceStockPart.Service.ID), 
                nameof(ServiceStockPart.Service.Name));
            StockParts = new SelectList(
                _context.StockParts, 
                nameof(ServiceStockPart.StockPart.ID), 
                nameof(ServiceStockPart.StockPart.Name));
            return Page();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ServiceStockPart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceStockPartExists(ServiceStockPart.ID))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("./Index");
        }

        private bool ServiceStockPartExists(string id)
        {
            return _context.ServiceStockParts.Any(e => e.ID == id);
        }
    }
}
