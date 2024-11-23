namespace Locker.ExternalServices.BestWondService.Request;


public class LockerCreateOrderRequest{
 
    public string DeviceNumber{get;set;}

    public required string OrderId{get;set;}

    public required string BoxSize{get;set;}
}