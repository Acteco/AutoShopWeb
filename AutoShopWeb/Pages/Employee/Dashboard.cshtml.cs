using AutoShopWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoShopWeb.Infrastructure;

namespace AutoShopWeb.Pages.Employee;

[Authorize(Roles = $"{Roles.Employee},{Roles.Admin}")]
public class DashboardModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public DashboardModel(AutoShopDbContext db) => _db = db;

    public int CarsInStock { get; private set; }
    public int ContractsTotal { get; private set; }
    public int PaidCount { get; private set; }
    public decimal Revenue { get; private set; }

    public async Task OnGetAsync()
    {
        CarsInStock = await _db.Cars.CountAsync(c => c.Status == "В наличии");
        ContractsTotal = await _db.Contracts.CountAsync();
        PaidCount = await _db.Payments.CountAsync(p => p.Status == "Оплачен");
        Revenue = await _db.Payments.Where(p => p.Status == "Оплачен").SumAsync(p => (decimal?)p.Amount) ?? 0m;
    }
}
