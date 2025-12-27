using AutoShopWeb.Data;
using AutoShopWeb.Infrastructure;
using AutoShopWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoShopWeb.Pages.Employee;

[Authorize(Roles = $"{Roles.Employee},{Roles.Admin}")]
public class ServicesCrudModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public ServicesCrudModel(AutoShopDbContext db) => _db = db;

    public List<Service> Services { get; private set; } = new();
    public string? Message { get; set; }

    [BindProperty] public string CreateServiceType { get; set; } = "";
    [BindProperty] public decimal CreateCost { get; set; } = 1000;

    public async Task OnGetAsync()
        => Services = await _db.Services.OrderBy(s => s.ServiceType).ToListAsync();

    public async Task<IActionResult> OnPostCreateAsync()
    {
        try
        {
            _db.Services.Add(new Service
            {
                ServiceType = CreateServiceType,
                Cost = CreateCost
            });
            await _db.SaveChangesAsync();
            Message = "Услуга добавлена.";
        }
        catch (Exception ex)
        {
            Message = ex.Message;
        }

        await OnGetAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateAsync(int id, string serviceType, decimal cost)
    {
        try
        {
            var s = await _db.Services.FirstOrDefaultAsync(x => x.ServiceId == id);
            if (s == null) { Message = "Не найдено."; await OnGetAsync(); return Page(); }

            s.ServiceType = serviceType;
            s.Cost = cost;

            await _db.SaveChangesAsync();
            Message = "Сохранено.";
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
            var s = await _db.Services.FirstOrDefaultAsync(x => x.ServiceId == id);
            if (s != null)
            {
                _db.Services.Remove(s);
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
