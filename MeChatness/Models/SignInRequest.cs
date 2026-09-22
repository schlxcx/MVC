namespace MeChatness.Models;

public class SignInRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool RememberBrowser { get; set; } = false;
}
