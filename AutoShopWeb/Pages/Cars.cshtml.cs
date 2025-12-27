using AutoShopWeb.Data;
using AutoShopWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoShopWeb.Pages;

public class CarsModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public CarsModel(AutoShopDbContext db) => _db = db;

    public List<Car> Cars { get; private set; } = new();
    public int Total { get; private set; }

    [BindProperty(SupportsGet = true)] public string? Q { get; set; }
    [BindProperty(SupportsGet = true)] public int? YearFrom { get; set; }
    [BindProperty(SupportsGet = true)] public int? YearTo { get; set; }
    [BindProperty(SupportsGet = true)] public decimal? PriceFrom { get; set; }
    [BindProperty(SupportsGet = true)] public decimal? PriceTo { get; set; }
    [BindProperty(SupportsGet = true)] public string? Status { get; set; }

    [BindProperty(SupportsGet = true)] public string Sort { get; set; } = "model";

    public async Task OnGetAsync()
    {
        IQueryable<Car> query = _db.Cars.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(Q))
        {
            var q = Q.Trim();
            query = query.Where(c => c.Model.Contains(q));
        }

        if (YearFrom.HasValue) query = query.Where(c => c.ReleaseYear >= YearFrom.Value);
        if (YearTo.HasValue) query = query.Where(c => c.ReleaseYear <= YearTo.Value);
        if (PriceFrom.HasValue) query = query.Where(c => c.Price >= PriceFrom.Value);
        if (PriceTo.HasValue) query = query.Where(c => c.Price <= PriceTo.Value);

        if (!string.IsNullOrWhiteSpace(Status))
            query = query.Where(c => c.Status == Status);

        query = Sort switch
        {
            "price_asc" => query.OrderBy(c => c.Price),
            "price_desc" => query.OrderByDescending(c => c.Price),
            "year_desc" => query.OrderByDescending(c => c.ReleaseYear),
            "mileage_asc" => query.OrderBy(c => c.Mileage),
            _ => query.OrderBy(c => c.Model)
        };

        Total = await query.CountAsync();
        Cars = await query.ToListAsync();
    }
}
