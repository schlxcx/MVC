using MeChatness.Models;
using MySqlConnector;

namespace MeChatness.DataAccess;

public class TokenSqlManager
{
    const string RefreshToken = "RefreshToken";
    const string TokenTableName = "token";
    const string TokentypeTableName = "tokentype";

    private readonly string _connString;

    public TokenSqlManager(IConfiguration config) { _connString = config.GetConnectionString("Default")!; }

    public async Task<List<TokenDataModel>?> GetAllTokensAsync()
    {
        List<TokenDataModel>? tokens = null;
        try
        {
            using var conn = new MySqlConnection(_connString);

            conn.Open();

            using var cmd = new MySqlCommand($"SELECT * FROM {TokenTableName} JOIN {TokentypeTableName} ON {TokenTableName}.tokentype_id = {TokentypeTableName}.tokentype_id", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tokens.Add(new TokenDataModel
                {
                    Id = reader.GetInt32(TokenTableName + ".token_id"),
                    Token = reader.GetString(TokenTableName + ".token"),
                    TokenType = reader.GetString(TokentypeTableName + ".tokentype").ToLower() == RefreshToken.ToLower() ? TokenType.RefreshToken : TokenType.AccessToken,
                    Expiration = reader.GetDateTime(TokenTableName + ".expires"),
                    CreatedAt = reader.GetDateTime(TokenTableName + ".created_at"),
                    UserId = reader.GetInt32(TokenTableName + ".user_id")
                });
            }
        }
        catch (Exception ex) { }
        return tokens;
    }

    public async Task<TokenDataModel?> GetTokenByUserIdAsync(int userId)
    {
        TokenDataModel? token = null;
        try
        {
            using var conn = new MySqlConnection(_connString);

            conn.Open();

            using var cmd = new MySqlCommand($"SELECT * FROM {TokenTableName} JOIN {TokentypeTableName} ON {TokenTableName}.tokentype_id = {TokentypeTableName}.tokentype_id WHERE user_id = '{userId}'", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                token = new TokenDataModel
                {
                    Id = reader.GetInt32(TokenTableName + ".token_id"),
                    Token = reader.GetString(TokenTableName + ".token"),
                    TokenType = reader.GetString(TokentypeTableName + ".tokentype").ToLower() == RefreshToken.ToLower() ? TokenType.RefreshToken : TokenType.AccessToken,
                    Expiration = reader.GetDateTime(TokenTableName + ".expires"),
                    CreatedAt = reader.GetDateTime(TokenTableName + ".created_at"),
                    UserId = reader.GetInt32(TokenTableName + ".user_id")
                };
            }
        }
        catch (Exception ex) { }
        return token;
    }

    public async Task<TokenDataModel?> GetTokenByTokenAsync(string searchedToken)
    {
        TokenDataModel? token = null;
        try
        {
            using var conn = new MySqlConnection(_connString);

            conn.Open();

            using var cmd = new MySqlCommand($"SELECT * FROM {TokenTableName} JOIN {TokentypeTableName} ON {TokenTableName}.tokentype_id = {TokentypeTableName}.tokentype_id WHERE user_id = '{searchedToken}'", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                token = new TokenDataModel
                {
                    Id = reader.GetInt32(TokenTableName + ".token_id"),
                    Token = reader.GetString(TokenTableName + ".token"),
                    TokenType = reader.GetString(TokentypeTableName + ".tokentype").ToLower() == RefreshToken.ToLower() ? TokenType.RefreshToken : TokenType.AccessToken,
                    Expiration = reader.GetDateTime(TokenTableName + ".expires_at"),
                    CreatedAt = reader.GetDateTime(TokenTableName + ".created_at"),
                    UserId = reader.GetInt32(TokenTableName + ".user_id")
                };
            }
        }
        catch (Exception ex) { }
        return token;
    }

    public async Task<bool> TryCreateToken(TokenDataModel token)
    {
        try
        {
            using var conn = new MySqlConnection(_connString);
            conn.Open();
            using var cmdSelect = new MySqlCommand($"SELECT tokentype_id FROM {TokentypeTableName} WHERE tokentype = '{token.TokenType}'", conn);
            using var reader = await cmdSelect.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                using var cmdInsert = new MySqlCommand($"INSERT INTO {token} (token, expires_at, created_at, tokentype_id, user_id) VALUES ('{token.Token}', '{token.Expiration}', '{token.CreatedAt}', '{reader.GetInt32("tokentype_id")}' '{token.UserId}'", conn);

                return true;
            }
        }
        catch (Exception ex) { }
        return false;
    }
}
