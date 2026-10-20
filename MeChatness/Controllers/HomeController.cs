using MeChatness.Models;
using MeChatness.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System.ComponentModel;

namespace MeChatness.Controllers;

public class HomeController : Controller
{
    private readonly string _connString;

    public HomeController(IConfiguration config) { _connString = config.GetConnectionString("Default")!; }

    public async Task<IActionResult> Index()
    {
        if (!await IsValidToken())
            return Redirect("/Login/SignIn");

        return Redirect("/Home/Chat");
        //return RedirectToAction("DbTest");
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
               Expires
           });
        }

        return true;
    }

    //public IActionResult DbTest()
    //{
    //    var namen = new List<string>();


    //    using var conn = new MySqlConnection(_connString);

    //    conn.Open();


    //    using var cmd = new MySqlCommand("SELECT output FROM test", conn);

    //    using var reader = cmd.ExecuteReader();

    //    while (reader.Read())
    //    {
    //        namen.Add(reader.GetString(0));

    //    }
    //    return View(namen);

    //}
}
