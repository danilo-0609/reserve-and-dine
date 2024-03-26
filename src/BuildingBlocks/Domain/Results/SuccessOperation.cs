namespace BuildingBlocks.Domain.Results;

public sealed record SuccessOperation
{
    private string _value;

    public static SuccessOperation Code => new SuccessOperation(nameof(Code));

    private SuccessOperation(string value)
    {
        _value = value;
    }
}
