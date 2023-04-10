using RecallerBot.Models;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using RecallerBot.Constants;

namespace RecallerBot.Services;

internal sealed class WebhookService : IHostedService
{
    private readonly BotConfiguration _botConfiguration;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(
        BotConfiguration botConfiguration,
        ITelegramBotClient botClient,
        ILogger<WebhookService> logger)
    {
        _botConfiguration = botConfiguration;
        _botClient = botClient;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var webhookAddress = @$"{_botConfiguration.HostAddress}/bot/{_botConfiguration.EscapedBotToken}";

        _logger.LogInformation(LogMessages.SetWebhook, webhookAddress);

        try
        {
            await _botClient.SetWebhookAsync(
            url: webhookAddress,
            allowedUpdates: Array.Empty<UpdateType>(),
            cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(LogMessages.Error, exception.Message, exception.StackTrace);

            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(LogMessages.RemoveWebhook);

        await _botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}
