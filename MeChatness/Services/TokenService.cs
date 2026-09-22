using System.Security.Cryptography;
using MeChatness.Models;
using MeChatness.DataAccess;

namespace MeChatness.Services;

public static class TokenService
{
    public static async Task<string?> GenerateTokenAsync() => await CreateTokenAsync();

    private static async Task<string?> CreateTokenAsync()
    {
        string token = "";
        List<SignInDataModel>? signInDatas = await LoginJsonManager.TryLoadAllUsersAsync();
        if (signInDatas == null)
            return null;

        while (token.Length < 1 || signInDatas.Any(u => u.Token == token)) // any prüft ob eine Eigenschaft Token den selben Token enthält
        {
            token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)); // wird in das Base64(keine Leerzeichen, etc vorhanden) format formatiert
        }
        return token;
    }
}
