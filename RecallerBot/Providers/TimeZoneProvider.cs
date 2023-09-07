using RecallerBot.Extensions;

namespace RecallerBot.Providers;

internal static class TimeZoneProvider
{
    private const string _utcPlus2 = "E. Europe Standard Time";
    private const string _utcPlus3 = "Arab Standard Time";

    public static TimeZoneInfo GetTimeZone()
    {
        string currentTimeZoneId = GetCurrentTimezoneId();

        return TimeZoneInfo.FindSystemTimeZoneById(currentTimeZoneId);
    }

    private static string GetCurrentTimezoneId()
    {
        DateTime now = DateTime.UtcNow;

        DateTime lastSundayOfMarch = now.GetLastSundayOfMonthWithTime(month: 3, hour: 3);
        DateTime lastSundayOfOctober = now.GetLastSundayOfMonthWithTime(month: 10, hour: 4);

        return now >= lastSundayOfMarch && now < lastSundayOfOctober
            ? _utcPlus2
            : _utcPlus3;
    }
}
