using Hangfire;
using Hangfire.Storage;
using RecallerBot.Constants;
using RecallerBot.Enums;

namespace RecallerBot.Services;

internal sealed class ScheduleService
{
    private readonly IStorageConnection _storageConnection;
    private readonly ILogger<ScheduleService> _logger;
    private const int _maximumRecurringJobsNumber = 4;

    public ScheduleService(
        IStorageConnection storageConnection,
        ILogger<ScheduleService> logger)
    {
        _storageConnection = storageConnection;
        _logger = logger;
    }

    public Dictionary<CronExpression, string> CronExpressions =>
        new()
        {
            { CronExpression.EachFriday, "0 10 * * 5" },
            { CronExpression.EachLastDayOfMonth, "0 10 L * *" },
            { CronExpression.Minutely, Cron.Minutely() }
        };

    public void Schedule(long chatId)
    {
        StartOnFriday(chatId, NotificationMessages.FirstReminder);

        StartOnFriday(chatId, NotificationMessages.LastReminder);

        StartOnLastDayOfMonth(chatId, NotificationMessages.FirstReminder);

        StartOnLastDayOfMonth(chatId, NotificationMessages.LastReminder);
    }

    public void ScheduleTest(long chatId)
    {
        StartMinutely(chatId, NotificationMessages.FirstReminder);

        StartMinutely(chatId, NotificationMessages.LastReminder);
    }

    public void Unschedule()
    {
        foreach (var recurringJob in _storageConnection.GetRecurringJobs())
        {
            RecurringJob.RemoveIfExists(recurringJob.Id);
        }

        _logger.LogInformation(LogMessages.AllJobsUnscheduled, _storageConnection.GetRecurringJobs().Count);
    }

    private void StartOnFriday(long chatId, string message)
    {
        CheckIfCanSchedule();

        RecurringJob.AddOrUpdate<BotNotificationService>(
                methodCall: (notificationService) => notificationService.Send(chatId, message),
                cronExpression: CronExpressions[CronExpression.EachFriday]);

        _logger.LogInformation(LogMessages.JobScheduled, message, CronExpressions[CronExpression.EachFriday]);
    }

    private void StartOnLastDayOfMonth(long chatId, string message)
    {
        CheckIfCanSchedule();

        RecurringJob.AddOrUpdate<BotNotificationService>(
                methodCall: (notificationService) => notificationService.Send(chatId, message),
                cronExpression: CronExpressions[CronExpression.EachLastDayOfMonth]);

        _logger.LogInformation(LogMessages.JobScheduled, message, CronExpressions[CronExpression.EachLastDayOfMonth]);
    }

    private void StartMinutely(long chatId, string message)
    {
        CheckIfCanSchedule();

        RecurringJob.AddOrUpdate<BotNotificationService>(
                methodCall: (notificationService) => notificationService.Send(chatId, message),
                cronExpression: CronExpressions[CronExpression.Minutely]);

        _logger.LogInformation(LogMessages.JobScheduled, message, CronExpressions[CronExpression.Minutely]);
    }

    private void CheckIfCanSchedule()
    {
        if (_storageConnection.GetRecurringJobs().Count == _maximumRecurringJobsNumber)
        {
            throw new Exception(ErrorMessages.ScheduleOverflowing);
        }
    }
}
