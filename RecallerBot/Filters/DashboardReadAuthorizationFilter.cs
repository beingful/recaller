using Hangfire.Dashboard;
using RecallerBot.Models.Configuration;

namespace RecallerBot.Filters;

public sealed class DashboardReadAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly HangfireDashboardAccess _hangfireAccess;

    public DashboardReadAuthorizationFilter(HangfireDashboardAccess hangfireAccess)
    {
        _hangfireAccess = hangfireAccess;
    }

    public bool Authorize(DashboardContext context)
    {
        HttpContext httpContext = context.GetHttpContext();

        return httpContext.User.Identity?.IsAuthenticated == true
            && httpContext.User.Claims.Any(claim =>
                        claim.Type == _hangfireAccess.ClaimType
                        && claim.Value == _hangfireAccess.ClaimValue);
    }
}
