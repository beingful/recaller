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

    public const string JobScheduled = "The job with message {message} and time period {timePeriod} is scheduled";

    public const string JobsWithEndpointUnscheduled = "Jobs with endpoint {endpoint} are unscheduled. Current number of jobs: {jobsNumber}";

    public const string BotDoesNotSleep = "Bot does not sleep";
}
