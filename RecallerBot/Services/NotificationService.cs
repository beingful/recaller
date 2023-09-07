using RecallerBot.Extensions;
using RecallerBot.Interfaces;
using RecallerBot.Models;
using RecallerBot.Providers;

namespace RecallerBot.Services;

internal class NotificationService : IConditionedNotificationService
{
    private readonly IBotMessageService _botMessageService;

    public NotificationService(IBotMessageService botMessageService)
    {
        _botMessageService = botMessageService;
    }

    public async Task SendAsync(Notification notification) =>
        await _botMessageService.SendTextMessageAsync(notification.ChatId, notification.Text);

    public async Task SendExceptFridaysAsync(Notification notification)
    {
        DateTime today = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneProvider.GetTimeZone());
        await _botMessageService.SendTextMessageAsync(notification.ChatId, notification.Text);

        //if (today.IsLastWeekdayOfMonth() && today.IsNotFriday())
        //{
        //    await _botMessageService.SendTextMessageAsync(notification.ChatId, notification.Text);
        //}
    }
}
