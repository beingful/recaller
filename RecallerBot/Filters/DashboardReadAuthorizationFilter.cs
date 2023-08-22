using Hangfire.Dashboard;
using RecallerBot.Models.Configuration;

namespace RecallerBot.Filters;

public sealed class DashboardReadAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly HangfireDashboardAccess _hangfireAccess;
    private readonly ILogger<DashboardReadAuthorizationFilter> _logger;

    public DashboardReadAuthorizationFilter(HangfireDashboardAccess hangfireAccess,
        ILogger<DashboardReadAuthorizationFilter> logger)
    {
        _hangfireAccess = hangfireAccess;
        _logger = logger;
    }

    public bool Authorize(DashboardContext context)
    {
        HttpContext httpContext = context.GetHttpContext();

        string claims = string.Empty;

        httpContext.User.Claims.ToList().ForEach(claim => claims += $"\n({claim.Type}, {claim.Value})\n");

        _logger.LogInformation("All claims: " + claims);

        _logger.LogInformation("IsAuthenticated: " + httpContext.User.Identity?.IsAuthenticated);
        _logger.LogInformation($"{_hangfireAccess.ClaimType}: {httpContext.User.Claims.FirstOrDefault(x => x.Type == _hangfireAccess.ClaimType)?.Value}");

        return httpContext.User.Identity?.IsAuthenticated == true
            && httpContext.User.Claims.Any(claim =>
                {
                    return claim.Type == _hangfireAccess.ClaimType
                        && claim.Value == _hangfireAccess.ClaimValue;
                });
    }
}
