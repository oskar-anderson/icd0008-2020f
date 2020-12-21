using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.CRUDServiceStockPart
{
    public class CreateModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public CreateModel(DAL.AppDbContext context)
        {
            _context = context;
        }
        
        public SelectList StockParts { get; set; } = default!;
        public SelectList Services { get; set; } = default!;

        public IActionResult OnGet()
        {
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

        [BindProperty] 
        public ServiceStockPart ServiceStockPart { get; set; } = default!;

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ServiceStockParts.Add(ServiceStockPart);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
