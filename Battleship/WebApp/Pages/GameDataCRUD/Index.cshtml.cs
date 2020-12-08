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
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<DbGameData> DbGameData { get;set; } = null!;

        public async Task OnGetAsync()
        {
            DbGameData = await _context.GameData
                .Include(d => d.ActivePlayer)
                .Include(d => d.InactivePlayer).ToListAsync();
        }
    }
}
