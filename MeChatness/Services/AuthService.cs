using MeChatness.DataAccess;
using MeChatness.Models;

namespace MeChatness.Services;

public class AuthService
{
    private readonly IConfiguration _config;
    private static LoginSqlManager LoginSqlManager;
    private static TokenService TokenService;

    public AuthService(IConfiguration config) 
    {
        _config = config;
        LoginSqlManager = new LoginSqlManager(config);
        TokenService = new TokenService(config);
    }

    public async Task<List<TokenDataModel>?> SignInAsync(SignInRequest request)
    {
        if (request == null || request.Password == null || request.Username == null)
            return null;
        //LoginDataModel? data = await LoginJsonManager.TryLoadUserAsync(request.Username);
        LoginDataModel? data = await LoginSqlManager.GetUserByUsernameAsync(request.Username);
        if (data == null || data.Password == null || data.Id == null)
            return null;
        if (!PasswordService.IsPasswordValid(request.Password, data.Password))
            return null;

        return await TokenService.GenerateTokenAsync(data.Id, request.RememberBrowser);
    }

    public async Task<(bool isToken, List<SignUpError>? errors, TokenDataModel? tokenData)> SignUpAsync(SignUpRequest signUpRequest)
    {
        if (signUpRequest == null)
            return (false, new List<SignUpError> { SignUpError.invalidRequest }, null);

        bool isInvalidEmail = signUpRequest.Email == null || !signUpRequest.Email.Contains("@");
        bool isPasswordTooShort = signUpRequest.Password == null || signUpRequest.Password.Length < 8;
        bool isUsernameInvalid = signUpRequest.Username == null || signUpRequest.Username.Length < 3;
        if (isPasswordTooShort || isInvalidEmail || isUsernameInvalid)
        {
            List<SignUpError> validateErrors = new List<SignUpError>();
            if (isPasswordTooShort)
                validateErrors.Add(SignUpError.passwordTooShort);
            if (isInvalidEmail)
                validateErrors.Add(SignUpError.invalidEmail);
            if (isUsernameInvalid)
                validateErrors.Add(SignUpError.invalidUsername);
            return (false, validateErrors, null);
        }

        string hashedPassword = PasswordService.HashPassword(signUpRequest.Password);

        LoginDataModel user = new LoginDataModel { Username = signUpRequest.Username, Email = signUpRequest.Email, Password = signUpRequest.Password };
        
        var (isSuccessful, error, id) = await LoginSqlManager.TryCreateUser(user);
        if (!isSuccessful || id == null)
            return (false, new List<SignUpError>() { error }, null);

        List<TokenDataModel>? tokens = await TokenService.GenerateTokenAsync(id.Value);

        if (tokens == null || tokens.Count == 0)
            return (false, new List<SignUpError> { SignUpError.aMistakeHappened }, null);

        return (true, null, tokens.FirstOrDefault());
    }

    public async Task<(bool isTokenValid, List<TokenDataModel>? tokenData)> IsVerified(Dictionary<string, TokenType> tokens) => await TokenService.IsTokenValidAsync(tokens);
}
