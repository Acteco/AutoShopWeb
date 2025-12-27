using AutoShopWeb.Data;
using AutoShopWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoShopWeb.Pages;

public class ServicesModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public ServicesModel(AutoShopDbContext db) => _db = db;

    public List<Service> Services { get; private set; } = new();
    public int Total { get; private set; }

    [BindProperty(SupportsGet = true)] public string? Q { get; set; }
    [BindProperty(SupportsGet = true)] public decimal? PriceFrom { get; set; }
    [BindProperty(SupportsGet = true)] public decimal? PriceTo { get; set; }
    [BindProperty(SupportsGet = true)] public string Sort { get; set; } = "name";

    public async Task OnGetAsync()
    {
        IQueryable<Service> query = _db.Services.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Q))
        {
            var q = Q.Trim();
            query = query.Where(s => s.ServiceType.Contains(q));
        }

        if (PriceFrom.HasValue) query = query.Where(s => s.Cost >= PriceFrom.Value);
        if (PriceTo.HasValue) query = query.Where(s => s.Cost <= PriceTo.Value);

        query = Sort switch
        {
            "price_asc" => query.OrderBy(s => s.Cost),
            "price_desc" => query.OrderByDescending(s => s.Cost),
            _ => query.OrderBy(s => s.ServiceType)
        };

        Total = await query.CountAsync();
        Services = await query.ToListAsync();
    }
}
