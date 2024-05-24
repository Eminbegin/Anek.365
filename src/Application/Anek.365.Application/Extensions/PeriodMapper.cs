using Anek._365.Application.Models;
using Anek._365.Application.Tools;

namespace Anek._365.Application.Extensions;

public static class PeriodMapper
{
    public static DateTimeOffset ToDateTimeOffset(this Period period)
    {
        return period switch
        {
            Period.None => DateTimeOffset.MinValue,
            Period.AllTime => DateTimeOffset.MinValue,
            Period.Week => Calendar.CurrentDateTimeOffset().AddDays(-7),
            Period.Month => Calendar.CurrentDateTimeOffset().AddMonths(-1),
            Period.Year => Calendar.CurrentDateTimeOffset().AddYears(-1),
            _ => throw new ArgumentOutOfRangeException(nameof(period), period, null),
        };
    }
}