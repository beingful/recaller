namespace RecallerBot.Constants;

internal static class LogMessages
{
    public const string SetWebhook = "Setting webhook: {webhookAddress}";

    public const string RemoveWebhook = "Removing webhook";

    public const string NotTextMessage = "Bot received something but not a text message";

    public const string MessageReceived = "Received a message \"{message}\" from chat with id {chatId}";

    public const string ChatNotAllowed = "Chat is not allowed";

    public const string CommandNotRecognized = "Command is not recognized";

    public const string StartHandleMessage = "Start handle the message";

    public const string Error = "There is an error. Message: {message} \n Stacktrace: {stacktrace}";

    public const string JobScheduled = "The job with message {message} and cron {cron} is scheduled";

    public const string AllJobsUnscheduled = "All jobs are unscheduled. Current number of jobs: {jobsNumber}";
}
