using System.Security.Cryptography;

namespace AutoShopWeb.Security;

public static class PasswordHasher
{
    public static (byte[] hash, byte[] salt) HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(32);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 120_000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);
        return (hash, salt);
    }

    public static bool Verify(string password, byte[] hash, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 120_000, HashAlgorithmName.SHA256);
        byte[] computed = pbkdf2.GetBytes(hash.Length);
        return CryptographicOperations.FixedTimeEquals(computed, hash);
    }
}
