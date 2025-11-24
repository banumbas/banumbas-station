namespace Content.Shared._Sunrise.Carrying;

/// <summary>
/// Вызывается на CarriableComponent для проверки возможности взять объект.
/// Системы могут отменить событие, установив Cancelled = true, если объект нельзя взять.
/// </summary>
[ByRefEvent]
public struct CanCarryAttemptEvent
{
    public bool Cancelled;
    public readonly EntityUid Carrier;

    public CanCarryAttemptEvent(EntityUid carrier)
    {
        Carrier = carrier;
        Cancelled = false;
    }
}



