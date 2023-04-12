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

    public void Schedule(Action<string> sendMessage)
    {
        StartJob(
            jobId: $"{nameof(NotificationMessages.FirstReminder)}:{nameof(CronExpression.EachFriday)}",
            methodCall: () => sendMessage(NotificationMessages.FirstReminder),
            cronExpression: CronExpressions[CronExpression.EachFriday]);

        StartJob(
            jobId: $"{nameof(NotificationMessages.LastReminder)}:{nameof(CronExpression.EachFriday)}",
            methodCall: () => sendMessage(NotificationMessages.LastReminder),
            cronExpression: CronExpressions[CronExpression.EachFriday]);

        StartJob(
            jobId: $"{nameof(NotificationMessages.FirstReminder)}:{nameof(CronExpression.EachLastDayOfMonth)}",
            methodCall: () =>
            {
                if (DateTime.Today.DayOfWeek != DayOfWeek.Friday)
                {
                    sendMessage(NotificationMessages.FirstReminder);
                }
            },
            cronExpression: CronExpressions[CronExpression.EachLastDayOfMonth]);

        StartJob(
            jobId: $"{nameof(NotificationMessages.LastReminder)}:{nameof(CronExpression.EachLastDayOfMonth)}",
            methodCall: () =>
            {
                if (DateTime.Today.DayOfWeek != DayOfWeek.Friday)
                {
                    sendMessage(NotificationMessages.LastReminder);
                }
            },
            cronExpression: CronExpressions[CronExpression.EachLastDayOfMonth]);
    }

    public void ScheduleTest(Action<string> sendMessage)
    {
        StartJob(
            jobId: $"{nameof(NotificationMessages.FirstReminder)}:{nameof(CronExpression.Minutely)}",
            methodCall: () => sendMessage(NotificationMessages.FirstReminder),
            cronExpression: Cron.Minutely());

        StartJob(
            jobId: $"{nameof(NotificationMessages.LastReminder)}:{nameof(CronExpression.Minutely)}",
            methodCall: () => sendMessage(NotificationMessages.LastReminder),
            cronExpression: Cron.Minutely());
    }

    public void Unschedule()
    {
        foreach (var recurringJob in _storageConnection.GetRecurringJobs())
        {
            RecurringJob.RemoveIfExists(recurringJob.Id);
        }

        _logger.LogInformation(LogMessages.AllJobsUnscheduled, _storageConnection.GetRecurringJobs().Count);
    }

    private void StartJob(string jobId, Action methodCall, string cronExpression)
    {
        if (_storageConnection.GetRecurringJobs().Count == _maximumRecurringJobsNumber)
        {
            throw new Exception(ErrorMessages.ScheduleOverflowing);
        }
        else
        {
            RecurringJob.AddOrUpdate($"{Guid.NewGuid()}", () => methodCall.Invoke(), cronExpression);

            _logger.LogInformation(LogMessages.JobScheduled, jobId);
        }
    }
}
