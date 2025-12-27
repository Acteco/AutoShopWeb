using System.Security.Claims;

namespace AutoShopWeb.Infrastructure;

public static class UserClaims
{
    public static int? GetClientId(this ClaimsPrincipal user)
        => int.TryParse(user.FindFirstValue("ClientId"), out var v) ? v : null;

    public static int? GetEmployeeId(this ClaimsPrincipal user)
        => int.TryParse(user.FindFirstValue("EmployeeId"), out var v) ? v : null;
}
