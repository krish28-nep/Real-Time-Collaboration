using System.Security.Cryptography;

namespace RealTimeCollaboration.Common.Utils;

public static class SlugGenerator
{
    private const string SuffixCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";
    private const int DefaultSuffixLength = 6;

    public static string Create(string value, int suffixLength = DefaultSuffixLength)
    {
        var slugValue = string.Join(
            "-",
            value.Trim()
                .ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));

        return $"{slugValue}-{CreateRandomSuffix(suffixLength)}";
    }

    private static string CreateRandomSuffix(int suffixLength)
    {
        return RandomNumberGenerator.GetString(SuffixCharacters, suffixLength);
    }
}
