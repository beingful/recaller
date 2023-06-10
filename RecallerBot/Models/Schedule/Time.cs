using RecallerBot.Enums;

namespace RecallerBot.Models.Schedule;

internal sealed class Time
{
    public TimePeriod TimePeriod { get; init; }

    public int Hour { get; init; }

    public int Minute { get; init; }

    public int MinuteInterval { get; init; }
}
