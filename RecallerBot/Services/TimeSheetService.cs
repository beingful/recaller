using RecallerBot.Constants;
using RecallerBot.Enums;
using RecallerBot.Models.Schedule;

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
            new(message: NotificationMessages.FirstReminder,
                chatId: chatId,
                triggerTime: new Time
                {
                    TimePeriod = TimePeriod.Friday,
                    Hour = 1,
                    Minute = 25
                }),
            new(message: NotificationMessages.LastReminder,
                chatId: chatId,
                triggerTime: new Time
                {
                    TimePeriod = TimePeriod.Friday,
                    Hour = 1,
                    Minute = 26
                })
        });

        _scheduleService.ScheduleAllExceptOnFridays<NotificationService>(new List<Job>
        {
            new(message: NotificationMessages.FirstReminder,
                chatId: chatId,
                triggerTime: new Time
                {
                    TimePeriod = TimePeriod.Daily,
                    Hour = 1,
                    Minute = 25
                }),
            new(message: NotificationMessages.LastReminder,
                chatId: chatId,
                triggerTime: new Time
                {
                    TimePeriod = TimePeriod.Daily,
                    Hour = 1,
                    Minute = 26
                })
        });
    }

    public void StopNotifying(long chatId) =>
        _scheduleService.UnscheduleByEndpoint(chatId.ToString());
}
