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

        string headers = "All headers:";

        httpContext.Request.Headers.ToList().ForEach(header => headers += $@"
        ({header.Key}, {header.Value})
        ");

        string claims = "All claims:";

        httpContext.User.Claims.ToList().ForEach(claim => claims += $@"
        ({claim.Type}, {claim.Value})
        ");

        _logger.LogInformation(claims);

        return headers + claims;
    }
}
