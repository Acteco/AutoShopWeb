using AutoShopWeb.Data;
using AutoShopWeb.Infrastructure;
using AutoShopWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoShopWeb.Pages.Employee.Payments;

[Authorize(Roles = $"{Roles.Employee},{Roles.Admin}")]
public class EditModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public EditModel(AutoShopDbContext db) => _db = db;

    public Payment? Payment { get; private set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public int ContractNumber { get; set; }

    [BindProperty] public DateTime PayDate { get; set; } = DateTime.Today;
    [BindProperty] public decimal Amount { get; set; }
    [BindProperty] public string Status { get; set; } = "Ожидает";
    [BindProperty] public string PaymentMethod { get; set; } = "Карта";

    public string? Error { get; set; }

    public async Task OnGetAsync()
    {
        Payment = await _db.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.PaymentId == Id);
        if (Payment == null) return;

        ContractNumber = Payment.ContractNumber;
        PayDate = Payment.PayDate.ToDateTime(TimeOnly.MinValue);
        Amount = Payment.Amount;
        Status = Payment.Status;
        PaymentMethod = Payment.PaymentMethod;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var p = await _db.Payments.FirstOrDefaultAsync(x => x.PaymentId == Id);
        if (p == null)
        {
            Error = "Оплата не найдена.";
            return Page();
        }

        try
        {
            p.PayDate = DateOnly.FromDateTime(PayDate);
            p.Amount = Amount;
            p.Status = Status;
            p.PaymentMethod = PaymentMethod;

            await _db.SaveChangesAsync();

            var contract = await _db.Contracts
                .Include(c => c.Car)
                .FirstOrDefaultAsync(c => c.ContractNumber == p.ContractNumber);

            if (contract != null)
            {
                if (Status == "Оплачен" && contract.Car.Status != "Продан")
                    contract.Car.Status = "Продан";

                if (Status != "Оплачен" && contract.Car.Status == "Продан")
                    contract.Car.Status = "Зарезервирован";

                await _db.SaveChangesAsync();
            }

            return Redirect($"/Employee/Payments?contractNumber={p.ContractNumber}");
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            await OnGetAsync();
            return Page();
        }
    }
}
