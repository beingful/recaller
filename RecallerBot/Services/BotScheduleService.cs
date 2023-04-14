using Hangfire;
using Hangfire.Storage;
using RecallerBot.Constants;
using RecallerBot.Enums;

namespace RecallerBot.Services;

internal sealed class BotScheduleService
{
    private readonly IStorageConnection _storageConnection;
    private readonly ILogger<BotScheduleService> _logger;

    private const int _maximumRecurringJobsNumber = 5;

    public BotScheduleService(
        IStorageConnection storageConnection,
        ILogger<BotScheduleService> logger)
    {
        _storageConnection = storageConnection;
        _logger = logger;
    }

    public Dictionary<CronExpression, string> CronExpressions =>
        new()
        {
            { CronExpression.EachFriday, Cron.Weekly(DayOfWeek.Thursday, 19, 51)/*"26 19 * * THU"*//*"0 10 * * FRI"*/ },
            { CronExpression.EachLastDayOfMonth, Cron.Weekly(DayOfWeek.Thursday, 19, 52)/*"27 19 * * THU"*//*"0 10 L * *"*/ },
            { CronExpression.Minutely, Cron.Minutely() }
        };

    public void Schedule(long chatId)
    {
        Start(chatId, NotificationMessages.FirstReminder, CronExpressions[CronExpression.EachFriday]);

        Start(chatId, NotificationMessages.LastReminder, CronExpressions[CronExpression.EachFriday]);

        StartExcludingFridays(chatId, NotificationMessages.FirstReminder, CronExpressions[CronExpression.EachLastDayOfMonth]);

        StartExcludingFridays(chatId, NotificationMessages.LastReminder, CronExpressions[CronExpression.EachLastDayOfMonth]);
    }

    public void ScheduleTest(long chatId)
    {
        Start(chatId, NotificationMessages.FirstReminder, CronExpressions[CronExpression.Minutely]);

        Start(chatId, NotificationMessages.LastReminder, CronExpressions[CronExpression.Minutely]);
    }

    public void Unschedule()
    {
        foreach (var recurringJob in _storageConnection.GetRecurringJobs())
        {
            RecurringJob.RemoveIfExists(recurringJob.Id);
        }

        _logger.LogInformation(LogMessages.AllJobsUnscheduled, _storageConnection.GetRecurringJobs().Count);
    }

    private void Start(long chatId, string message, string cron)
    {
        CheckIfCanSchedule();

        RecurringJob.AddOrUpdate<BotNotificationService>(
                recurringJobId: $"{Guid.NewGuid()}",
                methodCall: (notificationService) => notificationService.SendAsync(chatId, message),
                cronExpression: cron);

        _logger.LogInformation(LogMessages.JobScheduled, message, cron);
    }

    private void StartExcludingFridays(long chatId, string message, string cron)
    {
        CheckIfCanSchedule();

        RecurringJob.AddOrUpdate<BotNotificationService>(
                recurringJobId: $"{Guid.NewGuid()}",
                methodCall: (notificationService) => notificationService.SendIfNotFridayAsync(chatId, message),
                cronExpression: cron);

        _logger.LogInformation(LogMessages.JobScheduled, message, cron);
    }

    private void CheckIfCanSchedule()
    {
        if (_storageConnection.GetRecurringJobs().Count == _maximumRecurringJobsNumber)
        {
            throw new Exception(ErrorMessages.ScheduleOverflowing);
        }
    }
}
