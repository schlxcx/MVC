using Isopoh.Cryptography.Argon2;

namespace MeChatness.Services;

public static class PasswordService
{
    public static string HashPassword(string password) => GetHashedPassword(password);
    public static bool IsPasswordValid(string password, string hash) => CheckPassword(password, hash);

    private static bool CheckPassword(string password, string hash) => Argon2.Verify(hash, password);
    private static string GetHashedPassword(string password) => Argon2.Hash(password);
}
