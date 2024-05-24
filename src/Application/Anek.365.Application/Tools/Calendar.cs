namespace Anek._365.Application.Tools;

public static class Calendar
{
    private static readonly TimeZoneInfo TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

    public static DateTimeOffset CurrentDateTimeOffset()
        => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimeZoneInfo);
}