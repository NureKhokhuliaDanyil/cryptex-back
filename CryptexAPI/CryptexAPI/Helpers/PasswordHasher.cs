using System.Security.Cryptography;

namespace CryptexAPI.Helpers;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 10000;

    public static (string hash, string salt) HashPassword(string password)
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var saltBytes = new byte[SaltSize];
            rng.GetBytes(saltBytes);

            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations);
            var hashBytes = pbkdf2.GetBytes(KeySize);

            var salt = Convert.ToBase64String(saltBytes);
            var hash = Convert.ToBase64String(hashBytes);

            return (hash, salt);
        }
    }

    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);
        var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations);
        var hashBytes = pbkdf2.GetBytes(KeySize);
        var hash = Convert.ToBase64String(hashBytes);

        return hash == storedHash;
    }
}