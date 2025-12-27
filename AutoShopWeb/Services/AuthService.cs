using System.Security.Claims;
using AutoShopWeb.Data;
using AutoShopWeb.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace AutoShopWeb.Services;

public sealed class AuthService
{
    private readonly AutoShopDbContext _db;

    public AuthService(AutoShopDbContext db) => _db = db;

    public async Task<bool> SignInAsync(HttpContext http, string login, string password)
    {
        login = (login ?? "").Trim();

        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Login == login);

        if (user == null) return false;
        if (user.IsActive == false) return false;

        if (!PasswordHasher.Verify(password, user.PasswordHash, user.PasswordSalt))
            return false;

        user.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        string roleName = user.Role?.Name ?? "Client";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.Role, roleName),
        };

        if (user.ClientId != null)
            claims.Add(new("ClientId", user.ClientId.Value.ToString()));
        if (user.EmployeeId != null)
            claims.Add(new("EmployeeId", user.EmployeeId.Value.ToString()));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties { IsPersistent = true });

        return true;
    }

    public async Task SignOutAsync(HttpContext http)
    {
        await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task<bool> RegisterClientAsync(
        string lastName, string firstName, string? middleName,
        string passportSeries, string passportNumber,
        DateTime birthDate, DateTime passportIssueDate,
        string phone, string? email, string? address,
        string login, string password)
    {
        login = (login ?? "").Trim();

        bool loginExists = await _db.Users.AnyAsync(u => u.Login == login);
        if (loginExists) return false;

        // создаём клиента
        var client = new Models.Client
        {
            LastName = lastName,
            FirstName = firstName,
            MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName,
            PassportSeries = passportSeries,
            PassportNumber = passportNumber,
            BirthDate = DateOnly.FromDateTime(birthDate),
            PassportIssueDate = DateOnly.FromDateTime(passportIssueDate),
            Phone = phone,
            Email = string.IsNullOrWhiteSpace(email) ? null : email,
            Address = string.IsNullOrWhiteSpace(address) ? null : address
        };

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();

        // роль Client
        var role = await _db.Roles.FirstAsync(r => r.Name == "Client");

        var (hash, salt) = PasswordHasher.HashPassword(password);

        var user = new Models.User
        {
            Login = login,
            PasswordHash = hash,
            PasswordSalt = salt,
            RoleId = role.RoleId,
            ClientId = client.ClientId,
            EmployeeId = null,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RegisterEmployeeAsync(
    string lastName, string firstName, string? middleName,
    string position, string phone, string? email,
    string login, string password)
    {
        login = (login ?? "").Trim();

        bool loginExists = await _db.Users.AnyAsync(u => u.Login == login);
        if (loginExists) return false;

        var emp = new Models.Employee
        {
            LastName = lastName,
            FirstName = firstName,
            MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName,
            Position = position,
            Phone = phone,
            Email = string.IsNullOrWhiteSpace(email) ? null : email
        };

        _db.Employees.Add(emp);
        await _db.SaveChangesAsync();

        var role = await _db.Roles.FirstAsync(r => r.Name == "Employee");

        var (hash, salt) = PasswordHasher.HashPassword(password);

        var user = new Models.User
        {
            Login = login,
            PasswordHash = hash,
            PasswordSalt = salt,
            RoleId = role.RoleId,
            ClientId = null,
            EmployeeId = emp.EmployeeId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return true;
    }
}
