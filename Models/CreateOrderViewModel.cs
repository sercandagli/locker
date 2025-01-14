using Locker.Enums;

namespace Locker.Models;



public class CreateOrderViewModel
{
    public int BoxId { get; set; }

    public string SourceLockerName { get; set; }

    public string TargetLockerName { get; set; }

    public string Name { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public string Address { get; set; }

    public string AddressLat { get; set; }

    public string AddressLong { get; set; }

    public int SelectedSourceRegionId { get; set; }
    public int SelectedTargetRegionId { get; set; }

    public string Note { get; set; }

    public int SourceSubRegionId { get; set; }

    public int TargetSubRegionId { get; set; }

    public string ReceiverName { get; set; }

    public string ReceiverPhone { get; set; }

    public string ReceiverEmail { get; set; }

    public string ReceiverAddress { get; set; }

    public string ReceiverAddressLat { get; set; }

    public string ReceiverAddressLong { get; set; }

    public string Items { get; set; }

    public DeliveryType DeliveryType { get; set; }

    public UserType OrderType { get; set; }

}