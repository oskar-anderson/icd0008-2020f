using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;

namespace WebApp.Pages_GameDataCRUD
{
    public class DetailsModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DetailsModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public DbGameData DbGameData { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DbGameData = await _context.GameData
                .Include(d => d.ActivePlayer)
                .Include(d => d.InactivePlayer).FirstOrDefaultAsync(m => m.ID == id);

            if (DbGameData == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
