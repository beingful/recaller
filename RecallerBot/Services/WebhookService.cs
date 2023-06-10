using RecallerBot.Constants;
using RecallerBot.Interfaces;
using RecallerBot.Models.Configuration;

namespace RecallerBot.Services;

internal sealed class WebhookService : IHostedService
{
    private readonly Bot _bot;
    private readonly IBotEndpointService _botEndpointService;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(
        Bot bot,
        IBotEndpointService botEndpointService,
        ILogger<WebhookService> logger)
    {
        _bot = bot;
        _botEndpointService = botEndpointService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(LogMessages.SetWebhook, _bot.WebhookUrl);

        try
        {
            await _botEndpointService.SetWebhookAsync(_bot.WebhookUrl, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(ErrorMessages.GeneralError, exception.Message, exception.StackTrace);

            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(LogMessages.RemoveWebhook);

        await _botEndpointService.DeleteWebhooksAsync(cancellationToken);
    }
}
