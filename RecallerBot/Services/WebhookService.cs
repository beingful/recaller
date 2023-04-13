using RecallerBot.Models;
using RecallerBot.Constants;
using RecallerBot.Interfaces;

namespace RecallerBot.Services;

internal sealed class WebhookService : IHostedService
{
    private readonly BotConfiguration _botConfiguration;
    private readonly BotScheduleService _scheduleService;
    private readonly IBotEndpointService _botEndpointService;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(
        BotScheduleService scheduleService,
        BotConfiguration botConfiguration,
        IBotEndpointService botEndpointService,
        ILogger<WebhookService> logger)
    {
        _botConfiguration = botConfiguration;
        _scheduleService = scheduleService;
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

            _scheduleService.ScheduleBotTriggering(_botConfiguration.TestChat, _botConfiguration.DoNotSleepCommand);
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

        await _botEndpointService.DeleteWebhooksAsync(cancellationToken);
    }
}
