using AutoShopWeb.Data;
using AutoShopWeb.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoShopWeb.Pages.Client;

[Authorize(Roles = Roles.Client)]
public class MyPaymentsModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public MyPaymentsModel(AutoShopDbContext db) => _db = db;

    public sealed record Row(int ContractNumber, DateOnly PayDate, decimal Amount, string Status, string PaymentMethod);
    public List<Row> Items { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var clientId = User.GetClientId();
        if (clientId == null) return;

        Items = await _db.Payments
            .Include(p => p.ContractNumberNavigation)
            .Where(p => p.ContractNumberNavigation.ClientId == clientId.Value)
            .OrderByDescending(p => p.PayDate)
            .Select(p => new Row(p.ContractNumber, p.PayDate, p.Amount, p.Status, p.PaymentMethod))
            .ToListAsync();
    }
}
