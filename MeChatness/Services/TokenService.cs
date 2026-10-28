using System.Security.Cryptography;
using MeChatness.Models;
using MeChatness.DataAccess;

namespace MeChatness.Services;

public class TokenService
{
    private readonly IConfiguration _config;
    private static TokenSqlManager TokenSqlManager;

    public TokenService(IConfiguration config)
    {
        _config = config;
        TokenSqlManager = new TokenSqlManager(config);
    }

    public async Task<List<TokenDataModel>?> GenerateTokenAsync(int userId, bool containsRefreshToken = false, List<string>? tokens = null, bool isOnlyRefreshToken = false) => await CreateTokenAsync(userId, containsRefreshToken, tokens, isOnlyRefreshToken);
    public async Task<(bool isTokenValid, List<TokenDataModel>? tokenList)> IsTokenValidAsync(Dictionary<string, TokenType> tokens) => await CheckTokenIsValidAsync(tokens);

    private async Task<List<TokenDataModel>> CreateTokenAsync(int userId, bool containsRefreshToken, List<string>? tokens, bool isOnlyRefreshToken)
    {
        string token = "";
        List<TokenDataModel>? tokenDatas = await TokenJsonManager.TryLoadAllTokensAsync();
        if (tokenDatas == null)
            return null;

        while (token.Length < 1 || tokenDatas.Any(t => t.Token == token)) // any prüft ob eine Eigenschaft Token den selben Token enthält
        {
            token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)); // wird in das Base64(keine Leerzeichen, etc vorhanden) format formatiert
        }

        int id = tokenDatas.Count > 0 ? tokenDatas.Max(u => u.Id) + 1 : 1;

        TokenDataModel tokenData = null;
        TokenDataModel refreshTokenData = null;

        if (isOnlyRefreshToken)
        {
            TokenDataModel? existingRefreshToken = await TokenJsonManager.TryLoadTokenAsync(token, TokenType.RefreshToken);

            if (existingRefreshToken == null || existingRefreshToken.Token == null)
                return null;

            await TokenJsonManager.TryDeleteTokenAsync(existingRefreshToken.Token, TokenType.RefreshToken);

            tokenData = new TokenDataModel()
            {
                Id = id,
                Token = token,
                TokenType = TokenType.RefreshToken,
                Expiration = DateTime.UtcNow.AddDays(7),
                UserId = existingRefreshToken.UserId
            };
        }
        else
        {
            if (!containsRefreshToken)
            {
                tokenData = new TokenDataModel()
                {
                    Id = id,
                    Token = token,
                    TokenType = TokenType.AccessToken,
                    Expiration = DateTime.UtcNow.AddHours(12),
                    UserId = userId
                };
            }

            if (containsRefreshToken)
            {
                id++;

                refreshTokenData = new TokenDataModel()
                {
                    Id = id,
                    Token = token,
                    TokenType = TokenType.RefreshToken,
                    Expiration = DateTime.UtcNow.AddDays(7),
                    UserId = userId
                };
            }
        }

        if (!await TokenJsonManager.TrySaveTokenAsync(tokenData))
            return null;

        List<TokenDataModel> tokensToReturn = new List<TokenDataModel>([tokenData]);

        if (refreshTokenData != null)
        {
            await TokenJsonManager.TrySaveTokenAsync(refreshTokenData);
            tokensToReturn.Add(refreshTokenData);
        }

        return tokensToReturn;
    }

    private async Task<(bool isTokenValid, List<TokenDataModel>? tokenList)> CheckTokenIsValidAsync(Dictionary<string, TokenType> tokens)
    {
        List<TokenDataModel>? tokenDatas = await TokenJsonManager.TryLoadAllTokensAsync();
        if (tokenDatas == null)
            return (false, null);
        List<TokenDataModel?> tokenDataList = tokenDatas.Where(t => tokens.ContainsKey(t.Token) && t.TokenType == tokens[t.Token]).ToList();
        if (tokenDataList == null || tokenDataList.Count == 0)
            return (false, null);
        
        bool isAccessTokenUpdated = false;
        bool isRefreshTokenUpdated = false;
        TokenDataModel? accessToken = tokenDataList.FirstOrDefault(t => t.TokenType == TokenType.AccessToken);
        TokenDataModel? refreshToken = tokenDataList.FirstOrDefault(t => t.TokenType == TokenType.RefreshToken);

        if (accessToken == null || string.IsNullOrWhiteSpace(accessToken.Token))
            return (false, null);

        if (accessToken.Expiration < DateTime.UtcNow && accessToken.TokenType == TokenType.AccessToken)
        {
            await TokenJsonManager.TryDeleteTokenAsync(accessToken.Token, accessToken.TokenType);
            if (refreshToken == null || string.IsNullOrWhiteSpace(refreshToken.Token))
                return (false, null);
        
            accessToken = (await CreateTokenAsync(-1, false, [refreshToken.Token], false)).FirstOrDefault();
            isAccessTokenUpdated = true;
        }
        
        if (refreshToken != null && !string.IsNullOrWhiteSpace(refreshToken.Token) && refreshToken.Expiration < DateTime.UtcNow && refreshToken.TokenType == TokenType.RefreshToken)
        {
            await TokenJsonManager.TryDeleteTokenAsync(refreshToken.Token, refreshToken.TokenType);
            refreshToken = (await CreateTokenAsync(-1, true, new List<string>([refreshToken.Token]), true)).FirstOrDefault();
            isRefreshTokenUpdated = true;
        }
        return (true, isAccessTokenUpdated || isRefreshTokenUpdated ?
                        isAccessTokenUpdated && isRefreshTokenUpdated ?
                            new List<TokenDataModel?> { accessToken, refreshToken } : 
                                isAccessTokenUpdated ? new List<TokenDataModel?> { accessToken } : new List<TokenDataModel?>
                            { refreshToken } : null)!;
    }
}
