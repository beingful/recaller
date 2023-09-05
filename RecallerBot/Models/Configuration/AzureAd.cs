#nullable disable

namespace RecallerBot.Models.Configuration;

internal sealed class AzureAd
{
    public string Instance { get; init; }

    public string Domain { get; init; }

    public string TenantId { get; init; }

    public string Authority { get; init; }

    public string ClientId { get; init; }

    public string ClientSecret { get; init; }

    public string CallbackPath { get; init; }

    public string Scope { get; init; }
}
