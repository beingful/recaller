#nullable disable

namespace RecallerBot.Models.Configuration;

internal sealed class Authentication
{
    public string Authority { get; init; }

    public string Audience { get; set; }
}
