using AutoShopWeb.Data;
using AutoShopWeb.Infrastructure;
using AutoShopWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoShopWeb.Pages.Client;

[Authorize(Roles = Roles.Client)]
public class ProfileModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public ProfileModel(AutoShopDbContext db) => _db = db;

    public Models.Client? Client { get; private set; }

    [BindProperty] public string Phone { get; set; } = "";
    [BindProperty] public string? Email { get; set; }
    [BindProperty] public string? Address { get; set; }

    public string? Success { get; set; }
    public string? Error { get; set; }

    public async Task OnGetAsync()
    {
        var clientId = User.GetClientId();
        if (clientId == null) return;

        Client = await _db.Clients.FindAsync(clientId.Value);
        if (Client == null) return;

        Phone = Client.Phone;
        Email = Client.Email;
        Address = Client.Address;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var clientId = User.GetClientId();
        if (clientId == null) return RedirectToPage("/Account/Login");

        var c = await _db.Clients.FindAsync(clientId.Value);
        if (c == null) return Page();

        try
        {
            c.Phone = Phone;
            c.Email = string.IsNullOrWhiteSpace(Email) ? null : Email;
            c.Address = string.IsNullOrWhiteSpace(Address) ? null : Address;

            await _db.SaveChangesAsync();
            Success = "Сохранено.";
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }

        return await ReloadSelf();
    }

    private async Task<IActionResult> ReloadSelf()
    {
        await OnGetAsync();
        return Page();
    }
}
