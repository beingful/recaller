#nullable disable

namespace RecallerBot.Models.Schedule;

internal sealed class Job
{
    public readonly string Id;

    public readonly Notification Notification;

    public readonly Time TriggerTime;

    public Job(string message, long chatId, Time triggerTime)
    {
        Id = $"{chatId}-{message}-{triggerTime.TimePeriod}";
        Notification = new(chatId, message);
        TriggerTime = triggerTime;
    }
}
