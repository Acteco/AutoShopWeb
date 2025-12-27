using AutoShopWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoShopWeb.Pages.Account;

[IgnoreAntiforgeryToken]
public class RegisterClientModel : PageModel
{
    private readonly AuthService _auth;
    public RegisterClientModel(AuthService auth) => _auth = auth;

    [BindProperty] public string LastName { get; set; } = "";
    [BindProperty] public string FirstName { get; set; } = "";
    [BindProperty] public string? MiddleName { get; set; }

    [BindProperty] public string PassportSeries { get; set; } = "";
    [BindProperty] public string PassportNumber { get; set; } = "";
    [BindProperty] public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-18);
    [BindProperty] public DateTime PassportIssueDate { get; set; } = DateTime.Today;

    [BindProperty] public string Phone { get; set; } = "";
    [BindProperty] public string? Email { get; set; }
    [BindProperty] public string? Address { get; set; }

    [BindProperty] public string Login { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    [BindProperty] public string Password2 { get; set; } = "";

    public string? Error { get; set; }
    public string? Success { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Password != Password2)
        {
            Error = "Пароли не совпадают.";
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
        {
            Error = "Логин и пароль обязательны.";
            return Page();
        }

        try
        {
            bool ok = await _auth.RegisterClientAsync(
                LastName, FirstName, MiddleName,
                PassportSeries, PassportNumber,
                BirthDate, PassportIssueDate,
                Phone, Email, Address,
                Login, Password);

            if (!ok)
            {
                Error = "Такой логин уже существует.";
                return Page();
            }

            Success = "Клиент зарегистрирован! Теперь войдите.";
            return Page();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            return Page();
        }
    }
}
