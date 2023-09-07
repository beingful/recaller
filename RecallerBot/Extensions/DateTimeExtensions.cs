namespace RecallerBot.Extensions;

internal static class DateTimeExtensions
{
    public static DateTime GetLastSundayOfMonthWithTime(this DateTime date, int month, int hour)
    {
        DateTime lastDayOfMonth = new(date.Year, month,
            DateTime.DaysInMonth(date.Year, month),
            hour: hour, minute: 0, second: 0);

        DateTime lastSundayOfMonth = lastDayOfMonth;

        return lastSundayOfMonth.GetLastDayOfMonthByCondition(IsSunday);
    }

    public static DateTime GetLastWeekdayOfMonth(this DateTime date)
    {
        DateTime lastDayOfMonth = new(date.Year, date.Month,
            DateTime.DaysInMonth(date.Year, date.Month));

        DateTime lastWeekdayOfMonth = lastDayOfMonth.GetLastDayOfMonthByCondition(IsNotWeekend);

        return lastWeekdayOfMonth;
    }

    public static bool IsNotFriday(this DateTime date) =>
        date.DayOfWeek != DayOfWeek.Friday;

    private static bool IsNotWeekend(this DateTime date) =>
        date.DayOfWeek != DayOfWeek.Saturday
        && date.DayOfWeek != DayOfWeek.Sunday;

    private static bool IsSunday(this DateTime date) =>
        date.DayOfWeek == DayOfWeek.Sunday;

    private static DateTime GetLastDayOfMonthByCondition(this DateTime date, Predicate<DateTime> condition)
    {
        DateTime lastDayOfMonthByCondition = date;

        while (!condition(lastDayOfMonthByCondition))
        {
            lastDayOfMonthByCondition = lastDayOfMonthByCondition.AddDays(-1);
        }

        return lastDayOfMonthByCondition;
    }
}
