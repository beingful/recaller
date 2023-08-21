namespace RecallerBot.Services;

public class GetService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GetService> _logger;

    public GetService(IHttpContextAccessor httpContextAccessor, ILogger<GetService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public string GetClaims()
    {
        HttpContext httpContext = _httpContextAccessor.HttpContext!;

        string claims = string.Empty;

        httpContext.User.Claims.ToList().ForEach(claim => claims += $"\n({claim.Type}, {claim.Value})\n");

        _logger.LogInformation("All claims: " + claims);

        return claims;
    }
}
