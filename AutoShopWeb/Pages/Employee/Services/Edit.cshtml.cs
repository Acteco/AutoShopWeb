using AutoShopWeb.Data;
using AutoShopWeb.Infrastructure;
using AutoShopWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoShopWeb.Pages.Employee.Services;

[Authorize(Roles = $"{Roles.Employee},{Roles.Admin}")]
public class EditModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public EditModel(AutoShopDbContext db) => _db = db;

    public Service? Service { get; private set; }

    [BindProperty(SupportsGet = true)] public int Id { get; set; }
    [BindProperty] public string ServiceType { get; set; } = "";
    [BindProperty] public decimal Cost { get; set; }

    public string? Error { get; set; }

    public async Task OnGetAsync()
    {
        Service = await _db.Services.FirstOrDefaultAsync(s => s.ServiceId == Id);
        if (Service == null) return;

        ServiceType = Service.ServiceType;
        Cost = Service.Cost;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var s = await _db.Services.FirstOrDefaultAsync(x => x.ServiceId == Id);
        if (s == null)
        {
            Error = "Услуга не найдена.";
            return Page();
        }

        try
        {
            s.ServiceType = ServiceType;
            s.Cost = Cost;
            await _db.SaveChangesAsync();

            return RedirectToPage("/Employee/Services");
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            await OnGetAsync();
            return Page();
        }
    }
}
