using System.Security.Cryptography;
using MeChatness.Models;
using MeChatness.DataAccess;

namespace MeChatness.Services;

public static class TokenService
{
    public static async Task<TokenDataModel?> GenerateTokenAsync(int id, bool isRefreshToken = false) => await CreateTokenAsync(id, isRefreshToken);

    private static async Task<TokenDataModel?> CreateTokenAsync(int id, bool isRefreshToken)
    {
        string token = "";
        List<TokenDataModel>? tokenDatas = await TokenJsonManager.TryLoadAllTokensAsync();
        if (tokenDatas == null)
            return null;

        while (token.Length < 1 || tokenDatas.Any(t => t.Token == token)) // any prüft ob eine Eigenschaft Token den selben Token enthält
        {
            token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)); // wird in das Base64(keine Leerzeichen, etc vorhanden) format formatiert
        }
        return new TokenDataModel 
        { 
            Id = id,
            Token = token,
            TokenType = TokenType.AccessToken,
            Expiration = DateTime.UtcNow.AddHours(1) // Token läuft nach 1 Stunde ab
        };
    }
}
