using RecallerBot.Interfaces;

namespace RecallerBot.Services;

internal sealed class BotNotificationService
{
    private readonly IBotMessageService _botMessageService;

    public BotNotificationService(IBotMessageService botMessageService) =>
        _botMessageService = botMessageService;

    public async Task Send(long chatId, string message) =>
        await _botMessageService.SendTextMessageAsync(chatId, message);

    public async Task SendIfNotFriday(long chatId, string message)
    {
        if (DateTime.Today.DayOfWeek != DayOfWeek.Friday)
        {
            await _botMessageService.SendTextMessageAsync(chatId, message);
        }
    }
}
