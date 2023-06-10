#nullable disable

namespace RecallerBot.Models.Request;

internal sealed class Update
{
    public int UpdateId { get; } = 1;

    public Message Message { get; init; }
}
