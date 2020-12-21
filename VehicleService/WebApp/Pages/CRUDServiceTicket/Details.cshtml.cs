using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.CRUDServiceTicket
{
    public class DetailsModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DetailsModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public ServiceTicket ServiceTicket { get; set; } = default!;

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
            return Page();
        }
    }
}
