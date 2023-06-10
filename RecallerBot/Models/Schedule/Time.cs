using RecallerBot.Enums;

namespace RecallerBot.Models.Schedule;

internal sealed class Time
{
    public TimePeriod TimePeriod { get; init; }

    public int Hours { get; init; }

    public int Minutes { get; init; }

    public int MinuteInterval { get; init; }
}
