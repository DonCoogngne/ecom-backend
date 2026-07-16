using System.Security.Cryptography;
using System.Text;

namespace ecom_backend.Security;

public static class TokenHasher
{
    /// <summary>Generates a cryptographically-random, URL-safe refresh token.</summary>
    public static string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    /// <summary>Returns a deterministic SHA-256 hex hash used for storage/lookup.</summary>
    public static string Hash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
