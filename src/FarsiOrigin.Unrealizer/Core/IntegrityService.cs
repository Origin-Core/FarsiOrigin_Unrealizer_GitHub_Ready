using System.Security.Cryptography;

namespace FarsiOrigin.Unrealizer.Core;

public static class IntegrityService
{
    public static string Sha256(string file)
    {
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(file);
        return Convert.ToHexString(sha.ComputeHash(stream));
    }
}
