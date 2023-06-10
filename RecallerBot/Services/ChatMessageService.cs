using RecallerBot.Constants;
using RecallerBot.Models;
using Telegram.Bot.Types;

namespace RecallerBot.Services;

internal sealed class ChatMessageService
{
    private readonly Dictionary<string, string> _allowedChats;
    private readonly Dictionary<string, Enums.BotCommand> _allowedCommands;

    public ChatMessageService(BotConfiguration botConfiguration, IConfiguration configuration)
    {
        _allowedChats = configuration
                            .GetSection(ConfigurationConstants.AllowedChats)
                            .Get<Dictionary<string, string>>()!;

        _allowedCommands = new()
        {
            { $"/start{botConfiguration.BotUsername}", Enums.BotCommand.Start },
            { $"/stop{botConfiguration.BotUsername}", Enums.BotCommand.Stop },
            { $"/test{botConfiguration.BotUsername}", Enums.BotCommand.AlarmClock }
        };
    }

    public long TestChat => Convert.ToInt64(_allowedChats[Chats.TestChat]);

    public bool IsChatAllowed(Message message) =>
        _allowedChats.ContainsValue($"{message.Chat.Id}");

    public bool IsCommandRecognized(Message message) =>
        message.Text != null && _allowedCommands.ContainsKey(message.Text);

    public Enums.BotCommand MapToCommand(Message message) =>
        message.Text != null && _allowedCommands.TryGetValue(message.Text, out var command)
        ? command
        : Enums.BotCommand.Undefined;
}
