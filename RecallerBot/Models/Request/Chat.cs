namespace RecallerBot.Models.Request;

internal sealed class Chat
{
    public long Id { get; init; }

    public string Type { get; } = "private";
}
