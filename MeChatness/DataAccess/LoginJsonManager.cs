using MeChatness.Models;
using System.Text.Json;

namespace MeChatness.DataAccess;

public static class LoginJsonManager
{
    const string RouteToJson = "~/../Data/LoginDataTestFile.json";

    public async static Task<SignInDataModel?> TryLoadUserAsync(string username)
    {
        List<SignInDataModel>? dataList = null;
        SignInDataModel? signInData = null;
        try
        {
            string json = await File.ReadAllTextAsync(RouteToJson);
            dataList = JsonSerializer.Deserialize<List<SignInDataModel>>(json);
            signInData = dataList.First(u => u.Username == username);
        }
        catch
        {
            return null;
        }

        return signInData;
    }
    public async static Task<List<SignInDataModel>?> TryLoadAllUsersAsync()
    {
        List<SignInDataModel>? signInDataList = null;
        try
        {
            string json = await File.ReadAllTextAsync(RouteToJson);
            signInDataList = JsonSerializer.Deserialize<List<SignInDataModel>>(json);
        }
        finally { } // wird immer ausgeführt -> kein Return enthalten / catch wäre auch möglich

        return signInDataList;
    }
}
