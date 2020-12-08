using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Battleship
{
    public class Load : PageModel
    {
        private readonly AppDbContext _context;

        public Load(AppDbContext context)
        {
            _context = context;
        }

        public ICollection<DbGameData> States { get; set; } = null!;

        public IActionResult OnGet()
        {
            States = DbQueries.GetAllGames();
            return Page();
        }
        

        public IActionResult OnPost(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DbQueries.DeleteGame(id);
            return OnGet();
        }
    }
}