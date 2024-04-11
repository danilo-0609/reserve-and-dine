using BuildingBlocks.Domain.Entities;
using Newtonsoft.Json;

namespace Dinners.Domain.Menus.Schedules;

public sealed record MenuScheduleId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static MenuScheduleId Create(Guid value) => new MenuScheduleId(value);

    public static MenuScheduleId CreateUnique() => new MenuScheduleId(Guid.NewGuid());

    [JsonConstructor]
    private MenuScheduleId(Guid value)
    {
        Value = value;
    }
}
