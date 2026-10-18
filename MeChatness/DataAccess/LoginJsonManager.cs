using MeChatness.Models;
using System.Text.Json;

namespace MeChatness.DataAccess;

public static class LoginJsonManager
{
    const string RouteToJson = "~/../Data/LoginDataTestFile.json";

    public async static Task<LoginDataModel?> TryLoadUserAsync(string username)
    {
        List<LoginDataModel>? dataList = null;
        LoginDataModel? signInData = null;
        try
        {
            string json = await File.ReadAllTextAsync(RouteToJson);
            dataList = JsonSerializer.Deserialize<List<LoginDataModel>>(json);
            signInData = dataList.First(u => u.Username == username);
        }
        catch
        {
            return null;
        }

        return signInData;
    }
    public async static Task<List<LoginDataModel>?> TryLoadAllUsersAsync()
    {
        List<LoginDataModel>? signInDataList = null;
        try
        {
            string json = await File.ReadAllTextAsync(RouteToJson);
            signInDataList = JsonSerializer.Deserialize<List<LoginDataModel>>(json);
        }
        finally { } // wird immer ausgeführt -> kein Return enthalten / catch wäre auch möglich

        return signInDataList;
    }

    public async static Task<(bool isSuccessful, string message, int? id)> TrySaveUserAsync(SignUpRequest signUpRequest)
    {
        List<LoginDataModel>? dataList = await TryLoadAllUsersAsync() ?? new List<LoginDataModel>();

        if (string.IsNullOrWhiteSpace(signUpRequest.Username) || string.IsNullOrWhiteSpace(signUpRequest.Password) || string.IsNullOrWhiteSpace(signUpRequest.Email))
        {
            return (false, "Username, password and email cannot be empty", null);
        }

        if (dataList.Any(u => u.Username == signUpRequest.Username))
        {
            return (false, "User already exists", null);
        }

        int id = dataList.Count > 0 ? dataList.Max(u => u.Id) + 1 : 1;

        LoginDataModel newUser = new LoginDataModel
        {
            Username = signUpRequest.Username,
            Password = hashedPassword,
            Email = signUpRequest.Email
        };

        dataList.Add(newUser);
        string json = JsonSerializer.Serialize(dataList, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(RouteToJson, json);
        return (true, "User created successfully", id);
    }
}
