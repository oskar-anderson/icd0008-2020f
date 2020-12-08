using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;

namespace WebApp.Pages_GameDataCRUD
{
    public class CreateModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public CreateModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["ActivePlayerID"] = new SelectList(_context.Player, "ID", "ID");
        ViewData["InactivePlayerID"] = new SelectList(_context.Player, "ID", "ID");
            return Page();
        }

        [BindProperty] 
        public DbGameData DbGameData { get; set; } = null!;

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.GameData.Add(DbGameData);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
