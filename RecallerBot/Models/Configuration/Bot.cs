#nullable disable

namespace RecallerBot.Models.Configuration;

internal sealed class Bot
{
    public string Token { get; init; }

    public string Username { get; init; }

    public string HostAddress { get; init; }

    public long CreatorUserId { get; init; }

    public Dictionary<string, long> AllowedChats { get; init; }

    public long ProdChat => AllowedChats[nameof(ProdChat)];

    public string EscapedBotToken => Token?.Replace(':', '_');

    public string WebhookUrl => $"{HostAddress}/bot/{EscapedBotToken}";
}
