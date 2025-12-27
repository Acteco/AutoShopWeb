using AutoShopWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoShopWeb.Pages.Admin;

[Authorize(Roles = "Admin")]
public class CreateEmployeeUserModel : PageModel
{
    private readonly AuthService _auth;
    public CreateEmployeeUserModel(AuthService auth) => _auth = auth;

    [BindProperty] public string LastName { get; set; } = "";
    [BindProperty] public string FirstName { get; set; } = "";
    [BindProperty] public string? MiddleName { get; set; }

    [BindProperty] public string Position { get; set; } = "Менеджер";
    [BindProperty] public string Phone { get; set; } = "";
    [BindProperty] public string? Email { get; set; }

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
            bool ok = await _auth.RegisterEmployeeAsync(
                LastName, FirstName, MiddleName,
                Position, Phone, Email,
                Login, Password);

            if (!ok)
            {
                Error = "Такой логин уже существует.";
                return Page();
            }

            Success = "Сотрудник создан. Теперь можно войти под его логином.";
            // сброс паролей из модели
            Password = Password2 = "";
            return Page();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            return Page();
        }
    }

    public List<SelectListItem> Positions { get; } = new()
{
    new("Администратор", "Администратор"),
    new("Менеджер", "Менеджер"),
    new("Кассир", "Кассир"),
    new("Склад", "Склад"),
    new("Бухгалтер", "Бухгалтер"),
};
}
