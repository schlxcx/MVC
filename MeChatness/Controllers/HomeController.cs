using MeChatness.Models;
using Microsoft.AspNetCore.Mvc;
using MeChatness.Services;

namespace MeChatness.Controllers;

public class HomeController : Controller
{
    public async Task<IActionResult> Index()
    {
        if ( ! await IsValidToken())
            return Redirect("/Login/SignIn");
        
        return Redirect("/Home/Chat");
    }

    public IActionResult Chat()
    {
        return View();
    }

    private async Task<bool> IsValidToken()
    {
        Request.Cookies.TryGetValue(TokenType.AccessToken.ToString(), out string? accessToken);
        Request.Cookies.TryGetValue(TokenType.RefreshToken.ToString(), out string? refreshToken);

        if (accessToken == null)
            return false;

        (bool isValid, List<TokenDataModel>? tokenList) = await AuthService.IsVerified(string.IsNullOrWhiteSpace(refreshToken) ? new Dictionary<string, TokenType> { { accessToken, TokenType.AccessToken } } : new Dictionary<string, TokenType> { { accessToken, TokenType.AccessToken }, { refreshToken, TokenType.RefreshToken } });

        if (!isValid)
        {
            Response.Cookies.Delete("Token");
            return false;
        }

        if (tokenList == null || tokenList.Count == 0)
            return true;

        foreach (var token in tokenList)
        {
           Response.Cookies.Append(token.TokenType.ToString(), token.Token, new CookieOptions
           {
               HttpOnly = true,
               Secure = true,
               SameSite = SameSiteMode.Lax,
               Expires = token.Expiration
           });
        }

        return true;
    }
}
