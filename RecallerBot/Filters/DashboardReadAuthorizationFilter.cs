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

        var claim = httpContext.User.Claims.FirstOrDefault(x => x.Type == _hangfireAccess.ClaimName);

        _logger.LogInformation("Check claim {claimType} with value {claimValue}", claim?.Type, claim?.Value);

        return httpContext.User.Identity?.IsAuthenticated ?? false
            && httpContext.User.Claims.Any(claim =>
                {
                    return claim.Type == _hangfireAccess.ClaimName
                        && claim.Value == _hangfireAccess.ClaimValue;
                });
    }
}
