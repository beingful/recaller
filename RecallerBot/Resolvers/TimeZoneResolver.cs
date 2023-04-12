using Hangfire;
using Hangfire.Annotations;

namespace RecallerBot.Resolvers;

internal sealed class TimeZoneResolver : ITimeZoneResolver
{
    private const string _utcPlus2 = "E. Europe Standard Time";
    private const string _utcPlus3 = "Arab Standard Time";

    public TimeZoneInfo GetTimeZoneById([NotNull] string timeZoneId)
    {
        string currentTimeZoneId = GetCurrentTimezoneId(DateTime.UtcNow);

        return TimeZoneInfo.FindSystemTimeZoneById(currentTimeZoneId);
    }

    private string GetCurrentTimezoneId(DateTime now)
    {
        DateTime lastSundayOfMarch = GetLastSundayOfMonth(now, 3, 3);
        DateTime lastSundayOfOctober = GetLastSundayOfMonth(now, 10, 4);

        return now >= lastSundayOfMarch && now < lastSundayOfOctober
            ? _utcPlus2
            : _utcPlus3;
    }

    private DateTime GetLastSundayOfMonth(DateTime dateTime, int month, int hour)
    {
        DateTime lastDayOfMonth = new(
            year: dateTime.Year,
            month: dateTime.Month,
            day: DateTime.DaysInMonth(dateTime.Year, month),
            hour: hour,
            minute: 0,
            second: 0);

        DateTime lastSundayOfMonth = lastDayOfMonth;

        while (lastSundayOfMonth.DayOfWeek != DayOfWeek.Sunday)
        {
            lastSundayOfMonth = lastSundayOfMonth.AddDays(-1);
        }

        return lastSundayOfMonth;
    }
}
