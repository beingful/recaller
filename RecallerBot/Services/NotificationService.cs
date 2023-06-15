using RecallerBot.Interfaces;
using RecallerBot.Models;

namespace RecallerBot.Services;

internal class NotificationService : IConditionedNotificationService
{
    private readonly IBotMessageService _botMessageService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IBotMessageService botMessageService, ILogger<NotificationService> logger)
    {
        _botMessageService = botMessageService;
        _logger = logger;
    }

    public async Task SendAsync(Notification notification) =>
        await _botMessageService.SendTextMessageAsync(notification.ChatId, notification.Text);

    public async Task SendExceptFridaysAsync(Notification notification)
    {
        if (DateTime.Today.DayOfWeek != DayOfWeek.Thursday) 
        {
            await _botMessageService.SendTextMessageAsync(notification.ChatId, notification.Text);
        }
    }
}
