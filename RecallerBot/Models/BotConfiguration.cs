using RecallerBot.Enums;

namespace RecallerBot.Models;

#nullable disable

internal sealed class BotConfiguration
{
    public string BotToken { get; init; }

    public string BotUsername { get; init; }

    public string HostAddress { get; init; }

    public Dictionary<string, string> AllowedChats { get; init; }

    public string EscapedBotToken => BotToken?.Replace(':', '_');

    public Dictionary<string, BotCommand> Commands => new()
    {
        { $"/start{BotUsername}", BotCommand.Start },
        { $"/stop{BotUsername}", BotCommand.Stop },
        { $"/test{BotUsername}", BotCommand.Test }
    };
}
