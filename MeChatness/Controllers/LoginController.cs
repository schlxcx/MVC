using Microsoft.AspNetCore.Mvc;
using MeChatness.Models;
using MeChatness.Services;

namespace MeChatness.Controllers;

public class LoginController : Controller
{
    public IActionResult SignUp()
    {
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
        string? token = await LoginService.SignInAsync(request);
        if (token == null)
            return RedirectToAction("SignIn", "Login", new { IsLoginInvalid = true }); // new { variable = value } ist ein annonymes Objekt -> das braucht es für den Parameter
        return RedirectToAction("SignUp", "Login"); // muss geändert werden
    }
}
