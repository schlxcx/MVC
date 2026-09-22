using MeChatness.DataAccess;
using MeChatness.Models;

namespace MeChatness.Services;

public static class LoginService
{
    public async static Task<string?> SignInAsync(SignInRequest request)
    {
        if (request == null || request.Password == null || request.Username == null)
            return null;
        SignInDataModel? data = await LoginJsonManager.TryLoadUserAsync(request.Username);
        if (data == null || data.Password == null)
            return null;
        if (!PasswordService.IsPasswordValid(request.Password, data.Password))
            return null;
        return await TokenService.GenerateTokenAsync();
    }
}
