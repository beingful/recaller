using RecallerBot.Interfaces;
using RecallerBot.Models;

namespace RecallerBot.Services;

internal class NotificationService : INotificationService, IConditionedNotificationService
{
    private readonly IBotMessageService _botMessageService;

    public NotificationService(IBotMessageService botMessageService) =>
        _botMessageService = botMessageService;

    public async Task SendAsync(Notification notification) =>
        await _botMessageService.SendTextMessageAsync(notification.ChatId, notification.Text);

    public async Task SendByConditionAsync(Notification notification, Func<bool>? sendIfCallback = null)
    {
        if (sendIfCallback == null || sendIfCallback.Invoke()) 
        {
            await _botMessageService.SendTextMessageAsync(notification.ChatId, notification.Text);
        }
    }
}
