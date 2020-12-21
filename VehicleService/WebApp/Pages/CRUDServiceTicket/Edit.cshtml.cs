using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.CRUDServiceTicket
{
    public class EditModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public EditModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ServiceTicket ServiceTicket { get; set; } = default!;

        public SelectList Services { get; set; } = default!;
        
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ServiceTicket = await _context.ServiceTickets
                .Include(s => s.Service).FirstOrDefaultAsync(m => m.ID == id);

            if (ServiceTicket == null)
            {
                return NotFound();
            }
            Services = new SelectList(
                _context.Services, 
                nameof(ServiceStockPart.Service.ID), 
                nameof(ServiceStockPart.Service.Name));
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

            _context.Attach(ServiceTicket).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceTicketExists(ServiceTicket.ID))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("./Index");
        }

        private bool ServiceTicketExists(string id)
        {
            return _context.ServiceTickets.Any(e => e.ID == id);
        }
    }
}
