using RecallerBot.Models;
using RecallerBot.Constants;
using RecallerBot.Interfaces;

namespace RecallerBot.Services;

internal sealed class WebhookService : IHostedService
{
    private readonly BotConfiguration _botConfiguration;
    private readonly IBotEndpointService _botEndpointService;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(
        BotConfiguration botConfiguration,
        IBotEndpointService botEndpointService,
        ILogger<WebhookService> logger)
    {
        _botConfiguration = botConfiguration;
        _botEndpointService = botEndpointService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var webhookAddress = @$"{_botConfiguration.HostAddress}/bot/{_botConfiguration.EscapedBotToken}";

        _logger.LogInformation(LogMessages.SetWebhook, webhookAddress);

        try
        {
            await _botEndpointService.SetWebhookAsync(webhookAddress, cancellationToken);
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

        await _botEndpointService.DeleteWebhookAsync(cancellationToken);
    }
}
