using RecallerBot.Models;
using Telegram.Bot.Types;

namespace RecallerBot.Services;

internal sealed class MessageValidationService
{
    private readonly BotConfiguration _botCofiguration;

    public MessageValidationService(BotConfiguration botCofiguration) =>
        _botCofiguration = botCofiguration;

    public bool IsChatAllowed(Message message) =>
        _botCofiguration.AllowedChats.ContainsValue($"{message.Chat.Id}");

    public bool IsCommandRecognized(Message message) =>
        _botCofiguration.Commands.ContainsKey(message.Text);

    public Enums.BotCommand MapToCommand(Message message) =>
        _botCofiguration.Commands.GetValueOrDefault(message.Text);
}
