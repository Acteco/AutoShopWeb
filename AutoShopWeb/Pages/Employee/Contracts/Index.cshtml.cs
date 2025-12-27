using AutoShopWeb.Data;
using AutoShopWeb.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoShopWeb.Pages.Employee.Contracts;

[Authorize(Roles = $"{Roles.Employee},{Roles.Admin}")]
public class IndexModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public IndexModel(AutoShopDbContext db) => _db = db;

    public sealed record Row(int ContractNumber, DateOnly CreatedAt, string ClientFio, string CarModel, decimal TotalAmount, string PaymentMethod);
    public List<Row> Items { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Items = await _db.Contracts
            .Include(c => c.Client)
            .Include(c => c.Car)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new Row(
                c.ContractNumber,
                c.CreatedAt,
                (c.Client.LastName + " " + c.Client.FirstName + " " + (c.Client.MiddleName ?? "")).Trim(),
                c.Car.Model,
                c.TotalAmount,
                c.PaymentMethod
            ))
            .ToListAsync();
    }
}
