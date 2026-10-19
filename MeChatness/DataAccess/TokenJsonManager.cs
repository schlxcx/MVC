using MeChatness.Models;
using System.Text.Json;

namespace MeChatness.DataAccess;

public class TokenJsonManager
{
    const string RouteToJson = "~/../Data/TokenDataTestFile.json";

    public async static Task<List<TokenDataModel>?> TryLoadAllTokensAsync()
    {
        List<TokenDataModel>? tokenDataList = null;
        try
        {
            string json = await File.ReadAllTextAsync(RouteToJson);
            tokenDataList = JsonSerializer.Deserialize<List<TokenDataModel>>(json);
        }
        catch
        {
            return null;
        }
        return tokenDataList;
    }

    public async static Task<TokenDataModel?> TryLoadTokenAsync(int id, TokenType tokenType)
    {
        List<TokenDataModel>? dataList = null;
        TokenDataModel? tokenData = null;
        try
        {
            string json = await File.ReadAllTextAsync(RouteToJson);
            dataList = JsonSerializer.Deserialize<List<TokenDataModel>>(json);
            tokenData = dataList.FirstOrDefault(t => t.Id == id && t.TokenType == tokenType);
        }
        catch
        {
            return null;
        }
        return tokenData;
    }

    public async static Task<TokenDataModel?> TryLoadTokenAsync(string token, TokenType tokenType)
    {
        List<TokenDataModel>? dataList = null;
        TokenDataModel? tokenData = null;
        try
        {
            string json = await File.ReadAllTextAsync(RouteToJson);
            dataList = JsonSerializer.Deserialize<List<TokenDataModel>>(json);
            tokenData = dataList.FirstOrDefault(t => t.Token == token && t.TokenType == tokenType);
        }
        catch
        {
            return null;
        }
        return tokenData;
    }

    public async static Task<bool> TrySaveTokenAsync(TokenDataModel tokenData)
    {
        List<TokenDataModel>? dataList = null;
        try
        {
            string json = await File.ReadAllTextAsync(RouteToJson);
            dataList = JsonSerializer.Deserialize<List<TokenDataModel>>(json);
            dataList.Add(tokenData);
            string updatedJson = JsonSerializer.Serialize(dataList, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(RouteToJson, updatedJson);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async static Task<bool> TryDeleteTokenAsync(string token, TokenType tokenType)
    {
        List<TokenDataModel>? dataList = null;
        try
        {
            string json = await File.ReadAllTextAsync(RouteToJson);
            dataList = JsonSerializer.Deserialize<List<TokenDataModel>>(json);
            TokenDataModel? tokenData = dataList.FirstOrDefault(t => t.Token == token && t.TokenType == tokenType);
            if (tokenData != null)
            {
                dataList.Remove(tokenData);
                string updatedJson = JsonSerializer.Serialize(dataList, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(RouteToJson, updatedJson);
                return true;
            }
        }
        catch
        {
            return false;
        }
        return true;
    }   
}
