using AutoShopWeb.Data;
using AutoShopWeb.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoShopWeb.Pages.Admin;

[Authorize(Roles = Roles.Admin)]
public class UsersModel : PageModel
{
    private readonly AutoShopDbContext _db;
    public UsersModel(AutoShopDbContext db) => _db = db;

    public sealed record Row(int UserId, string Login, string RoleName, bool IsActive, DateTime CreatedAt, DateTime? LastLoginAt);
    public List<Row> Users { get; private set; } = new();

    public string? Message { get; set; }

    public async Task OnGetAsync()
    {
        Users = await _db.Users
            .Include(u => u.Role)
            .OrderByDescending(u => u.UserId)
            .Select(u => new Row(u.UserId, u.Login, u.Role.Name, u.IsActive, u.CreatedAt, u.LastLoginAt))
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDisableAsync(int id)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.UserId == id);
        if (u != null)
        {
            u.IsActive = false;
            await _db.SaveChangesAsync();
            Message = "Пользователь отключён.";
        }
        await OnGetAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostEnableAsync(int id)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.UserId == id);
        if (u != null)
        {
            u.IsActive = true;
            await _db.SaveChangesAsync();
            Message = "Пользователь включён.";
        }
        await OnGetAsync();
        return Page();
    }
}
