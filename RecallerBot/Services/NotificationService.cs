using RecallerBot.Interfaces;
using RecallerBot.Models;

namespace RecallerBot.Services;

internal class NotificationService : IConditionedNotificationService
{
    private readonly IBotMessageService _botMessageService;

    public NotificationService(IBotMessageService botMessageService) =>
        _botMessageService = botMessageService;

    public async Task SendAsync(Notification notification) =>
        await _botMessageService.SendTextMessageAsync(notification.ChatId, notification.Text);

    public async Task SendExceptFridaysAsync(Notification notification)
    {
        if (DateTime.Today.DayOfWeek != DayOfWeek.Friday) 
        {
            await _botMessageService.SendTextMessageAsync(notification.ChatId, notification.Text);
        }
    }
}
