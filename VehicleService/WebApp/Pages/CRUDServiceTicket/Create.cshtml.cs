using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.CRUDServiceTicket
{
    public class CreateModel : PageModel
    {
        private readonly DAL.AppDbContext _context;
        
        public CreateModel(DAL.AppDbContext context)
        {
            _context = context;
        }
        
        public SelectList Services { get; set; } = default!;

        public IActionResult OnGet()
        {
            Services = new SelectList(
                _context.Services, 
                nameof(ServiceStockPart.Service.ID), 
                nameof(ServiceStockPart.Service.Name));
            return Page();
        }

        [BindProperty] 
        public ServiceTicket ServiceTicket { get; set; } = default!;

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ServiceTickets.Add(ServiceTicket);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
