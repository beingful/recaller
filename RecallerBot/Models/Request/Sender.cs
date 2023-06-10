namespace RecallerBot.Models.Request;

internal sealed class Sender
{
    public long Id { get; init; }

    public bool IsBot { get; } = false;

    public string FirstName { get; } = string.Empty;
}
