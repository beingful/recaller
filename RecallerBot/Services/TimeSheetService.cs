using RecallerBot.Constants;
using RecallerBot.Enums;
using RecallerBot.Models;

namespace RecallerBot.Services;

internal class TimeSheetService
{
    private readonly ScheduleService _scheduleService;

    public TimeSheetService(ScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public void StartNotifying(long chatId)
    {
        _scheduleService.ScheduleAll<NotificationService>(new List<Job>
        {
            new(message: NotificationMessages.FirstReminder, chatId: chatId, triggerTime: TimePeriod.OnFridays),
            new(message: NotificationMessages.LastReminder, chatId: chatId, triggerTime: TimePeriod.OnFridays)
        });

        _scheduleService.ScheduleAllExceptOnFridays<NotificationService>(new List<Job>
        {
            new(message: NotificationMessages.FirstReminder, chatId: chatId, triggerTime: TimePeriod.OnLastDayOfMonth),
            new(message: NotificationMessages.LastReminder, chatId: chatId, triggerTime: TimePeriod.OnLastDayOfMonth)
        });
    }

    public void UnscheduleNotifications(long chatId) =>
        _scheduleService.UnscheduleByEndpoint(chatId);
}
