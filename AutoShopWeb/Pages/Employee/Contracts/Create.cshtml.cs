using AutoShopWeb.Data;
using AutoShopWeb.Infrastructure;
using AutoShopWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoShopWeb.Pages.Employee.Contracts;

[Authorize(Roles = $"{Roles.Employee},{Roles.Admin}")]
public class CreateModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public CreateModel(AutoShopDbContext db) => _db = db;

    public sealed record Opt(int Id, string Text);

    public List<Opt> Clients { get; private set; } = new();
    public List<Opt> Employees { get; private set; } = new();
    public List<Opt> Cars { get; private set; } = new();
    public List<Opt> Services { get; private set; } = new();

    [BindProperty] public int ContractNumber { get; set; }
    [BindProperty] public int ClientId { get; set; }
    [BindProperty] public int CarId { get; set; }
    [BindProperty] public int EmployeeId { get; set; }
    [BindProperty] public int? ServiceId { get; set; }

    [BindProperty] public decimal TotalAmount { get; set; } = 100000;
    [BindProperty] public string PaymentMethod { get; set; } = "Карта";

    [BindProperty] public DateTime CreatedAt { get; set; } = DateTime.Today;

    public string? Error { get; set; }
    public string? Success { get; set; }

    public async Task OnGetAsync()
        => await LoadLists();

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadLists();

        if (ContractNumber <= 0) { Error = "Введите номер договора."; return Page(); }
        if (TotalAmount <= 0) { Error = "Сумма должна быть > 0."; return Page(); }

        bool exists = await _db.Contracts.AnyAsync(c => c.ContractNumber == ContractNumber);
        if (exists) { Error = "Договор с таким номером уже существует."; return Page(); }

        try
        {
            var contract = new Contract
            {
                ContractNumber = ContractNumber,
                ClientId = ClientId,
                CarId = CarId,
                EmployeeId = EmployeeId,
                ServiceId = ServiceId,
                TotalAmount = TotalAmount,
                PaymentMethod = PaymentMethod,
                CreatedAt = DateOnly.FromDateTime(CreatedAt)
            };

            _db.Contracts.Add(contract);
            var car = await _db.Cars.FirstAsync(c => c.CarId == CarId);
            if (car.Status == "В наличии")
                car.Status = "Зарезервирован";

            await _db.SaveChangesAsync();

            Success = "Договор создан.";
            return RedirectToPage("/Employee/Contracts/Index");
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            return Page();
        }
    }

    private async Task LoadLists()
    {
        Clients = await _db.Clients
            .OrderBy(c => c.LastName)
            .Select(c => new Opt(c.ClientId, (c.LastName + " " + c.FirstName + " " + (c.MiddleName ?? "")).Trim() + $" | {c.PassportSeries}{c.PassportNumber}"))
            .ToListAsync();

        Employees = await _db.Employees
            .OrderBy(e => e.LastName)
            .Select(e => new Opt(e.EmployeeId, (e.LastName + " " + e.FirstName + " " + (e.MiddleName ?? "")).Trim() + $" | {e.Position}"))
            .ToListAsync();

        Cars = await _db.Cars
            .OrderByDescending(c => c.Status == "В наличии") // сначала "в наличии"
            .ThenBy(c => c.Model)
            .Select(c => new Opt(c.CarId, $"{c.Model} | {c.ReleaseYear} | {c.Price} | {c.Status}"))
            .ToListAsync();

        Services = await _db.Services
            .OrderBy(s => s.ServiceType)
            .Select(s => new Opt(s.ServiceId, $"{s.ServiceType} | {s.Cost}"))
            .ToListAsync();
    }
}
