using Microsoft.AspNetCore.Mvc;

namespace MeChatness.Controllers;

public class LoginController : Controller
{
    public IActionResult SignUp()
    {
        return View();
    }

    public IActionResult SignIn()
    {
        ViewData["ErrorSignIn"] = "Benutzername oder Passwort ist falsch";
        return View();
    }
}
