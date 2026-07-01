using System.Security.Cryptography;
using System.Text;

namespace Education.Helpers;

public static class HashHelper
{
    public static string GetSha256Hash(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        using SHA256 sha256Hash = SHA256.Create();
        var hash = sha256Hash.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
