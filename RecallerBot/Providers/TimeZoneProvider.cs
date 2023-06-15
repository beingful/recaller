﻿namespace RecallerBot.Providers;

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

        DateTime lastSundayOfMarch = GetLastSundayOfMonth(now.Year, 3, 3);
        DateTime lastSundayOfOctober = GetLastSundayOfMonth(now.Year, 10, 4);

        return now >= lastSundayOfMarch && now < lastSundayOfOctober
            ? _utcPlus2
            : _utcPlus3;
    }

    private static DateTime GetLastSundayOfMonth(int year, int month, int hour)
    {
        DateTime lastDayOfMonth = new(
            year: year, month: month, day: DateTime.DaysInMonth(year, month),
            hour: hour, minute: 0, second: 0);

        DateTime lastSundayOfMonth = lastDayOfMonth;

        while (lastSundayOfMonth.DayOfWeek != DayOfWeek.Sunday)
        {
            lastSundayOfMonth = lastSundayOfMonth.AddDays(-1);
        }

        return lastSundayOfMonth;
    }
}