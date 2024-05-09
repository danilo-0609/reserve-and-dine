using BuildingBlocks.Domain.AggregateRoots;
using System.Text.Json.Serialization;

namespace Users.Domain.UserRegistrations;

public sealed record UserRegistrationId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    public static UserRegistrationId Create(Guid value) => new UserRegistrationId(value);

    public static UserRegistrationId CreateUnique() => new UserRegistrationId(Guid.NewGuid());

    [JsonConstructor]
    private UserRegistrationId(Guid value)
    {
        Value = value;
    }

    private UserRegistrationId()
    {
    }
}
