namespace Dinners.Domain.Common;

public sealed record TimeRange
{
    public DateTime Start   { get; set; } 

    public DateTime End { get; set; }

    public TimeRange(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }
}
