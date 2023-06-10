using RecallerBot.Models;

namespace RecallerBot.Interfaces;

internal interface IConditionedNotificationService : INotificationService
{
    public Task SendByConditionAsync(Notification notification, Func<bool> condition);
}
