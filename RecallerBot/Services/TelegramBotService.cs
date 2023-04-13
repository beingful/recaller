using RecallerBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace RecallerBot.Services;

internal sealed class TelegramBotService : IBotMessageService, IBotEndpointService
{
    private readonly ITelegramBotClient _botClient;

    public TelegramBotService(ITelegramBotClient botClient) =>
        _botClient = botClient;

    async Task IBotMessageService.SendTextMessageAsync(long chatId, string message) =>
        await _botClient.SendTextMessageAsync(chatId, message);

    async Task IBotEndpointService.SetWebhookAsync(string url, CancellationToken cancellationToken) =>
        await _botClient.SetWebhookAsync(
            url: url,
            allowedUpdates: Array.Empty<UpdateType>(),
            cancellationToken: cancellationToken);

    async Task IBotEndpointService.DeleteWebhookAsync(CancellationToken cancellationToken) =>
        await _botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
}
