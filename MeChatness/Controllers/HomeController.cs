using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MeChatness.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Redirect("/Login/SignIn");
    }
}
