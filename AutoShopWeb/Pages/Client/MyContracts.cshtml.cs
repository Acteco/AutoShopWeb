using AutoShopWeb.Data;
using AutoShopWeb.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoShopWeb.Pages.Client;

[Authorize(Roles = Roles.Client)]
public class MyContractsModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public MyContractsModel(AutoShopDbContext db) => _db = db;

    public sealed record Row(int ContractNumber, DateOnly CreatedAt, string CarModel, decimal TotalAmount, string PaymentMethod);
    public List<Row> Items { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var clientId = User.GetClientId();
        if (clientId == null) return;

        Items = await _db.Contracts
            .Where(c => c.ClientId == clientId.Value)
            .Include(c => c.Car)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new Row(c.ContractNumber, c.CreatedAt, c.Car.Model, c.TotalAmount, c.PaymentMethod))
            .ToListAsync();
    }
}
