using AutoShopWeb.Data;
using AutoShopWeb.Infrastructure;
using AutoShopWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoShopWeb.Pages.Employee;

[Authorize(Roles = $"{Roles.Employee},{Roles.Admin}")]
public class CarsCrudModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public CarsCrudModel(AutoShopDbContext db) => _db = db;

    public List<Car> Cars { get; private set; } = new();
    public string? Message { get; set; }

    [BindProperty] public string CreateModel { get; set; } = "";
    [BindProperty] public int CreateYear { get; set; } = DateTime.Now.Year;
    [BindProperty] public int CreateMileage { get; set; } = 0;
    [BindProperty] public decimal CreatePrice { get; set; } = 100000;
    [BindProperty] public string CreateStatus { get; set; } = "В наличии";

    public async Task OnGetAsync()
        => Cars = await _db.Cars.OrderByDescending(c => c.CarId).ToListAsync();

    public async Task<IActionResult> OnPostCreateAsync()
    {
        try
        {
            _db.Cars.Add(new Car
            {
                Model = CreateModel,
                ReleaseYear = CreateYear,
                Mileage = CreateMileage,
                Price = CreatePrice,
                Status = CreateStatus
            });
            await _db.SaveChangesAsync();
            Message = "Автомобиль добавлен.";
        }
        catch (Exception ex)
        {
            Message = ex.Message;
        }

        await OnGetAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            var car = await _db.Cars.FirstOrDefaultAsync(c => c.CarId == id);
            if (car != null)
            {
                _db.Cars.Remove(car);
                await _db.SaveChangesAsync();
                Message = "Удалено.";
            }
        }
        catch (Exception ex)
        {
            Message = ex.Message;
        }

        await OnGetAsync();
        return Page();
    }
}
