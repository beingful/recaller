namespace RecallerBot.Constants;

internal static class ErrorMessages
{
    public const string GeneralError = "There is an error. Message: {message} \n Stacktrace: {stacktrace}";

    public const string ScheduleOverflowing = "Can not schedule the job. The maximum number of jobs is already reached";

    public const string CanNotRequestBot = "Can not call bot with url {url}";
}
