using MeChatness.DataAccess;
using MeChatness.Models;

namespace MeChatness.Services;

public static class LoginService
{
    public async static Task<TokenDataModel?> SignInAsync(SignInRequest request)
    {
        if (request == null || request.Password == null || request.Username == null)
            return null;
        LoginDataModel? data = await LoginJsonManager.TryLoadUserAsync(request.Username);
        if (data == null || data.Password == null || data.Id == null)
            return null;
        if (!PasswordService.IsPasswordValid(request.Password, data.Password))
            return null;

        
        return await TokenService.GenerateTokenAsync(data.Id, request.RememberBrowser);
    }

    public async static Task<(bool isToken, string? output, TokenDataModel? tokenData)> SignUpAsync(SignUpRequest signUpRequest)
    {
        if (signUpRequest == null || signUpRequest.Password == null || signUpRequest.Username == null || signUpRequest.Email == null)
            return (false, "Ungültige Anfrage", null);
        string hashedPassword = PasswordService.HashPassword(signUpRequest.Password);
        
        var (isSuccessful, message, id) = await LoginJsonManager.TrySaveUserAsync(signUpRequest);
        if (!isSuccessful || id == null)
            return (false, message, null);
        return (true, null, await TokenService.GenerateTokenAsync(Convert.ToInt32(id)));
    }
}
