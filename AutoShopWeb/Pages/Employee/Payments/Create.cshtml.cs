using AutoShopWeb.Data;
using AutoShopWeb.Infrastructure;
using AutoShopWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoShopWeb.Pages.Employee.Payments;

[Authorize(Roles = $"{Roles.Employee},{Roles.Admin}")]
public class CreateModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public CreateModel(AutoShopDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int ContractNumber { get; set; }

    [BindProperty] public DateTime PayDate { get; set; } = DateTime.Today;
    [BindProperty] public decimal Amount { get; set; } = 1000;
    [BindProperty] public string Status { get; set; } = "Оплачен";
    [BindProperty] public string PaymentMethod { get; set; } = "Карта";

    public string? ContractInfo { get; set; }
    public string? Error { get; set; }
    public string? Success { get; set; }

    public async Task OnGetAsync()
    {
        var c = await _db.Contracts
            .Include(x => x.Client)
            .Include(x => x.Car)
            .FirstOrDefaultAsync(x => x.ContractNumber == ContractNumber);

        if (c == null) { Error = "Договор не найден."; return; }

        ContractInfo = $"№{c.ContractNumber} | {c.Client.LastName} {c.Client.FirstName} | {c.Car.Model} | сумма {c.TotalAmount}";
        Amount = c.TotalAmount;
        PaymentMethod = c.PaymentMethod;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var contract = await _db.Contracts
            .Include(x => x.Car)
            .FirstOrDefaultAsync(x => x.ContractNumber == ContractNumber);

        if (contract == null) { Error = "Договор не найден."; return Page(); }
        if (Amount <= 0) { Error = "Сумма должна быть > 0."; return Page(); }

        try
        {
            _db.Payments.Add(new Payment
            {
                ContractNumber = ContractNumber,
                Amount = Amount,
                Status = Status,
                PayDate = DateOnly.FromDateTime(PayDate),
                PaymentMethod = PaymentMethod
            });

            if (Status == "Оплачен")
            {
                if (contract.Car.Status != "Продан")
                    contract.Car.Status = "Продан";
            }

            await _db.SaveChangesAsync();

            Success = "Оплата сохранена.";
            return RedirectToPage("/Employee/Contracts/Index");
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            return Page();
        }
    }
}
