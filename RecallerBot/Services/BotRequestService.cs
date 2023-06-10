using Flurl.Http;
using RecallerBot.Constants;
using RecallerBot.Interfaces;
using RecallerBot.Models;
using RecallerBot.Models.Configuration;
using RecallerBot.Models.Request;

namespace RecallerBot.Services;

internal sealed class BotRequestService : INotificationService
{
    private readonly Bot _bot;
    private readonly ILogger<BotRequestService> _logger;

    public BotRequestService(Bot bot, ILogger<BotRequestService> logger)
    {
        _bot = bot;
        _logger = logger;
    }

    public async Task SendAsync(Notification notification)
    {
        Update update = new()
        {
            Message = new Message()
            {
                From = new Sender()
                {
                    Id = Convert.ToInt64(_bot.CreatorUserId)
                },
                Chat = new Chat()
                {
                    Id = Convert.ToInt64(notification.ChatId)
                },
                Text = notification.Text
            }
        };

        try
        {
            await _bot.WebhookUrl.PostJsonAsync(update);

            _logger.LogInformation(LogMessages.BotDoesNotSleep);
        }
        catch (Exception ex)
        {
            _logger.LogError(ErrorMessages.GeneralError, ex.Message, ex.StackTrace);
            _logger.LogError(ErrorMessages.CanNotRequestBot, _bot.WebhookUrl);
        }
    }
}
