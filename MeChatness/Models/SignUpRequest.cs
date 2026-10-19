namespace MeChatness.Models;

public class SignUpRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public bool AgreeToTerms { get; set; }
}
    