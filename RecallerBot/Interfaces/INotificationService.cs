using RecallerBot.Models;

namespace RecallerBot.Interfaces;

internal interface INotificationService
{
    public Task SendAsync(Notification notification);
}
