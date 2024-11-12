namespace Chat.Entities;

public class Message
{
    public int Id { get; set; }
    public required string Sender { get; set; }
    public required string Content { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public required string ChatId { get; set; }
}
