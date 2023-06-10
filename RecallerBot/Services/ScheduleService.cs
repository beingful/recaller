using Hangfire;
using Hangfire.Storage;
using RecallerBot.Constants;
using RecallerBot.Enums;
using RecallerBot.Interfaces;
using RecallerBot.Models.Schedule;

namespace RecallerBot.Services;

internal sealed class ScheduleService
{
    private readonly IStorageConnection _storageConnection;
    private readonly ILogger<ScheduleService> _logger;

    private const int _maximumRecurringJobsNumber = 5;

    public ScheduleService(IStorageConnection storageConnection, ILogger<ScheduleService> logger)
    {
        _storageConnection = storageConnection;
        _logger = logger;
    }

    public Dictionary<TimePeriod, Func<Time, string>> CronExpressions => new()
    {
        { TimePeriod.Friday, (Time time) => Cron.Weekly(DayOfWeek.Saturday, time.Hours, time.Minutes)/*"26 19 * * THU"*//*"0 10 * * FRI"*/ },
        { TimePeriod.LastDayOfMonth, (Time time) => $"{time.Minutes} {time.Hours} * * SAT" }, //*"0 10 L * *"*/ },
        { TimePeriod.MimuteInterval, (Time time) => $"*/{time.MinuteInterval} * * * *" }
    };

    public void ScheduleAll<T>(List<Job> jobs) where T : INotificationService
    {
        jobs.ForEach(job => Schedule<T>(job));
    }

    public void Schedule<T>(Job job) where T : INotificationService
    {
        CheckIfCanSchedule();

        RecurringJob.AddOrUpdate<T>(
                recurringJobId: job.Id,
                methodCall: (notificationService) => notificationService.SendAsync(job.Notification),
                cronExpression: CronExpressions[job.TriggerTime.TimePeriod](job.TriggerTime));

        _logger.LogInformation(LogMessages.JobScheduled, job.Notification.Text, job.TriggerTime.TimePeriod);
    }

    public void ScheduleAllExceptOnFridays<T>(List<Job> jobs) where T : IConditionedNotificationService
    {
        jobs.ForEach(job => ScheduleExceptOnFridays<T>(job));
    }

    public void UnscheduleByEndpoint(long endpoint)
    {
        _storageConnection.GetRecurringJobs().ForEach(job =>
        {
            if (job.Id.Contains(endpoint.ToString()))
            {
                RecurringJob.RemoveIfExists(job.Id);
            }
        });

        _logger.LogInformation(LogMessages.JobsWithEndpointUnscheduled, endpoint, _storageConnection.GetRecurringJobs().Count);
    }

    private void ScheduleExceptOnFridays<T>(Job job) where T : IConditionedNotificationService
    {
        CheckIfCanSchedule();

        RecurringJob.AddOrUpdate<T>(
                recurringJobId: job.Id,
                methodCall: (notificationService) => notificationService
                .SendExceptFridaysAsync(job.Notification),
                cronExpression: CronExpressions[job.TriggerTime.TimePeriod](job.TriggerTime));

        _logger.LogInformation(LogMessages.JobScheduled, job.Notification.Text, job.TriggerTime.TimePeriod);
    }

    private void CheckIfCanSchedule()
    {
        if (_storageConnection.GetRecurringJobs().Count == _maximumRecurringJobsNumber)
        {
            throw new Exception(ErrorMessages.ScheduleOverflowing);
        }
    }
}
