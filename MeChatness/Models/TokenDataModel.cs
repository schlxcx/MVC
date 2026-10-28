namespace MeChatness.Models;

public class TokenDataModel
{
    public int Id { get; set; }
    public string? Token { get; set; }
    public TokenType TokenType { get; set; }
    public DateTime Expiration { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserId { get; set; }
}
