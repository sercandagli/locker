namespace Locker.Enums;

public enum OrderItemStatus
{
    OrderCreated = 1,
    AtSourceCabine = 2,
    AtCourier = 3,
    AtTargetCabine = 4,
    Delivered = 5,
    Cancelled = 9
}