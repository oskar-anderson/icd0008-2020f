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
    public class DeleteModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DeleteModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DbGameData = await _context.GameData.FindAsync(id);

            if (DbGameData != null)
            {
                _context.GameData.Remove(DbGameData);
                _context.Player.Remove(DbGameData.ActivePlayer);
                _context.Player.Remove(DbGameData.InactivePlayer);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
