using AutoShopWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoShopWeb.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly AuthService _auth;
    public LogoutModel(AuthService auth) => _auth = auth;

    public async Task<IActionResult> OnGetAsync()
    {
        await _auth.SignOutAsync(HttpContext);
        return RedirectToPage("/Account/Login");
    }
}
