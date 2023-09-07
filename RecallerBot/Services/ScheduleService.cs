using Hangfire;
using Hangfire.Storage;
using RecallerBot.Constants;
using RecallerBot.Enums;
using RecallerBot.Interfaces;
using RecallerBot.Models.Schedule;
using RecallerBot.Providers;

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
        { TimePeriod.OnFridays, (Time time) => Cron.Minutely() },//Cron.Weekly(DayOfWeek.Friday, time.Hour, time.Minute) },
        { TimePeriod.Daily, (Time time) => Cron.Minutely() },
        { TimePeriod.MinuteInterval, (Time time) => $"*/{time.MinuteInterval} * * * *" }
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
                cronExpression: CronExpressions[job.TriggerTime.TimePeriod](job.TriggerTime),
                options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneProvider.GetTimeZone()
                });

        _logger.LogInformation(LogMessages.JobScheduled, job.Notification.Text, job.TriggerTime.TimePeriod);
        _logger.LogInformation(LogMessages.JobsNumber, _storageConnection.GetRecurringJobs().Count);
    }

    public void ScheduleAllExceptOnFridays<T>(List<Job> jobs) where T : IConditionedNotificationService
    {
        jobs.ForEach(job => ScheduleExceptOnFridays<T>(job));
    }

    public void UnscheduleByEndpoint(string endpoint)
    {
        CheckIfCanUnschedule(endpoint);

        _storageConnection.GetRecurringJobs().ForEach(job =>
        {
            if (job.Id.Contains(endpoint))
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
                methodCall: (notificationService) => notificationService.SendExceptFridaysAsync(job.Notification),
                cronExpression: CronExpressions[job.TriggerTime.TimePeriod](job.TriggerTime),
                options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneProvider.GetTimeZone()
                });

        _logger.LogInformation(LogMessages.JobScheduled, job.Notification.Text, job.TriggerTime.TimePeriod);
        _logger.LogInformation(LogMessages.JobsNumber, _storageConnection.GetRecurringJobs().Count);
    }

    private void CheckIfCanSchedule()
    {
        if (_storageConnection.GetRecurringJobs().Count == _maximumRecurringJobsNumber)
        {
            throw new Exception(ErrorMessages.ScheduleOverflowing);
        }
    }

    private void CheckIfCanUnschedule(string endpoint)
    {
        if (!_storageConnection.GetRecurringJobs().Any(job => job.Id.Contains(endpoint)))
        {
            throw new Exception(ErrorMessages.CanNotUnscheduleJobs);
        }
    }
}
