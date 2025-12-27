using AutoShopWeb.Data;
using AutoShopWeb.Infrastructure;
using AutoShopWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoShopWeb.Pages.Employee.Cars;

[Authorize(Roles = $"{Roles.Employee},{Roles.Admin}")]
public class EditModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public EditModel(AutoShopDbContext db) => _db = db;

    public Car? Car { get; private set; }

    [BindProperty(SupportsGet = true)] public int Id { get; set; }

    [BindProperty] public string ModelName { get; set; } = "";
    [BindProperty] public int ReleaseYear { get; set; }
    [BindProperty] public int Mileage { get; set; }
    [BindProperty] public decimal Price { get; set; }
    [BindProperty] public string Status { get; set; } = "В наличии";

    public string? Error { get; set; }

    public async Task OnGetAsync()
    {
        Car = await _db.Cars.AsNoTracking().FirstOrDefaultAsync(c => c.CarId == Id);
        if (Car == null) return;

        ModelName = Car.Model;
        ReleaseYear = Car.ReleaseYear;
        Mileage = Car.Mileage;
        Price = Car.Price;
        Status = Car.Status;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var car = await _db.Cars.FirstOrDefaultAsync(c => c.CarId == Id);
        if (car == null)
        {
            Error = "Авто не найдено.";
            return Page();
        }

        try
        {
            car.Model = ModelName;
            car.ReleaseYear = ReleaseYear;
            car.Mileage = Mileage;
            car.Price = Price;
            car.Status = Status;

            await _db.SaveChangesAsync();
            return RedirectToPage("/Employee/Cars");
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            await OnGetAsync();
            return Page();
        }
    }
}
