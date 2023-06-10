namespace RecallerBot.Models;

#nullable disable

internal sealed class BotConfiguration
{
    public string BotToken { get; init; }

    public string BotUsername { get; init; }

    public string HostAddress { get; init; }

    public string EscapedBotToken => BotToken?.Replace(':', '_');
}
