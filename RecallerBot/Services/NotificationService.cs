using RecallerBot.Interfaces;
using RecallerBot.Models;
using RecallerBot.Providers;
using System;

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
        if (TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneProvider.GetTimeZone()).DayOfWeek != DayOfWeek.Friday) 
        {
            await _botMessageService.SendTextMessageAsync(notification.ChatId, notification.Text);
        }
    }
}
