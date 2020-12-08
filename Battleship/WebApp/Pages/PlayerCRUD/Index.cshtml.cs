using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;

namespace WebApp.Pages_PlayerCRUD
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<DbPlayerDTO> DbPlayerDTO { get;set; } = null!;

        public async Task OnGetAsync()
        {
            DbPlayerDTO = await _context.Player.ToListAsync();
        }
    }
}
