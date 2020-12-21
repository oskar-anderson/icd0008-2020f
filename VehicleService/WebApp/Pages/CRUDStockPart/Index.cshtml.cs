using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.CRUDStockPart
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<StockPart> StockParts { get;set; } = default!;
        public IList<StockPart> StockPartsRestock { get;set; } = default!;
        public int TotalRestockCost { get;set; } = 0;
        public string? Search { get; set; }
        
        private struct SearchOption
        {
            public readonly string Name;
            public readonly bool Include;

            public SearchOption(string _name, bool _include)
            {
                Name = _name;
                Include = _include;
            }
        }
        
        public async Task OnGetAsync(string? search, string? toDoAction)
        {
            List<SearchOption> nameSearches = new List<SearchOption>();
            List<SearchOption> categorySearches = new List<SearchOption>();
            List<SearchOption> amountSearches = new List<SearchOption>();
            
            var query = _context.StockParts.AsQueryable();
            StockPartsRestock = _context.StockParts.Where(x => x.CurrentQuantity < x.OptimalQuantity).ToList();
            TotalRestockCost = StockPartsRestock
                .Where(x => x.CurrentQuantity < x.OptimalQuantity)
                .Select(x => x.Price * (x.OptimalQuantity - x.CurrentQuantity))
                .Sum();
            if (toDoAction == "Reset")
            {
                Search = "";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(search))
                {
                    Search = search.ToLower().Trim();
                    var searchDict = search.ToLower().Trim().Split("; ");
                    foreach (var item in searchDict)
                    {
                        var oneKeyMultipleValue = item.Split(": ");
                        if (oneKeyMultipleValue.Length != 2) continue;
                        string[] values = oneKeyMultipleValue[1].Split(", ");;
                        foreach (string value in values)
                        {
                            string searchName = value;
                            bool include = true;
                            if (value.Length > 0 && value[0] == '!')
                            {
                                searchName = value.Substring(1);
                                include = false;
                            }
                            switch (oneKeyMultipleValue[0])
                            {
                                case "name":
                                    nameSearches.Add(new SearchOption(searchName, include));
                                    break;
                                case "category":
                                    categorySearches.Add(new SearchOption(searchName, include));
                                    break;
                                case "currentquantity":
                                    amountSearches.Add(new SearchOption(searchName, include));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(Search))
                {
                    if (nameSearches.Count == 0 && categorySearches.Count == 0 && amountSearches.Count == 0 )
                    {
                        query = query.Where(x => x.Name.ToLower().Contains(Search));
                    }
                    else
                    {
                        foreach (var nameSearchObject in nameSearches)
                        {
                            query = nameSearchObject.Include ? 
                                query.Where(x => x.Name.ToLower().Contains(nameSearchObject.Name)) : 
                                query.Where(x => !x.Name.ToLower().Contains(nameSearchObject.Name));
                        }
                        foreach (var categorySearchObject in categorySearches)
                        {
                            query = categorySearchObject.Include ? 
                                query.Where(x => x.Category.ToLower().Contains(categorySearchObject.Name)) : 
                                query.Where(x => !x.Category.ToLower().Contains(categorySearchObject.Name));
                        }
                        foreach (var amountSearchObject in amountSearches)
                        {
                            query = amountSearchObject.Include ? 
                                query.Where(x => x.CurrentQuantity.ToString().Contains(amountSearchObject.Name)) : 
                                query.Where(x => !x.CurrentQuantity.ToString().Contains(amountSearchObject.Name));
                        }
                    }
                }
            }
            
            StockParts = await query.OrderBy(b => b.Name).ToListAsync();
        }
    }
}
