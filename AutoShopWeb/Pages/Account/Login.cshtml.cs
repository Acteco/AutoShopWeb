using AutoShopWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoShopWeb.Pages.Account;

public class LoginModel : PageModel
{
    private readonly AuthService _auth;
    public LoginModel(AuthService auth) => _auth = auth;

    [BindProperty] public string Login { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    public string? Error { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
        {
            Error = "Введите логин и пароль.";
            return Page();
        }

        bool ok = await _auth.SignInAsync(HttpContext, Login, Password);
        if (!ok)
        {
            Error = "Неверный логин/пароль или аккаунт отключён.";
            return Page();
        }

        return RedirectToPage("/Index");
    }
}
