#nullable disable

namespace RecallerBot.Models.Request;

internal sealed class Message
{
    public int MessageId { get; } = 1;

    public Sender From { get; init; }

    public Chat Chat { get; init; }

    public long Date { get; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public string Text { get; init; }
}
