namespace Chat.Models.Login;

public class TokenModel
{
    public required string Token { get; set; }
    public DateTime Expiration { get; set; }
}

