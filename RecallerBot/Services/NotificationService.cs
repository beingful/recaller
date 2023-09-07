using RecallerBot.Constants;
using RecallerBot.Extensions;
using RecallerBot.Interfaces;
using RecallerBot.Models;
using RecallerBot.Providers;

namespace RecallerBot.Services;

internal class NotificationService : IConditionedNotificationService
{
    private readonly IBotMessageService _botMessageService;
    private readonly ILogger _logger;

    public NotificationService(
        IBotMessageService botMessageService,
        ILogger<NotificationService> logger)
    {
        _botMessageService = botMessageService;
        _logger = logger;
    }

    public async Task SendAsync(Notification notification) =>
        await _botMessageService.SendTextMessageAsync(notification.ChatId, notification.Text);

    public async Task SendExceptFridaysAsync(Notification notification)
    {
        DateTime today = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneProvider.GetTimeZone());

        if (today.IsLastWeekdayOfMonth() && today.IsNotFriday())
        {
            await _botMessageService.SendTextMessageAsync(notification.ChatId, notification.Text);
        }
        else
        {
            _logger.LogInformation(LogMessages.TodayIsFridayNotLastWeekdayOfMonth, today);
        }
    }
}
