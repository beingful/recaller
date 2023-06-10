using RecallerBot.Enums;

#nullable disable

namespace RecallerBot.Models;

internal sealed class Job
{
    public readonly string Id;

    public readonly Notification Notification;

    public readonly TimePeriod TriggerTime;

    public Job(string message, long chatId, TimePeriod triggerTime)
    {
        Id = $"{chatId}-{Guid.NewGuid()}";
        Notification = new(chatId, message);
        TriggerTime = triggerTime;
    }
}
