using Flurl.Http;
using RecallerBot.Constants;
using RecallerBot.Interfaces;
using RecallerBot.Models;

namespace RecallerBot.Services;

internal sealed class BotRequestService : INotificationService
{
    private readonly string _sendMessageUrl;
    private readonly ILogger<BotRequestService> _logger;

    public BotRequestService(IConfiguration configuration, ILogger<BotRequestService> logger)
    {
        _sendMessageUrl = configuration
                            .GetSection(ConfigurationConstants.BotRequestUrl)
                            .Get<string>()!;
        _logger = logger;
    }

    public async Task SendAsync(Notification notification)
    {
        try
        {
            await _sendMessageUrl.PostUrlEncodedAsync(new
            {
                chat_id = notification.ChatId,
                text = notification.Text
            });

            _logger.LogInformation(LogMessages.BotDoesNotSleep);
        }
        catch (Exception ex)
        {
            _logger.LogError(ErrorMessages.GeneralError, ex.Message, ex.StackTrace);
            _logger.LogError(ErrorMessages.CanNotRequestBot, _sendMessageUrl);
        }
    }
}
