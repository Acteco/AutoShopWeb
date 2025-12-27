using AutoShopWeb.Data;
using AutoShopWeb.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoShopWeb.Pages.Employee.Payments;

[Authorize(Roles = $"{Roles.Employee},{Roles.Admin}")]
public class IndexModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public IndexModel(AutoShopDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int ContractNumber { get; set; }

    public string? ContractInfo { get; private set; }
    public string? Error { get; private set; }

    public sealed record Row(int PaymentId, DateOnly PayDate, decimal Amount, string Status, string PaymentMethod);
    public List<Row> Items { get; private set; } = new();

    public async Task OnGetAsync()
    {
        if (ContractNumber <= 0)
        {
            Error = "Не указан номер договора (contractNumber).";
            return;
        }

        var contract = await _db.Contracts
            .Include(c => c.Client)
            .Include(c => c.Car)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ContractNumber == ContractNumber);

        if (contract == null)
        {
            Error = "Договор не найден.";
            return;
        }

        ContractInfo = $"№{contract.ContractNumber} | {contract.Client.LastName} {contract.Client.FirstName} | {contract.Car.Model} | сумма {contract.TotalAmount}";

        Items = await _db.Payments
            .AsNoTracking()
            .Where(p => p.ContractNumber == ContractNumber)
            .OrderByDescending(p => p.PayDate)
            .Select(p => new Row(p.PaymentId, p.PayDate, p.Amount, p.Status, p.PaymentMethod))
            .ToListAsync();
    }
}
