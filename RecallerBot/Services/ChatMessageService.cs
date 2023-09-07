using RecallerBot.Models.Configuration;
using Telegram.Bot.Types;

namespace RecallerBot.Services;

internal sealed class ChatMessageService
{
    private readonly Dictionary<string, long> _allowedChats;
    private readonly Dictionary<string, Enums.BotCommand> _allowedCommands;

    public ChatMessageService(Bot bot)
    {
        _allowedChats = bot.AllowedChats;
        _allowedCommands = new()
        {
            { $"/start{bot.Username}", Enums.BotCommand.Start },
            { $"/stop{bot.Username}", Enums.BotCommand.Stop },
            { $"/awake{bot.Username}",  Enums.BotCommand.Awake }
        };
    }

    public long TestChat => _allowedChats[nameof(TestChat)];

    public bool IsChatAllowed(Message message) =>
        _allowedChats.ContainsValue(message.Chat.Id);

    public bool IsCommandRecognized(Message message) =>
        message.Text != null && _allowedCommands.ContainsKey(message.Text);

    public Enums.BotCommand ToBotCommand(Message message) =>
        message.Text != null && _allowedCommands.TryGetValue(message.Text, out var command)
            ? command
            : Enums.BotCommand.Undefined;

    public string ToMessage(Enums.BotCommand botCommand) =>
        _allowedCommands
            .FirstOrDefault(command => command.Value.Equals(botCommand))
            .Key;
}
