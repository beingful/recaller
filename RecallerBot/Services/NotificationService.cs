using RecallerBot.Constants;
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

    public async Task SendAsync(Notification notification)
    {
        _logger.LogInformation(LogMessages.StartSendingNotification, notification.Text, notification.ChatId);

        await _botMessageService.SendTextMessageAsync(notification.ChatId, notification.Text);

        _logger.LogInformation(LogMessages.FinishSendingNotification, notification.Text, notification.ChatId);
    }

    public async Task SendExceptFridaysAsync(Notification notification)
    {
        if (DateTime.Today.DayOfWeek != DayOfWeek.Friday) 
        {
            _logger.LogInformation(LogMessages.StartSendingNotification, notification.Text, notification.ChatId);

            await _botMessageService.SendTextMessageAsync(notification.ChatId, notification.Text);

            _logger.LogInformation(LogMessages.FinishSendingNotification, notification.Text, notification.ChatId);
        }
    }
}
