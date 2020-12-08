using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;

namespace WebApp.Pages_GameDataCRUD
{
    public class EditModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public EditModel(DAL.AppDbContext context)
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
           ViewData["ActivePlayerID"] = new SelectList(_context.Player, "ID", "ID");
           ViewData["InactivePlayerID"] = new SelectList(_context.Player, "ID", "ID");
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

            _context.Attach(DbGameData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DbGameDataExists(DbGameData.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool DbGameDataExists(string id)
        {
            return _context.GameData.Any(e => e.ID == id);
        }
    }
}
