namespace RecallerBot.Interfaces;

internal interface IBotEndpointService
{
    public Task SetWebhookAsync(string url, CancellationToken cancellationToken);

    public Task DeleteWebhooksAsync(CancellationToken cancellationToken);
}
