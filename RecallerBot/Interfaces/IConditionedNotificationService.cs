using RecallerBot.Models;

namespace RecallerBot.Interfaces;

internal interface IConditionedNotificationService : INotificationService
{
    public Task SendExceptFridaysAsync(Notification notification);
}
