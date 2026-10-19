using Microsoft.AspNetCore.Mvc;
using MeChatness.Models;
using MeChatness.Services;

namespace MeChatness.Controllers;

public class LoginController : Controller
{
    public IActionResult SignUp(List<SignUpError>? errors = null)
    {
        if (errors != null && errors.Count > 0)
        {
            foreach (var error in errors)
            {
                switch (error)
                {
                    case SignUpError.invalidRequest:
                        ViewData["invalidRequest"] = "Ungültige Anfrage";
                        break;
                    case SignUpError.passwordTooShort:
                        ViewData["passwordTooShort"] = "Passwort ist zu kurz";
                        break;
                    case SignUpError.invalidEmail:
                        ViewData["invalidEmail"] = "Ungültige E-Mail-Adresse";
                        break;
                    case SignUpError.invalidUsername:
                        ViewData["invalidUsername"] = "Ungültiger Benutzername";
                        break;
                    case SignUpError.usernameAlreadyTaken:
                        ViewData["usernameAlreadyTaken"] = "Benutzername ist bereits vergeben";
                        break;
                    default:
                        ViewData["ErrorSignUp"] = "Ein Fehler ist aufgetreten. Bitte versuchen Sie es später erneut.";
                        break;
                }
            }
        }
        return View();
    }

    public IActionResult SignIn(bool IsLoginInvalid = false)
    {
        if (IsLoginInvalid)
            ViewData["ErrorSignIn"] = "Benutzername oder Passwort ist falsch";
        return View();
    }

    public async Task<IActionResult> CheckSignIn([FromForm] SignInRequest request)
    {
        List<TokenDataModel>? tokens = await AuthService.SignInAsync(request);
        if (tokens == null || tokens.Count == 0)
            return RedirectToAction("SignIn", "Login", new { IsLoginInvalid = true }); // new { variable = value } ist ein annonymes Objekt -> das braucht es für den Parameter

        foreach (var token in tokens)
        {
            Response.Cookies.Append(token.TokenType.ToString(), token.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = token.Expiration
            });
        }

        return RedirectToAction("Index", "Home"); // muss geändert werden
    }

    public async Task<IActionResult> SignUpUser([FromForm] SignUpRequest signUpRequest)
    {
        var (isToken, outputs, tokenData) = await AuthService.SignUpAsync(signUpRequest);
        if (!isToken || tokenData == null || tokenData.Token == null)
        {
            return RedirectToAction("SignUp", "Login", new { errors = outputs });
        }

        Response.Cookies.Append(TokenType.AccessToken.ToString(), tokenData.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = tokenData.Expiration
        });

        return RedirectToAction("Index", "Home");
    }
}
