#nullable disable

namespace RecallerBot.Models.Configuration;

internal sealed class Bot
{
    public string Token { get; init; }

    public string Username { get; init; }

    public string HostAddress { get; init; }

    public Dictionary<string, string> AllowedChats { get; init; }

    public string CreatorUserId { get; init; }

    public string EscapedBotToken => Token?.Replace(':', '_');

    public string WebhookUrl => $"{HostAddress}/bot/{EscapedBotToken}";
}
